namespace HereticalSolutions.Entities
{
	[NetworkEventComponent]
	public struct EventReceiverEntityComponent<TEntityID>
	{
		public TEntityID ReceiverID;
	}
}