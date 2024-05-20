namespace HereticalSolutions.Time
{
	public interface ITimerManager
	{
		string ID { get; }

		bool CreateTimer(
			out int timerID,
			out IRuntimeTimer timer);

		bool TryGetTimer(
			int timerID,
			out IRuntimeTimer timer);

		bool TryDestroyTimer(
			int timerID);
	}
}