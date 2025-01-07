using System;

namespace HereticalSolutions.Delegates
{
	public class BroadcasterGenericInvocationContext<TValue>
	{
		public Action<TValue> Delegate;
	}
}