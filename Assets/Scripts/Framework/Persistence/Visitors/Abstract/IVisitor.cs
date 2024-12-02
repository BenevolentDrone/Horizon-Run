using System;

namespace HereticalSolutions.Persistence
{
	public interface IVisitor
	{
		#region Can visit

		bool CanVisit<TVisitable>();

		bool CanVisit(
			Type visitableType);

		#endregion
	}
}