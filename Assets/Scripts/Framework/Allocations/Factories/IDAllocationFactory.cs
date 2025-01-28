using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Allocations.Factories
{
    public static class IDAllocationFactory
    {
        public static ByteIDAllocationController BuildByteIDAllocationController(
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<ByteIDAllocationController>();

            return new ByteIDAllocationController(
                new Queue<byte>(),
                logger);
        }

        public static UShortIDAllocationController BuildUShortIDAllocationController(
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<ByteIDAllocationController>();

            return new UShortIDAllocationController(
                new Queue<ushort>(),
                logger);
        }

        public static GUIDAllocationController BuildGUIDAllocationController()
        {
            return new GUIDAllocationController(
                new HashSet<Guid>());
        }
    }
}