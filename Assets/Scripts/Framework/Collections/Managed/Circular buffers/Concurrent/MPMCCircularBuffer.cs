using System;
using System.Threading;

namespace HereticalSolutions.Collections.Managed
{
	public class MPMCCircularBuffer<T>
		: IConcurrentCircularBuffer<T>
	{
		private readonly T[] buffer;

		private readonly AtomicElementState[] states;

		private readonly int capacity;
	

		private int head; // Index for consumers

		private int tail; // Index for producers
	

		public MPMCCircularBuffer(
			T[] buffer,
			AtomicElementState[] states)
		{
			this.buffer = buffer;

			this.states = states;

			capacity = buffer.Length;

			head = 0;

			tail = 0;
		}

		public bool TryProduce(
			T item,
			out AtomicElementState state)
		{
			state = null;

			int currentTail;

			while (true)
			{
				currentTail = tail;

				int index = currentTail % capacity;

				// Allow producing into slots in AwaitingProduction or Consumed states

				if (states[index].CompareExchange(
					EBufferElementState.PRODUCING,
					EBufferElementState.CONSUMED))
				{
					buffer[index] = item;

					state = states[index];

					state.Value = EBufferElementState.PRODUCED;

					Interlocked.CompareExchange(
						ref tail,
						currentTail + 1,
						currentTail);

					return true;
				}

				if (!SpinWait.SpinUntil(
					() => states[index].Value == EBufferElementState.CONSUMED,
						10))
				{
					return false; // Cannot produce, all slots full or contention
				}
			}
		}

		public bool TryConsume(
			out T item)
		{
			item = default;

			while (true)
			{
				int currentHead = head;
				int index = currentHead % capacity;

				if (states[index].CompareExchange(
					EBufferElementState.CONSUMING,
					EBufferElementState.PRODUCED))
				{
					item = buffer[index];

					states[index].Value = EBufferElementState.CONSUMED;

					Interlocked.CompareExchange(
						ref head,
						currentHead + 1,
						currentHead);

					return true;
				}

				if (!SpinWait.SpinUntil(
					() => states[index].Value == EBufferElementState.PRODUCED,
					10))
				{
					return false; // Cannot consume, buffer is empty or contention
				}
			}
		}

		public int Capacity => capacity;
	
		public int Count
		{
			get
			{
				int produced = tail;

				int consumed = head;
				
				return produced - consumed;
			}
		}

		public int Head => Volatile.Read(ref head);

		public int Tail => Volatile.Read(ref tail);

		public AtomicElementState GetElementState(
			int index)
		{
			if (index < 0 || index >= capacity)
				throw new ArgumentOutOfRangeException(
					nameof(index),
					$"INVALID INDEX: {index}");

			return states[index];
		}

		public T GetValue(int index)
		{
			if (index < 0 || index >= capacity)
				throw new ArgumentOutOfRangeException(
					nameof(index),
					$"INVALID INDEX: {index}");

			return buffer[index];
		}
	}
}