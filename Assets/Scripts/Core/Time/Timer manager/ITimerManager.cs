namespace HereticalSolutions.Time
{
	public interface ITimerManager
	{
		string ID { get; }

		bool CreateTimer(
			out ushort timerHandle,
			out IRuntimeTimer timer);

		bool TryGetTimer(
			ushort timerHandle,
			out IRuntimeTimer timer);

		bool TryDestroyTimer(
			ushort timerHandle);
	}
}