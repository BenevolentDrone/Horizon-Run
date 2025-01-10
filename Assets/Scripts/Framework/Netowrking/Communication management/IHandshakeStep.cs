using HereticalSolutions.StateMachines;

namespace HereticalSolutions.Networking
{
	public interface IHandshakeStep
		: IState
	{
		bool WillProcessPacket();

		void ProcessPacket();
	}
}