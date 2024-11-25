using System;

namespace HereticalSolutions.Persistence
{
	public interface IConcreteVisitor
	{
		Type DTOType { get; }
	}
}