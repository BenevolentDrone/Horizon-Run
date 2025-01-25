using System;

namespace HereticalSolutions.Delegates
{
	public class BroadcasterInvocationContext<TValue>
	{
		public Action<TValue> Delegate;
	}
}