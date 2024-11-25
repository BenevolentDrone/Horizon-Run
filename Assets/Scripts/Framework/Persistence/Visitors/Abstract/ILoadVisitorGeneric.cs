namespace HereticalSolutions.Persistence
{
    public interface ILoadVisitorGeneric
    {
        bool Load<TConcreteVisitable, TDTO>(
            TDTO DTO,
            out TConcreteVisitable visitable);
    }
}