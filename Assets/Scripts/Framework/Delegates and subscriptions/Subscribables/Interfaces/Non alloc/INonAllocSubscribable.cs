using System.Collections.Generic;

namespace HereticalSolutions.Delegates
{
	public interface INonAllocSubscribable
	{
		bool Subscribe(
			ISubscription subscription);

		bool Unsubscribe(
			ISubscription subscription);

		IEnumerable<ISubscription> AllSubscriptions { get; }

		void UnsubscribeAll();
	}
}