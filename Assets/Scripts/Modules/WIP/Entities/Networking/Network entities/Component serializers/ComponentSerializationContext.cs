using System;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class ComponentSerializationContext
    {
        public INetworkEntityRepository<Guid> NetworkEntityRepository;

        public ILoggerResolver LoggerResolver;
    }
}