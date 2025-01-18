using System;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class PoolWithAddressBuilder<T>
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly ILogger logger;

        public PoolWithAddressBuilder(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        private AddressTreeNode<T> root;

        public void Initialize(
            ManagedPoolWithAddress<T> pool)
        {
            root = new AddressTreeNode<T>
            {
                CurrentAddress = string.Empty,

                FullAddress = string.Empty,

                CurrentAddressHash = -1,

                FullAddressHashes = new int[0],

                Level = 0,
                
                PoolWithAddress = pool,

                Pool = null
            };
        }

        public void Parse(
            string address,
            IManagedPool<T> pool)
        {
            string[] addressParts = address.SplitAddressBySeparator();

            int[] addressHashes = address.AddressToHashes();

            AddressTreeNode<T> currentNode = root;

            for (int i = 0; i < addressHashes.Length; i++)
            {
                bool traversed = TraverseToChildNode(
                    ref currentNode,
                    addressHashes[i]);

                if (!traversed)
                    CreateAndTraverse(
                        addressParts,
                        addressHashes,
                        ref currentNode,
                        addressHashes[i]);
            }

            currentNode.Pool = pool;
        }

        private bool TraverseToChildNode(
            ref AddressTreeNode<T> currentNode,
            int targetAddressHash)
        {
            for (int i = 0; i < currentNode.Children.Count; i++)
            {
                if (currentNode.Children[i].CurrentAddressHash == targetAddressHash)
                {
                    currentNode = currentNode.Children[i];

                    return true;
                }
            }

            return false;
        }

        private void CreateAndTraverse(
            string[] addressParts,
            int[] addressHashes,
            ref AddressTreeNode<T> currentNode,
            int targetAddressHash)
        {
            int currentNodeLevel = currentNode.Level;

            AddressTreeNode<T> child = new AddressTreeNode<T>
            {
                CurrentAddress = addressParts.AddressAtDepth(currentNodeLevel),

                //TODO: check if works correctly
                FullAddress = addressParts.PartialAddress(currentNodeLevel),

                CurrentAddressHash = targetAddressHash,

                FullAddressHashes = addressHashes.PartialAddressHashes(currentNodeLevel),

                Level = currentNodeLevel + 1,
                
                PoolWithAddress = null,

                Pool = null
            };

            currentNode.Children.Add(child);

            currentNode = child;
        }

        public void Validate()
        {
            if (root == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "BUILDER NOT INITIALIZED"));

            ValidateNode(root);
        }

        private void ValidateNode(
            AddressTreeNode<T> node)
        {
            IRepository<int, IManagedPool<T>> repository = null;

            if (node.PoolWithAddress != null)
            {
                repository = node.PoolWithAddress.InnerPoolRepository;
            }
            else
            {
                repository =
                    RepositoryFactory.BuildDictionaryRepository<int, IManagedPool<T>>();
            }

            foreach (var child in node.Children)
            {
                ValidateNode(child);
                
                if (!repository.Has(child.CurrentAddressHash))
                {
                    repository.Add(
                        child.CurrentAddressHash,
                        child.PoolWithAddress);
                }
            }

            if (node.Pool != null
                && !repository.Has(0))
            {
                repository.Add(
                    0,
                    node.Pool);
            }

            if (node.PoolWithAddress == null)
                node.PoolWithAddress = AddressDecoratorPoolFactory.BuildPoolWithAddress(
                    repository,
                    node.Level,
                    null,
                    loggerResolver);
        }
    }
}