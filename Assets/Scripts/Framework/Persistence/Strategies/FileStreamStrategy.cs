using System;
using System.Threading.Tasks;

using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class FileStreamStrategy
        : ISerializationStrategy,
          IAsyncSerializationStrategy,
          IStrategyWithIODestination,
          IStrategyWithStream,
          IStrategyWithState,
          IBlockSerializationStrategy,
          IAsyncBlockSerializationStrategy,
          IStrategyWithFilter
    {
        private readonly bool flushAutomatically;

        private readonly ILogger logger;


        public string FullPath { get; private set; }


        private FileStream fileStream;

        public FileStream FileStream { get { return fileStream; } }


        public FileStreamStrategy(
            string fullPath,
            bool flushAutomatically = true,
            ILogger logger = null)
        {
            FullPath = fullPath;

            this.flushAutomatically = flushAutomatically;

            this.logger = logger;


            CurrentMode = EStreamMode.NONE;

            StreamOpen = false;

            fileStream = default(FileStream);
        }


        #region ISerializationStrategy

        #region Read

        public bool Read<TValue>(
            out TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = fileStream.Read(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            value = result.CastFromTo<byte[], TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = fileStream.Read(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            value = result;

            return true;
        }

        #endregion

        #region Write

        public bool Write<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.APPEND);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.READ);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            fileStream.Write(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #endregion

        #region IAsyncSerializationStrategy

        #region Read

        public async Task<(bool, TValue)> ReadAsync<TValue>(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = await fileStream.ReadAsync(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            TValue value = result.CastFromTo<byte[], TValue>();

            return (true, value);
        }

        public async Task<(bool, object)> ReadAsync(
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.READ);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            // Read the source file into a byte array.
            byte[] result = new byte[fileStream.Length];

            int numBytesToRead = (int)fileStream.Length;

            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = await fileStream.ReadAsync(
                    result,
                    numBytesRead,
                    numBytesToRead);

                // Break when the end of the file is reached.
                if (n == 0)
                    break;

                numBytesRead += n;

                numBytesToRead -= n;
            }

            numBytesToRead = result.Length;

            object value = result;

            return (true, value);
        }

        #endregion

        #region Write

        public async Task<bool> WriteAsync<TValue>(
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        public async Task<bool> WriteAsync(
            Type valueType,
            object value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        #endregion

        #region Append

        public async Task<bool> AppendAsync<TValue>(
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.APPEND);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        public async Task<bool> AppendAsync(
            Type valueType,
            object value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.APPEND);

            byte[] contents = value.CastFromTo<object, byte[]>();

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

            int numBytesToWrite = contents.Length;

            // Write the byte array to the other FileStream.
            await fileStream.WriteAsync(
                contents,
                0,
                numBytesToWrite);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        #endregion

        #endregion

        #region IStrategyWithIODestination

        public void EnsureIOTargetDestinationExists()
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());
        }

        public bool IOTargetExists()
        {
            return IOHelpers.FileExists(
                FullPath,
                logger,
                GetType());
        }

        public void CreateIOTarget()
        {
            string savePath = FullPath;

            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            if (!IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Create(
                    FullPath);
            }
        }

        public void EraseIOTarget()
        {
            string savePath = FullPath;

            if (IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Delete(savePath);
            }
        }

        #endregion

        #region IStrategyWithStream

        public EStreamMode CurrentMode { get; private set; }

        public Stream Stream
        {
            get
            {
                switch (CurrentMode)
                {
                    case EStreamMode.READ:
                    case EStreamMode.WRITE:
                    case EStreamMode.APPEND:
                    case EStreamMode.READ_AND_WRITE:
                        return fileStream;

                    default:
                        return null;
                }
            }
        }

        public bool StreamOpen { get; private set; }

        #region Flush

        public bool FlushAutomatically { get => flushAutomatically; }

        public void Flush()
        {
            if (!StreamOpen)
                return;

            if (fileStream != null)
            {
                fileStream.Flush(true);
            }
        }

        public async Task FlushAsync(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!StreamOpen)
                return;

            if (fileStream != null)
            {
                await fileStream.FlushAsync();
            }
        }

        #endregion


        #region Seek

        public long Position
        {
            get
            {
                if (!StreamOpen)
                    return -1;

                if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
                    && fileStream != null)
                {
                    return fileStream.Position;
                }

                return -1;
            }
        }

        public bool CanSeek
        {
            get
            {
                if (!StreamOpen)
                    return false;

                if ((CurrentMode == EStreamMode.READ ||CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
                    && fileStream != null)
                {
                    return fileStream.CanSeek;
                }

                return false;
            }
        }

        public bool Seek(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.Current);

                return true;
            }

            position = -1;

            return false;
        }

        public bool SeekFromStart(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.Begin);

                return true;
            }

            position = -1;

            return false;
        }

        public bool SeekFromFinish(
            long offset,
            out long position)
        {
            if (!StreamOpen)
            {
                position = -1;

                return false;
            }

            if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
                && fileStream != null)
            {
                position = fileStream.Seek(
                    offset,
                    SeekOrigin.End);

                return true;
            }

            position = -1;

            return false;
        }

        #endregion

        #endregion

        #region IStrategyWithState

        public bool SupportsSimultaneousReadAndWrite { get => true; }

        public void InitializeRead()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenReadStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = EStreamMode.READ;
        }

        public void FinalizeRead()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != EStreamMode.READ)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = EStreamMode.NONE;
        }

        public void InitializeWrite()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenWriteStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = EStreamMode.WRITE;
        }

        public void FinalizeWrite()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != EStreamMode.WRITE)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = EStreamMode.NONE;
        }

        public void InitializeAppend()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenAppendStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = EStreamMode.APPEND;
        }

        public void FinalizeAppend()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != EStreamMode.APPEND)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = EStreamMode.NONE;
        }

        public void InitializeReadAndWrite()
        {
            if (StreamOpen)
                return;

            StreamOpen = OpenReadWriteStream(
                out fileStream);

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO OPEN STREAM: {FullPath}"));

            CurrentMode = EStreamMode.READ_AND_WRITE;
        }

        public void FinalizeReadAndWrite()
        {
            if (!StreamOpen)
                return;

            if (CurrentMode != EStreamMode.READ_AND_WRITE)
                return;

            if (fileStream != null)
                CloseStream(
                    fileStream);

            fileStream = default(FileStream);

            StreamOpen = false;

            CurrentMode = EStreamMode.NONE;
        }

        #endregion

        #region IBlockSerializationStrategy

        #region Read

        public bool BlockRead<TValue>(
            int blockOffset,
            int blockSize,
            out TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = fileStream.Read(
                result,
                blockOffset,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            value = result.CastFromTo<byte[], TValue>();

            return true;
        }

        public bool BlockRead(
            Type valueType,
            int blockOffset,
            int blockSize,
            out object value)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = fileStream.Read(
                result,
                blockOffset,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            value = result;

            return true;
        }

        #endregion

        #region Write

        public bool BlockWrite<TValue>(
            TValue value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            fileStream.Write(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                Flush();

            return true;
        }

        public bool BlockWrite(
            Type valueType,
            object value,
            int blockOffset,
            int blockSize)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            fileStream.Write(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                Flush();

            return true;
        }

        #endregion

        #endregion

        #region IAsyncBlockSerializationStrategy

        #region Read

        public async Task<(bool, TValue)> BlockReadAsync<TValue>(
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = await fileStream.ReadAsync(
                result,
                blockOffset,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            TValue value = result.CastFromTo<byte[], TValue>();

            return (true, value);
        }

        public async Task<(bool, object)> BlockReadAsync(
            Type valueType,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.READ);

            // Read the source file into a byte array.
            byte[] result = new byte[blockSize];

            int resultLength = await fileStream.ReadAsync(
                result,
                blockOffset,
                blockSize);

            if (resultLength != blockSize)
            {
                byte[] resultTrimmed = new byte[resultLength];

                Array.Copy(
                    result,
                    resultTrimmed,
                    resultLength);

                result = resultTrimmed;
            }

            object value = result;

            return (true, value);
        }

        #endregion

        #region Write

        public async Task<bool> BlockWriteAsync<TValue>(
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue),
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<TValue, byte[]>();

            await fileStream.WriteAsync(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        public async Task<bool> BlockWriteAsync(
            Type valueType,
            object value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType,
                EStreamMode.WRITE);

            byte[] contents = value.CastFromTo<object, byte[]>();

            await fileStream.WriteAsync(
                contents,
                blockOffset,
                blockSize);

            if (flushAutomatically)
                await FlushAsync(
                    asyncContext);

            return true;
        }

        #endregion

        #endregion

        #region IStrategyWithFilter

        public bool AllowsType<TValue>()
        {
            return typeof(TValue) == typeof(byte[]);
        }

        public bool AllowsType(
            Type valueType)
        {
            return valueType == typeof(byte[]);
        }

        #endregion

        private void AssertStrategyIsValid(
            Type valueType,
            EStreamMode preferredMode)
        {
            if (valueType != typeof(byte[]))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: {valueType.Name}"));

            if (!StreamOpen)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"STREAM NOT OPEN: {FullPath}"));

            if (CurrentMode != preferredMode && CurrentMode != EStreamMode.READ_AND_WRITE)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID STREAM MODE: {CurrentMode}"));

            if (fileStream == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FILE STREAM IS NULL: {FullPath}"));
        }

        private bool OpenReadStream(
            out FileStream dataStream)
        {
            dataStream = default(FileStream);

            if (!IOHelpers.FileExists(
                FullPath,
                logger,
                GetType()))
            {
                return false;
            }

            dataStream = new FileStream(
                FullPath,
                FileMode.Open,
                FileAccess.Read);

            return true;
        }

        private bool OpenWriteStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Create,
                FileAccess.Write);

            return true;
        }

        private bool OpenAppendStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            //Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-fileStream
            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Append,
                FileAccess.Write);

            return true;
        }

        private bool OpenReadWriteStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            dataStream = new FileStream(
                FullPath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);

            return true;
        }

        private void CloseStream(FileStream dataStream)
        {
            dataStream.Close();
        }
    }
}