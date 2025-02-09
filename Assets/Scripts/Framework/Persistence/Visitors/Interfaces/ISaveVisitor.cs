using System;

namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitor
        : IVisitor
    {
        bool VisitSave<TVisitable>(
            ref object dto,
            TVisitable visitable);

        bool VisitSave(
            ref object dto,
            Type visitableType,
            object visitableObject);
    }
}