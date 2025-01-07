# INonAllocSubscribableSingleArgGeneric\<T\>

Represents a non-allocating subscribable interface with a single generic argument. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version, see [`ISubscribableSingleArgGeneric<T>`](ISubscribableSingleArgGeneric.md)

## Methods

Method | Description
--- | ---
`void Subscribe(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>> subscription)` | Subscribes a subscription handler to this subscribable
`void Unsubscribe(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>> subscription)` | Unsubscribes a subscription handler from this subscribable
`IEnumerable<INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>>> AllSubscriptions { get; }` | Gets all the subscriptions currently registered with this subscribable

## Using INonAllocSubscribableSingleArgGeneric\<T\>

### List all subscriptions

```csharp
INonAllocSubscribable foo;

var allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
INonAllocSubscribable foo;

void Bar(T argument);

//Create a subscription with a single generic argument
INonAllocSubscription subscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
	Bar,
	loggerResolver);

foo.Subscribe(
	(INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<TDelta>>)
		subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribable foo;

INonAllocSubscription subscription;

foo.Unsubscribe(
	(INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<TDelta>>)
		subscription);
```

## Creating INonAllocSubscribableSingleArgGeneric\<T\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a non alloc subscribable with a single generic argument
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocBroadcasterGeneric<T>(loggerResolver);
```

## Implementing INonAllocSubscribableSingleArgGeneric\<T\>

```csharp
#region INonAllocSubscribableSingleArgGeneric

public void Subscribe(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	if (!subscription.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	var subscriptionState = (INonAllocSubscriptionState<IInvokableSingleArgGeneric<T>>)subscription;

	subscriptionElement.Value = (INonAllocSubscription)subscriptionState;

	subscription.Activate(this, subscriptionElement);
}

public void Unsubscribe(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	if (!subscription.ValidateTermination(this))
		return;

	var poolElement = ((INonAllocSubscriptionState<IInvokableSingleArgGeneric<T>>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscription.Terminate();
}

IEnumerable<
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>>
		INonAllocSubscribable.AllSubscriptions
{
	get
	{
		var allSubscriptions = new INonAllocSubscriptionHandler<
			INonAllocSubscribable,
			IInvokableSingleArgGeneric<T>>
			[subscriptionsAsIndexable.Count];

		for (int i = 0; i < allSubscriptions.Length; i++)
			allSubscriptions[i] = (INonAllocSubscriptionHandler<
				INonAllocSubscribable,
				IInvokableSingleArgGeneric<T>>)
				subscriptionsAsIndexable[i].Value;

		return allSubscriptions;
	}
}

#endregion
```
