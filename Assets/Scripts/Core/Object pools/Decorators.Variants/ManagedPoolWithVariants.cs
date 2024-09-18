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
        private readonly IRepository<int, VariantContainer<T>> innerPoolsRepository;

        private readonly IRandomGenerator randomGenerator;

        private readonly ILogger logger;

        public ManagedPoolWithVariants(
            IRepository<int, VariantContainer<T>> innerPoolsRepository,
            IRandomGenerator randomGenerator,
            ILogger logger = null)
        {
            this.innerPoolsRepository = innerPoolsRepository;

            this.randomGenerator = randomGenerator;

            this.logger = logger;
        }

        #region IManagedPool

        public IPoolElementFacade<T> Pop()
        {
            #region Validation

            if (!innerPoolsRepository.TryGet(
                    0,
                    out var currentVariant))
                throw new Exception(
                    logger.TryFormatException<ManagedPoolWithVariants<T>>(
                        "NO VARIANTS PRESENT"));

            #endregion
            
            #region Random variant

            var hitDice = randomGenerator.Random(0, 1f);
            
            int index = 0;

            while (currentVariant.Chance < hitDice)
            {
                hitDice -= currentVariant.Chance;
                index++;

                if (!innerPoolsRepository.TryGet(
                        index,
                        out currentVariant))
                    throw new Exception(
                        logger.TryFormatException<ManagedPoolWithVariants<T>>(
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
                if (!innerPoolsRepository.TryGet(
                        arg.Variant,
                        out var variant))
                    throw new Exception(
                        logger.TryFormatException<ManagedPoolWithVariants<T>>(
                            $"INVALID VARIANT {{ {arg.Variant} }}"));

                var concreteResult = variant.Pool.Pop(args);

                return concreteResult;
            }

            #endregion

            #region Validation

            if (!innerPoolsRepository.TryGet(
                    0,
                    out var currentVariant))
                throw new Exception(
                    logger.TryFormatException<ManagedPoolWithVariants<T>>(
                        "NO VARIANTS PRESENT"));

            #endregion

            #region Random variant

            var hitDice = randomGenerator.Random(0, 1f);
            
            int index = 0;

            while (currentVariant.Chance < hitDice)
            {
                hitDice -= currentVariant.Chance;
                index++;

                if (!innerPoolsRepository.TryGet(index, out currentVariant))
                    throw new Exception(
                        logger.TryFormatException<ManagedPoolWithVariants<T>>(
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
                    logger.TryFormatException<ManagedPoolWithVariants<T>>(
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
			
            if (!instanceWithMetadata.Metadata.Has<IContainsVariant>())
                throw new Exception(
                    logger.TryFormatException<ManagedPoolWithVariants<T>>(
                        "POOL ELEMENT FACADE HAS NO VARIANT METADATA"));
            
            var metadata = instanceWithMetadata.Metadata.Get<IContainsVariant>();
            
            int variant = metadata.Variant;

            if (!innerPoolsRepository.TryGet(
                    variant,
                    out var poolByVariant))
                throw new Exception(
                    logger.TryFormatException<ManagedPoolWithVariants<T>>(
                        $"INVALID VARIANT {{variant}}"));

            poolByVariant.Pool.Push(instance);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPoolsRepository is ICleanuppable)
                (innerPoolsRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPoolsRepository is IDisposable)
                (innerPoolsRepository as IDisposable).Dispose();
        }

        #endregion
        
        public void AddVariant(
            float chance,
            IManagedPool<T> poolByVariant)
        {
            int index = 0;

            while (innerPoolsRepository.Has(index))
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
            innerPoolsRepository.Add(
                index,
                new VariantContainer<T>
                {
                    Chance = chance,

                    Pool = poolByVariant
                });
        }
    }
}