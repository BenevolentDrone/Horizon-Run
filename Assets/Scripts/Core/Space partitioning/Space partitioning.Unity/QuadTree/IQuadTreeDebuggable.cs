using System.Collections.Generic;

namespace HereticalSolutions.SpacePartitioning
{
    public interface IQuadTreeDebuggable
    {
        void Dump(List<Bounds2D> boundsList, List<Bounds2D> entityBounds);
    }
}