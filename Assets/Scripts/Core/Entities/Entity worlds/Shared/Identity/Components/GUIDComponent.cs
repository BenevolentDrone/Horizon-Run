using System;

namespace HereticalSolutions.Entities
{
    [Component("Identity")]
    [EntityIDComponent]
    public struct GUIDComponent
    {
        public Guid GUID;
    }
}