using System;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public class ComponentSerializationContext
    {
        public INetworkEntityRepository<Guid> NetworkEntityRepository;

        public ILoggerResolver LoggerResolver;
    }
}