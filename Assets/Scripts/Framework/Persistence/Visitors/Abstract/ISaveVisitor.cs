namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitor<TDTO>
        : IConcreteVisitor
    {
        bool Save(
            IVisitable visitable,
            out TDTO DTO);
    }
}