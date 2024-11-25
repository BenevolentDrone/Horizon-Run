namespace HereticalSolutions.Persistence
{
    public interface ILoadVisitor<TDTO>
        : IConcreteVisitor
    {
        bool Load(
            TDTO DTO,
            out IVisitable visitable);
    }
}