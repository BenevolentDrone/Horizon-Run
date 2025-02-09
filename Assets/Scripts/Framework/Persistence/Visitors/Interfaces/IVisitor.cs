using System;

namespace HereticalSolutions.Persistence
{
	public interface IVisitor
	{
		bool CanVisit<TVisitable>();

		bool CanVisit(
			Type visitableType);

		Type GetDTOType<TVisitable>();

		Type GetDTOType(
			Type visitableType);
	}
}