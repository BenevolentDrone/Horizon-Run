using System;

namespace HereticalSolutions.Delegates
{
	public class NonAllocBroadcasterInvocationContext
	{
		public INonAllocSubscription[] Subscriptions;

		public int Count;
	}
}