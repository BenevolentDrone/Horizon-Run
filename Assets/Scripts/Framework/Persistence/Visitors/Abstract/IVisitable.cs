namespace HereticalSolutions.Persistence
{
    public interface IVisitable
    {
        bool AcceptSave(
            ISaveVisitor visitor,
            ref object dto);

        bool AcceptPopulate(
            IPopulateVisitor visitor,
            object dto);
    }
}