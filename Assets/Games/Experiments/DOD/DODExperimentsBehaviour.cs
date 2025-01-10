#define USE_GC_HANDLES
//#define USE_MARSHALL_ALLOCHGLOBAL
//#define USE_NATIVEMEMORY

using System;
using System.Globalization;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using HereticalSolutions.Persistence;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using HereticalSolutions.Profiling;

using UnityEngine;

public class DODExperimentsBehaviour : MonoBehaviour
{
    //Courtesies:
    //https://github.com/bepu/bepuphysics2/blob/master/BepuUtilities/Memory/BufferPool.cs
    //https://github.com/Cysharp/NativeMemoryArray/blob/master/src/NativeMemoryArray/NativeMemoryArray.cs
    //https://kylekukshtel.com/csharp-lowlevel-memory-pinvoke-span-blittable-bindings
    
#if USE_NATIVEMEMORY
#else
    /// <summary>
    /// Byte alignment to enforce for all block allocations within the buffer pool.
    /// </summary>
    /// <remarks>Since this only applies at the level of blocks, we can use a pretty beefy value without much concern.</remarks>
    private const int BlockAlignment = 128;
#endif
    
    private const float INITIAL_DIMENSION_HALF_LIMIT = 1000.0f;
    
    //10m is for the purpose of making the difference in performance more visible. I am not sure what kind of game
    //(unless it's a whole ass matrix server) would need 10m entities for game design purposes
    [SerializeField]
    private int entityCount = 10000000;

    //Remember that this value is even lower for mobiles
    [SerializeField]
    private int cacheLineSize = 64;

    //I realize that according to Google, The size of L1 cache typically ranges from 2KB to 64KB so this is still rather
    //a variable than a constant
    //
    //But after running the tests on 10000000 entities and comparing the results to the
    //"Scrambled within cache line indexes" experiment (AVERAGE: 243.83 ms, MEDIAN: 242.59 ms, DEVIATION: 3.41 ms)
    //I've figured out the following:
    //32K: AVERAGE: 338.08 ms, MEDIAN: 338.83 ms, DEVIATION: 2.04 ms
    //16K: AVERAGE: 332.74 ms, MEDIAN: 330.86 ms, DEVIATION: 6.14 ms
    //8K: AVERAGE: 309.87 ms, MEDIAN: 309.54 ms, DEVIATION: 2.06 ms
    //4K: AVERAGE: 289.29 ms, MEDIAN: 288.28 ms, DEVIATION: 3.39 ms
    //2K: AVERAGE: 253.15 ms, MEDIAN: 252.66 ms, DEVIATION: 2.25 ms
    //The last one is "close enough" to cache line scramble for me, maybe i'd even lower the ceiling to 1K but
    //i'll leave it at 2K for now
    [SerializeField]
    private int l1CacheSize = 2048;
    
    [SerializeField]
    private int experimentsCount = 100;

    [SerializeField]
    private bool useAsserts;

    //When we do asserts the performance markers are disabled. You wouldn't want to compare performance to methods that
    //do text log outputs now do you?
    //Neither I expect you to be willing to read over 10m entity worth of logs
    [SerializeField]
    private int assertEntityCountOverride = 10;
    
    //When doing for loop over iterator, sorted indexes or indexes sorted within cache line size we're gaining ~5x
    //performance over iterating over scrambled indexes, thanks to cache locality. I had a thought that the amount of
    //things we can actually do within the single loop iteration could actually be more due to the fact that we're still
    //working with the same cache line before we iterate to the next value. So I've created another experiment in which
    //I'm doing the same operation multiple times for each for() iteration. The results are the following with 5:
    //
    //"Simple for loop" AVERAGE: 236.58 ms, MEDIAN: 236.31 ms, DEVIATION: 2.14 ms
    //"Scrambled within cache line indexes" AVERAGE: 241.56 ms, MEDIAN: 241.49 ms, DEVIATION: 4.57 ms
    //"Scrambled indexes" AVERAGE: 1356.68 ms, MEDIAN: 1352.83 ms, DEVIATION: 13.23 ms
    //"Simple for loop with redundancy" AVERAGE: 1209.92 ms, MEDIAN: 1210.24 ms, DEVIATION: 8.71 ms
    //That means you can do ~5 operations of the same complexity each iteration before your performance hits the same
    //level as the scrambled indexes. My hypothesis haven't proved to be correct but still there is a good side to this
    //experiment: if you plan to design some kind of optimization logic then you know that if it involves more than
    //4 relatively simple operations, you basically lose any performance gained by cache locality
    //I guess this may vary between different hardware but even if you gain more somewhere it doesn't mean that your
    //solution will scale the same on all target platforms. So for now, 5 it is
    [SerializeField]
    private int expectedRedundancyCountToMatchPerformanceLost = 5;
    
    #region Logging
    
    private ILogger logger;

    private ISerializer fileSinkSerializer;

    private bool catchingLogs = true;

    private bool isQuittingApplication;
    
    #endregion

    #region Markers
    
    private const int EXPERIMENT_MARKERS_COUNT = 10;
    
    private ProfilingMarker[] experimentMarkers;

    #endregion

    #region Entities
    
    private ExperimentPosition[] Positions;
    
    private ExperimentVelocity[] Velocities;

    private unsafe byte* positionsPtr;
        
    private unsafe byte* velocitiesPtr;
    
#if USE_GC_HANDLES
    
    private GCHandle positionsHandle;
    
    private GCHandle velocitiesHandle;
    
#endif

    private int[] sortedIndexes;

    private int[] scrambledIndexes;
    
    private int[] scrambledWithinCacheLineIndexes;
    
    private int[] scrambledWithinL1CacheIndexes;

    #endregion

    private int experimentPositionSize;

    private int experimentVelocitySize;

    #region Initialization
    
    private void Awake()
    {
        if (useAsserts)
            entityCount = assertEntityCountOverride;
        
        InitializeLogger();
        
        InitializeMarkers();

        InitializeEntities();
    }

    private void InitializeLogger()
    {
        string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

        dateTimeNow = dateTimeNow.Replace('T', '_');

        dateTimeNow = dateTimeNow.Replace(':', '-');

        string logFileName = dateTimeNow;


        ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

        var loggerResolver = loggerBuilder

                .NewLogger()

                .ToggleAllowedByDefault(
                    true)

                //Wrappers

                .AddWrapperBelow(
                    LoggerFactory.BuildProxyWrapper())

                .Build(); //Preemptively build the logger resolver so that it can be already injected

        loggerBuilder

            //Recursion prevention gate

            //THIS ONE IS PLACED BEFORE THE THREAD SAFETY WRAPPER FOR A REASON
            //IMAGINE AN ERROR LOG GOING IN
            //THE SEMAPHORE IS LOCKED
            //THE LOG IS GOING THROUGH ALL OF THE WRAPPERS AND REACHES UNITY DEBUG LOG BOTTOM WRAPPER
            //THE ERROR IS LOGGED WITH Debug.LogError
            //THEN THE FUN STARTS
            //THIS INSTALLER IS SUBSCRIBED TO UNITYS LOGS
            //IT SENDS IT DOWN THE LOGGER
            //WHERE IT REACHES THE FUCKING SEMAPHORE
            //AND WAITS FOR IT TO SPIN
            //WHILE Debug.LogError IS ACTUALLY A BLOCKING CALL
            //SO IT WONT START GOING UP THE CHAIN OF DELEGATES AND SPIN THE SEMAPHORE UNTIL THE CALLBACK IS FINISHED
            //AND CALLBACK WONT FINISH AS IT WAITS FOR THE SEMAPHORE TO SPIN
            //MAKING A DEADLOCK
            //THE EASIEST WAY TO PREVENT THIS IS TO PERFORM A RECURSION GATE BEFORE THE SEMAPHORE

            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithRecursionPreventionGate())

            //Thread safety

            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithSemaphoreSlim())

            //Prefixes

            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithThreadIndexPrefix())
            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(
                    false))

            // File sink

            .Branch();

        var branch = loggerBuilder.CurrentLogger;

        var fileSink = LoggerFactory.BuildFileSink(
            new FileAtApplicationDataPathSettings()
            {
                RelativePath = $"../Runtime logs/{logFileName}.log"
            },
            loggerResolver);

        fileSinkSerializer = fileSink.Serializer;

        loggerBuilder.AddSink(
            fileSink);

        loggerBuilder.CurrentLogger = branch;

        // Recursion prevention prefix

        loggerBuilder

            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithRecursionPreventionPrefix())

            //Toggling

            .AddWrapperBelow(
                LoggerFactory.BuildLoggerWrapperWithToggling(
                    true,
                    true,
                    true,
                    true))

            // Sink

            .AddSink(
                LoggerFactoryUnity.BuildUnityDebugLogSink());

        //Open stream

        var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

        streamStrategy?.InitializeAppend();

        logger = loggerBuilder.CurrentLogger;

        #region Catch logs

        if (catchingLogs)
        {
            Application.logMessageReceivedThreaded -= ReceivedLog;
            Application.logMessageReceivedThreaded += ReceivedLog;
        }

        #endregion
    }
    
    private void ReceivedLog(
        string logString,
        string stackTrace,
        LogType logType)
    {
#if UNITY_EDITOR
        if( isQuittingApplication )
            return;
#endif
            
        string log = string.IsNullOrEmpty(stackTrace)
            ? logString
            : $"{logString}\n{stackTrace}";

        switch(logType)
        {
            case LogType.Log:
                    
                logger.Log(
                    log);
                    
                break;
                
            case LogType.Warning:
                    
                logger.LogWarning(
                    log);
                    
                break;
                
            case LogType.Error:
                    
                logger.LogError(
                    log);
                    
                break;
                
            case LogType.Assert:
                    
                logger.Log(
                    log);
                    
                break;
                
            case LogType.Exception:
                    
                logger.LogError(
                    log);
                    
                break;
        }


    }

    private void InitializeMarkers()
    {
        experimentMarkers = new ProfilingMarker[EXPERIMENT_MARKERS_COUNT];
        
        for (int i = 0; i < EXPERIMENT_MARKERS_COUNT; i++)
        {
            experimentMarkers[i] = ProfilingManager.AllocateMarker($"Experiment {i} marker");
        }
    }

    private void InitializeEntities()
    {
        experimentPositionSize = Marshal.SizeOf(typeof(ExperimentPosition));
        
        logger.Log($"ExperimentPosition SIZE: {experimentPositionSize}");
        
        experimentVelocitySize = Marshal.SizeOf(typeof(ExperimentVelocity));
        
        logger.Log($"ExperimentVelocity SIZE: {experimentVelocitySize}");
        
        InitializePositions();
        
        InitializeVelocities();
        
        InitializePositionsUnsafe();
        
        InitializeVelocitiesUnsafe();
        
        InitializeSortedIndexes();
        
        InitializeScrambledIndexes();
        
        InitializeScrambledWithinCacheLineIndexes();

        InitializeScrambledWithinL1CacheIndexes();
    }

    private void InitializePositions()
    {
        Positions = new ExperimentPosition[entityCount];

        for (int i = 0; i < Positions.Length; i++)
        {
            Positions[i] = new ExperimentPosition
            {
                Position = new Vector3(
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT))
            };

            if (useAsserts)
            {
                logger.Log($"Initialized position {i} with value [{Positions[i].Position.x}; {Positions[i].Position.y}; {Positions[i].Position.z}]");
            }
        }
    }

    private void InitializeVelocities()
    {
        Velocities = new ExperimentVelocity[entityCount];

        for (int i = 0; i < Velocities.Length; i++)
        {
            Velocities[i] = new ExperimentVelocity
            {
                Velocity = new Vector3(
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT))
            };
            
            if (useAsserts)
            {
                logger.Log($"Initialized velocity {i} with value [{Velocities[i].Velocity.x}; {Velocities[i].Velocity.y}; {Velocities[i].Velocity.z}]");
            }
        }
    }

    private unsafe void InitializePositionsUnsafe()
    {
#if USE_GC_HANDLES
        
        positionsHandle = GCHandle.Alloc(
            new ExperimentPosition[entityCount],
            GCHandleType.Pinned);
        
        positionsPtr = (byte*)positionsHandle.AddrOfPinnedObject();
        
#elif USE_MARSHALL_ALLOCHGLOBAL

        int BlockSize = entityCount * experimentPositionSize;

        positionsPtr = (byte*)Marshal.AllocHGlobal(BlockSize).ToPointer();
        
#elif USE_NATIVEMEMORY

        int BlockSize = entityCount * experimentPositionSize;

        positionsPtr = (byte*)NativeMemory.AlignedAlloc(
            (nuint)BlockSize,
            BlockAlignment);
        
#endif

        for (int i = 0; i < entityCount; i++)
        {
            ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                positionsPtr + i * experimentPositionSize);
            
            currentPosition = new ExperimentPosition
            {
                Position = new Vector3(
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT))
            };
            
            if (useAsserts)
            {
                logger.Log($"Initialized unsafe position {i} with value [{currentPosition.Position.x}; {currentPosition.Position.y}; {currentPosition.Position.z}]");
            }
        }
    }
    
    private unsafe void InitializeVelocitiesUnsafe()
    {
#if USE_GC_HANDLES
        
        velocitiesHandle = GCHandle.Alloc(
            new ExperimentVelocity[entityCount],
            GCHandleType.Pinned);
        
        velocitiesPtr = (byte*)velocitiesHandle.AddrOfPinnedObject();
        
#elif USE_MARSHALL_ALLOCHGLOBAL

        int BlockSize = entityCount * experimentVelocitySize;

        velocitiesPtr = (byte*)Marshal.AllocHGlobal(BlockSize).ToPointer();

#elif USE_NATIVEMEMORY

        int BlockSize = entityCount * experimentVelocitySize;

        velocitiesPtr = (byte*)NativeMemory.AlignedAlloc(
            (nuint)BlockSize,
            BlockAlignment);
#endif

        for (int i = 0; i < entityCount; i++)
        {
            ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                velocitiesPtr + i * experimentVelocitySize);
            
            currentVelocity = new ExperimentVelocity
            {
                Velocity = new Vector3(
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT),
                    UnityEngine.Random.Range(-INITIAL_DIMENSION_HALF_LIMIT, INITIAL_DIMENSION_HALF_LIMIT))
            };
            
            if (useAsserts)
            {
                logger.Log($"Initialized unsafe velocity {i} with value [{currentVelocity.Velocity.x}; {currentVelocity.Velocity.y}; {currentVelocity.Velocity.z}]");
            }
        }
    }
    
    private void InitializeSortedIndexes()
    {
        sortedIndexes = new int[entityCount];

        for (int i = 0; i < sortedIndexes.Length; i++)
            sortedIndexes[i] = i;
    }

    private void InitializeScrambledIndexes()
    {
        scrambledIndexes = new int[entityCount];

        for (int i = 0; i < scrambledIndexes.Length; i++)
            scrambledIndexes[i] = i;

        for (int i = 0; i < scrambledIndexes.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, scrambledIndexes.Length);

            int temp = scrambledIndexes[i];

            scrambledIndexes[i] = scrambledIndexes[randomIndex];

            scrambledIndexes[randomIndex] = temp;
        }
    }
    
    private void InitializeScrambledWithinCacheLineIndexes()
    {
        int componentsPerCacheLine = cacheLineSize / experimentPositionSize;
        
        logger.Log($"COMPONENTS PER CACHE LINE: {componentsPerCacheLine}");
        
        scrambledWithinCacheLineIndexes = new int[entityCount];

        for (int i = 0; i < scrambledWithinCacheLineIndexes.Length; i++)
            scrambledWithinCacheLineIndexes[i] = i;

        for (int i = 0; i < scrambledWithinCacheLineIndexes.Length; i++)
        {
            int cacheLineStartIndex = i - i % componentsPerCacheLine;
            
            int cacheLineFinishIndex = cacheLineStartIndex + componentsPerCacheLine - 1;
            
            cacheLineFinishIndex = (cacheLineFinishIndex >= entityCount)
                ? entityCount - 1
                : cacheLineFinishIndex;
            
            int randomIndex = UnityEngine.Random.Range(
                cacheLineStartIndex,
                cacheLineFinishIndex + 1);

            int temp = scrambledWithinCacheLineIndexes[i];
            
            scrambledWithinCacheLineIndexes[i] = scrambledWithinCacheLineIndexes[randomIndex];
            
            scrambledWithinCacheLineIndexes[randomIndex] = temp;
        }
    }
    
    private void InitializeScrambledWithinL1CacheIndexes()
    {
        int componentsPerL1Cache = l1CacheSize / experimentPositionSize;
        
        logger.Log($"COMPONENTS PER L1 CACHE: {componentsPerL1Cache}");
        
        scrambledWithinL1CacheIndexes = new int[entityCount];

        for (int i = 0; i < scrambledWithinL1CacheIndexes.Length; i++)
            scrambledWithinL1CacheIndexes[i] = i;

        for (int i = 0; i < scrambledWithinL1CacheIndexes.Length; i++)
        {
            int l1CacheStartIndex = i - i % componentsPerL1Cache;
            
            int l1CacheFinishIndex = l1CacheStartIndex + componentsPerL1Cache - 1;
            
            l1CacheFinishIndex = (l1CacheFinishIndex >= entityCount)
                ? entityCount - 1
                : l1CacheFinishIndex;
            
            int randomIndex = UnityEngine.Random.Range(
                l1CacheStartIndex,
                l1CacheFinishIndex + 1);

            int temp = scrambledWithinL1CacheIndexes[i];
            
            scrambledWithinL1CacheIndexes[i] = scrambledWithinL1CacheIndexes[randomIndex];
            
            scrambledWithinL1CacheIndexes[randomIndex] = temp;
        }
    }
    
    #endregion

    void Start()
    {
        logger?.Log($"STARTING EXPERIMENTS. ENTITY COUNT: {entityCount} CACHE LINE SIZE: {cacheLineSize} EXPERIMENTS COUNT: {experimentsCount}");
        
        PerformExperiment(
            "Simple for loop",
            experimentMarkers[0],
            IterateOverEntitiesSimpleForLoop);
        
        PerformExperiment(
            "Sorted indexes",
            experimentMarkers[1],
            IterateOverEntitiesSortedIndexes);
        
        PerformExperiment(
            "Scrambled indexes",
            experimentMarkers[2],
            IterateOverEntitiesScrambledIndexes);
        
        PerformExperiment(
            "Scrambled within cache line indexes",
            experimentMarkers[3],
            IterateOverEntitiesScrambledWithinCacheLineIndexes);
        
        PerformExperiment(
            "Scrambled within L1 cache indexes",
            experimentMarkers[4],
            IterateOverEntitiesScrambledWithinL1CacheIndexes);
        
        PerformExperiment(
            "Simple for loop with redundancy",
            experimentMarkers[5],
            IterateOverEntitiesSimpleForLoopWithRedundancy);
        
        PerformExperiment(
            "Unsafe for loop",
            experimentMarkers[6],
            IterateOverEntitiesUnsafeForLoop);
        
        PerformExperiment(
            "Unsafe for loop with pointer arithmetics",
            experimentMarkers[7],
            IterateOverEntitiesUnsafeForLoopWithPointerArithmetics);
        
        PerformExperiment(
            "Unsafe for loop with scrambled indexes",
            experimentMarkers[8],
            IterateOverEntitiesUnsafeForLoopWithScrambledIndexes);
        
        PerformExperiment(
            "Unsafe for loop with scrambled within cache line indexes",
            experimentMarkers[9],
            IterateOverEntitiesUnsafeForLoopWithScrambledWithinCacheLineIndexes);
        
        Cleanup();
    }

    #region Experiments
    
    private void PerformExperiment(
        string experimentName,
        ProfilingMarker experimentMarker,
        Action experimentDelegate)
    {
        logger?.Log($"EXPERIMENT \"{experimentName}\" COMMENCING");

        double[] results = new double[experimentsCount];
        
        for (int i = 0; i < experimentsCount; i++)
        {
            if (!useAsserts)
            {
                experimentMarker.Clear();

                experimentMarker.Start();

                
                experimentDelegate?.Invoke();

                
                experimentMarker.Stop();

                results[i] = experimentMarker.ElapsedMilliseconds;
            }
            else
            {
                experimentDelegate?.Invoke();
            }
        }

        if (!useAsserts)
        {
            double average = 0.0;

            double median = 0.0;

            double deviation = 0.0;

            for (int i = 0; i < experimentsCount; i++)
            {
                average += results[i];
            }

            average /= experimentsCount;

            Array.Sort(results);

            if (experimentsCount % 2 == 0)
            {
                median = (results[experimentsCount / 2 - 1] + results[experimentsCount / 2]) / 2.0;
            }
            else
            {
                median = results[experimentsCount / 2];
            }

            for (int i = 0; i < experimentsCount; i++)
            {
                deviation += Math.Pow(results[i] - average, 2);
            }

            deviation = Math.Sqrt(deviation / experimentsCount);

            logger?.Log(
                $"EXPERIMENT \"{experimentName}\" FINISHED. AVERAGE: {average} ms, MEDIAN: {median} ms, DEVIATION: {deviation} ms");
        }
        else
        {
            logger?.Log($"EXPERIMENT \"{experimentName}\" FINISHED");
        }
    }
    
    private void IterateOverEntitiesSimpleForLoop()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                Positions[i].Position += Velocities[i].Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                var previousPosition = Positions[i].Position;

                Positions[i].Position += Velocities[i].Velocity;

                logger.Log(
                    $"{i}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{Velocities[i].Velocity.x}; {Velocities[i].Velocity.y}; {Velocities[i].Velocity.z}] = [{Positions[i].Position.x}; {Positions[i].Position.y}; {Positions[i].Position.z}]");
            }
        }
    }
    
    private void IterateOverEntitiesSortedIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = sortedIndexes[i];

                Positions[index].Position += Velocities[index].Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = sortedIndexes[i];

                var previousPosition = Positions[index].Position;

                Positions[index].Position += Velocities[index].Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{Velocities[index].Velocity.x}; {Velocities[index].Velocity.y}; {Velocities[index].Velocity.z}] = [{Positions[index].Position.x}; {Positions[index].Position.y}; {Positions[index].Position.z}]");
            }
        }
    }

    private void IterateOverEntitiesScrambledIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledIndexes[i];

                Positions[index].Position += Velocities[index].Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledIndexes[i];

                var previousPosition = Positions[index].Position;

                Positions[index].Position += Velocities[index].Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{Velocities[index].Velocity.x}; {Velocities[index].Velocity.y}; {Velocities[index].Velocity.z}] = [{Positions[index].Position.x}; {Positions[index].Position.y}; {Positions[index].Position.z}]");
            }
        }
    }
    
    private void IterateOverEntitiesScrambledWithinCacheLineIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinCacheLineIndexes[i];

                Positions[index].Position += Velocities[index].Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinCacheLineIndexes[i];

                var previousPosition = Positions[index].Position;

                Positions[index].Position += Velocities[index].Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{Velocities[index].Velocity.x}; {Velocities[index].Velocity.y}; {Velocities[index].Velocity.z}] = [{Positions[index].Position.x}; {Positions[index].Position.y}; {Positions[index].Position.z}]");
            }
        }
    }
    
    private void IterateOverEntitiesScrambledWithinL1CacheIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinL1CacheIndexes[i];

                Positions[index].Position += Velocities[index].Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinL1CacheIndexes[i];

                var previousPosition = Positions[index].Position;

                Positions[index].Position += Velocities[index].Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{Velocities[index].Velocity.x}; {Velocities[index].Velocity.y}; {Velocities[index].Velocity.z}] = [{Positions[index].Position.x}; {Positions[index].Position.y}; {Positions[index].Position.z}]");
            }
        }
    }

    private void IterateOverEntitiesSimpleForLoopWithRedundancy()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                for (int j = 0; j < expectedRedundancyCountToMatchPerformanceLost; j++)
                {
                    Positions[i].Position += Velocities[i].Velocity;
                }
            }
        }
    }

    private unsafe void IterateOverEntitiesUnsafeForLoop()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + i * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + i * experimentVelocitySize);

                currentPosition.Position += currentVelocity.Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + i * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + i * experimentVelocitySize);

                var previousPosition = currentPosition.Position;

                currentPosition.Position += currentVelocity.Velocity;

                logger.Log(
                    $"{i}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{currentVelocity.Velocity.x}; {currentVelocity.Velocity.y}; {currentVelocity.Velocity.z}] = [{currentPosition.Position.x}; {currentPosition.Position.y}; {currentPosition.Position.z}]");
            }
        }
    }
    
    private unsafe void IterateOverEntitiesUnsafeForLoopWithPointerArithmetics()
    {
        ExperimentPosition* currentPosition = (ExperimentPosition*)positionsPtr;
            
        ExperimentVelocity* currentVelocity = (ExperimentVelocity*)velocitiesPtr;

        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                currentPosition->Position += currentVelocity->Velocity;

                currentPosition++;

                currentVelocity++;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {

                var previousPosition = currentPosition->Position;

                currentPosition->Position += currentVelocity->Velocity;

                logger.Log(
                    $"{i}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{currentVelocity->Velocity.x}; {currentVelocity->Velocity.y}; {currentVelocity->Velocity.z}] = [{currentPosition->Position.x}; {currentPosition->Position.y}; {currentPosition->Position.z}]");

                currentPosition++;

                currentVelocity++;
            }
        }
    }
    
    private unsafe void IterateOverEntitiesUnsafeForLoopWithScrambledIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledIndexes[i];
                
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + index * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + index * experimentVelocitySize);

                currentPosition.Position += currentVelocity.Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledIndexes[i];
                
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + index * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + index * experimentVelocitySize);

                var previousPosition = currentPosition.Position;

                currentPosition.Position += currentVelocity.Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{currentVelocity.Velocity.x}; {currentVelocity.Velocity.y}; {currentVelocity.Velocity.z}] = [{currentPosition.Position.x}; {currentPosition.Position.y}; {currentPosition.Position.z}]");
            }
        }
    }
    
    private unsafe void IterateOverEntitiesUnsafeForLoopWithScrambledWithinCacheLineIndexes()
    {
        if (!useAsserts)
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinCacheLineIndexes[i];
                
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + index * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + index * experimentVelocitySize);

                currentPosition.Position += currentVelocity.Velocity;
            }
        }
        else
        {
            for (int i = 0; i < entityCount; i++)
            {
                int index = scrambledWithinCacheLineIndexes[i];
                
                ref ExperimentPosition currentPosition = ref Unsafe.AsRef<ExperimentPosition>(
                    positionsPtr + index * experimentPositionSize);

                ref ExperimentVelocity currentVelocity = ref Unsafe.AsRef<ExperimentVelocity>(
                    velocitiesPtr + index * experimentVelocitySize);

                var previousPosition = currentPosition.Position;

                currentPosition.Position += currentVelocity.Velocity;

                logger.Log(
                    $"{index}: [{previousPosition.x}; {previousPosition.y}; {previousPosition.z}] + [{currentVelocity.Velocity.x}; {currentVelocity.Velocity.y}; {currentVelocity.Velocity.z}] = [{currentPosition.Position.x}; {currentPosition.Position.y}; {currentPosition.Position.z}]");
            }
        }
    }
    
    #endregion

    private unsafe void Cleanup()
    {
        if (catchingLogs)
        {
            Application.logMessageReceivedThreaded -= ReceivedLog;
        }

        if (fileSinkSerializer != null)
        {
            var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

            streamStrategy?.FinalizeAppend();
        }

#if USE_GC_HANDLES

        if (positionsHandle.IsAllocated)
        {
            logger.Log("Freeing positions pointer");

            positionsHandle.Free();
        }
        
        if (velocitiesHandle.IsAllocated)
        {
            logger.Log("Freeing velocities pointer");

            velocitiesHandle.Free();
        }
        
#elif USE_MARSHALL_ALLOCHGLOBAL
        if (!Unsafe.IsNullRef(
            ref Unsafe.AsRef<byte>(
                positionsPtr)))
        {
            logger.Log("Freeing positions pointer");
            
            Marshal.FreeHGlobal(new IntPtr(positionsPtr));
        }

        if (!Unsafe.IsNullRef(
            ref Unsafe.AsRef<byte>(
                velocitiesPtr)))
        {
            logger.Log("Freeing velocities pointer");
            
            Marshal.FreeHGlobal(new IntPtr(velocitiesPtr));
        }

#elif USE_NATIVEMEMORY
        if (!Unsafe.IsNullRef(
            ref Unsafe.AsRef<byte>(
                positionsPtr)))
        {
            logger.Log("Freeing positions pointer");

            NativeMemory.Free(positionsPtr);
        }
        
        if (!Unsafe.IsNullRef(
            ref Unsafe.AsRef<byte>(
                velocitiesPtr)))
        {
            logger.Log("Freeing velocities pointer");

            NativeMemory.Free(velocitiesPtr);
        }
#endif
    }

    void Update()
    {
        
    }
    
#if UNITY_EDITOR
#if UNITY_2018_1_OR_NEWER
    private void OnApplicationQuitting()
#else
		private void OnApplicationQuit()
#endif
    {
        isQuittingApplication = true;
    }
#endif

    private void OnDisable()
    {
        Cleanup();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    struct ExperimentPosition
    {
        public Vector3 Position;
    }

    struct ExperimentVelocity
    {
        public Vector3 Velocity;
    }
}
