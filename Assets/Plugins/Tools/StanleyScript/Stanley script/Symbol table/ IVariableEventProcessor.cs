using System;

namespace HereticalSolutions.StanleyScript
{
	public interface IVariableEventProcessor
	{
		void ProcessVariableAddedEvent(
			IStanleyVariable variable);

		void ProcessVariableRemovedEvent(
			IStanleyVariable variable);
	}
}