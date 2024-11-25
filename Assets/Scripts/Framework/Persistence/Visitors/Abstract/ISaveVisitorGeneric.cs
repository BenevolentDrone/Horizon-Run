namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitorGeneric
    {
        bool Save<TConcreteVisitable, TDTO>(
            TConcreteVisitable visitable,
            out TDTO DTO);
    }
}