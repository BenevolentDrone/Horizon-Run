using System.Threading;

namespace HereticalSolutions.Collections.Managed
{
	public class AtomicElementState
	{
		private int state;
	
		public AtomicElementState(
			EBufferElementState initialState)
		{
			state = (int)initialState;
		}
	
		public EBufferElementState Value
		{
			get => (EBufferElementState)Volatile.Read(
				ref state);

			set => Interlocked.Exchange(
				ref state,
				(int)value);
		}
	
		public bool CompareExchange(
			EBufferElementState newValue,
			EBufferElementState comparand)
		{
			return Interlocked.CompareExchange(
				ref state,
				(int)newValue,
				(int)comparand) == (int)comparand;
		}
	}
}