using System;

namespace HereticalSolutions.Persistence
{
    public interface IVisitable
    {
        bool AcceptSave<TDTO>(
            ISaveVisitorGeneric visitor,
            out TDTO DTO);
        
        bool AcceptPopulate<TDTO>(
            ILoadVisitorGeneric visitor,
            TDTO DTO);
    }
}