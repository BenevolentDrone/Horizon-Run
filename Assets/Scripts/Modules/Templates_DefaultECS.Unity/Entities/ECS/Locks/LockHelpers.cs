using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public static class LockHelpers
    {
        public static void ChainAfter(
            Entity lockEntity,
            Entity targetLockEntity,
            ILogger logger)
        {
            ref var lockComponent = ref lockEntity.Get<LockComponent>();
            
            ref var targetLockComponent = ref targetLockEntity.Get<LockComponent>();
			

            if (targetLockComponent.NextLink.IsAlive)
            {
                ref var nextLinkLockComponent = ref targetLockComponent.NextLink.Get<LockComponent>();
                
                if (nextLinkLockComponent.PreviousLink != targetLockEntity)
                    logger?.LogError(
                        $"[LockHelpers] THE PREVIOUS LINK OF THE NEXT LINK OF {targetLockEntity} IS NOT {targetLockEntity}");

                nextLinkLockComponent.PreviousLink = lockEntity;
            }


            lockComponent.NextLink = targetLockComponent.NextLink;

            lockComponent.PreviousLink = targetLockEntity;
            

            targetLockComponent.NextLink = lockEntity;
        }

        public static LockComponent Unchain(
            Entity lockEntity,
            ILogger logger)
        {
            ref var lockComponent = ref lockEntity.Get<LockComponent>();
            

            var previousLink = lockComponent.PreviousLink;

            if (previousLink.IsAlive)
            {
                ref var previousLinkLockComponent = ref previousLink.Get<LockComponent>();
                
                if (previousLinkLockComponent.NextLink != lockEntity)
                    logger?.LogError(
                        $"[LockHelpers] THE NEXT LINK OF THE PREVIOUS LINK OF {lockEntity} IS NOT {lockEntity}");

                previousLinkLockComponent.NextLink = lockComponent.NextLink;
            }


            var nextLink = lockComponent.NextLink;

            if (nextLink.IsAlive)
            {
                ref var nextLinkLockComponent = ref nextLink.Get<LockComponent>();
                
                if (nextLinkLockComponent.PreviousLink != lockEntity)
                    logger?.LogError(
                        $"[LockHelpers] THE PREVIOUS LINK OF THE NEXT LINK OF {lockEntity} IS NOT {lockEntity}");

                nextLinkLockComponent.PreviousLink = lockComponent.PreviousLink;
            }

            
            lockComponent.NextLink = default;

            lockComponent.PreviousLink = default;

            return lockComponent;
        }
    }
}