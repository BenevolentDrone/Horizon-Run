using System.Collections.Generic;

namespace HereticalSolutions.Delegates
{
	public interface INonAllocSubscribable
	{
		bool Subscribe(
			INonAllocSubscription subscription);

		bool Unsubscribe(
			INonAllocSubscription subscription);

		IEnumerable<INonAllocSubscription> AllSubscriptions { get; }

		void UnsubscribeAll();
	}
}