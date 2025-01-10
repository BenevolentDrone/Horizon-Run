using System;
using HereticalSolutions.StateMachines;

namespace HereticalSolutions.Networking
{
	public interface IHandshakeProtocol
		: IStateMachine<IHandshakeStep>
	{
		IHandshakeStep CurrentStep { get; }

		IHandshakeStep GetStep<TStep>()
			where TStep : IHandshakeStep;

		IHandshakeStep GetStep(
			Type stepType);
	}
}