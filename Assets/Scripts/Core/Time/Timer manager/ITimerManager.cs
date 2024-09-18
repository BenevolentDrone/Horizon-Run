namespace HereticalSolutions.Time
{
	public interface ITimerManager
	{
		string ID { get; }

		bool CreateTimer(
			out ushort timerHandle,
			out IRuntimeTimer timer);
		
		bool GetOrCreateSharedTimer(
			string timerID,
			float expectedDuration,
			out ushort timerHandle,
			out IRuntimeTimer timer,
			out bool newInstance);

		bool TryGetTimer(
			ushort timerHandle,
			out IRuntimeTimer timer);

		bool TryDestroyTimer(
			ushort timerHandle);
	}
}