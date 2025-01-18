namespace HereticalSolutions.Collections.Managed
{
	public interface IConcurrentCircularBuffer<TValue>
	{
		bool TryProduce(
			TValue item,
			out AtomicElementState state);

		bool TryConsume(
			out TValue value);

		int Capacity { get; }

		int Count { get; }

		int Head { get; }

		int Tail { get; }

		AtomicElementState GetElementState(
			int index);

		TValue GetValue(
			int index);
	}
}