using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.RandomGeneration;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
    public class ManagedPoolWithVariants<T>
        : IManagedPool<T>,
          ICleanuppable,
          IDisposable
    {
        private readonly IRepository<int, VariantContainer<T>> innerPoolRepository;

        private readonly IRandomGenerator randomGenerator;

        private readonly ILogger logger;

        public ManagedPoolWithVariants(
            IRepository<int, VariantContainer<T>> innerPoolRepository,
            IRandomGenerator randomGenerator,
            ILogger logger = null)
        {
            this.innerPoolRepository = innerPoolRepository;

            this.randomGenerator = randomGenerator;

            this.logger = logger;
        }

        #region IManagedPool

        public IPoolElementFacade<T> Pop()
        {
            #region Validation

            if (!innerPoolRepository.TryGet(
                    0,
                    out var currentVariant))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "NO VARIANTS PRESENT"));

            #endregion
            
            #region Random variant

            var hitDice = randomGenerator.Random(0, 1f);
            
            int index = 0;

            while (currentVariant.Chance < hitDice)
            {
                hitDice -= currentVariant.Chance;
                index++;

                if (!innerPoolRepository.TryGet(
                        index,
                        out currentVariant))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            "INVALID VARIANT CHANCES"));
            }

            var result = currentVariant.Pool.Pop();

            return result;

            #endregion
        }

        public IPoolElementFacade<T> Pop(
            IPoolPopArgument[] args)
        {
            #region Variant from argument

            if (args != null 
                && args.TryGetArgument<VariantArgument>(out var arg))
            {
                if (!innerPoolRepository.TryGet(
                        arg.Variant,
                        out var variant))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID VARIANT {{ {arg.Variant} }}"));

                var concreteResult = variant.Pool.Pop(args);

                return concreteResult;
            }

            #endregion

            #region Validation

            if (!innerPoolRepository.TryGet(
                    0,
                    out var currentVariant))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "NO VARIANTS PRESENT"));

            #endregion

            #region Random variant

            var hitDice = randomGenerator.Random(0, 1f);
            
            int index = 0;

            while (currentVariant.Chance < hitDice)
            {
                hitDice -= currentVariant.Chance;
                index++;

                if (!innerPoolRepository.TryGet(index, out currentVariant))
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            "INVALID VARIANT CHANCES"));
            }

            var result = currentVariant.Pool.Pop(args);

            return result;

            #endregion
        }

        public void Push(
            IPoolElementFacade<T> instance)
        {
            IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
                instance as IPoolElementFacadeWithMetadata<T>;

            if (instanceWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
			
            if (!instanceWithMetadata.Metadata.Has<IContainsVariant>())
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO VARIANT METADATA"));
            
            var metadata = instanceWithMetadata.Metadata.Get<IContainsVariant>();
            
            int variant = metadata.Variant;

            if (!innerPoolRepository.TryGet(
                    variant,
                    out var poolByVariant))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VARIANT {{variant}}"));

            poolByVariant.Pool.Push(instance);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPoolRepository is ICleanuppable)
                (innerPoolRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPoolRepository is IDisposable)
                (innerPoolRepository as IDisposable).Dispose();
        }

        #endregion
        
        public void AddVariant(
            float chance,
            IManagedPool<T> poolByVariant)
        {
            int index = 0;

            while (innerPoolRepository.Has(index))
            {
                index++;
            }
            
            AddVariant(
                index,
                chance,
                poolByVariant);
        }
        
        public void AddVariant(
            int index,
            float chance,
            IManagedPool<T> poolByVariant)
        {
            innerPoolRepository.Add(
                index,
                new VariantContainer<T>
                {
                    Chance = chance,

                    Pool = poolByVariant
                });
        }
    }
}