using System;

using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [Component("Identity")]
    [EntityIDComponent]
    public struct GUIDComponent
    {
        public Guid GUID;
    }
}