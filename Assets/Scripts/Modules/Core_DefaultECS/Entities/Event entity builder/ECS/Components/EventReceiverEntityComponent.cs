namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[NetworkEventComponent]
	public struct EventReceiverEntityComponent<TEntityID>
	{
		public TEntityID ReceiverID;
	}
}