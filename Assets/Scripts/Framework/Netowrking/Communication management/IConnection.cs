using System;
using HereticalSolutions.Metadata;
using HereticalSolutions.StateMachines;

namespace HereticalSolutions.Networking
{
	//TODO: EACH CONNECTION HAS A
	//1. CONNECTION DATA - PEER ID, URI, PORT ETC
	//2. FSM OF THE CURRENT CONNECTION STATE
	//3. METADATA TO STORE ALL THE VARIOUS DATA EACH PARTICULAR CONNECTION NEEDS (LIKE SLOT ID IF THERE IS A SLOT SYSTEM)
	public interface IConnection<TPeerData>
	{
		TPeerData PeerData { get; }

		IHandshakeProtocol Protocol { get; }

		IWeaklyTypedMetadata Metadata { get; }
	}
}