# INonAllocSubscribable

Represents an interface for a non-allocating subscribable with multiple arguments. Arguments are passed as an array of objects. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version see [`ISubscribableMultipleArgs`](ISubscribableMultipleArgs.md).

## Methods

Method | Description
--- | ---
`void Subscribe(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableMultipleArgs> subscription)` | Subscribes a subscription handler to the non-allocating subscribable
`void Unsubscribe(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableMultipleArgs> subscription)` | Unsubscribes a subscription handler from the non-allocating subscribable
`IEnumerable<INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableMultipleArgs>> AllSubscriptions { get; }` | Gets all the subscriptions of the non-allocating subscribable

## Using INonAllocSubscribable

### List all subscriptions

```csharp
INonAllocSubscribable foo;

var allSubscriptions = foo.AllSubscriptions;
```

### Subscribe a subscription

```csharp
INonAllocSubscribable foo;

void Bar(object[] arguments);

//Create a subscription with multiple arguments
INonAllocSubscription subscription = DelegatesFactory.BuildSubscriptionMultipleArgs(
    Bar,
    loggerResolver)

foo.Subscribe(
	(INonAllocSubscriptionHandler<
        INonAllocSubscribable,
        IInvokableMultipleArgs>)
		subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribable foo;

INonAllocSubscription subscription;

foo.Unsubscribe(
	(INonAllocSubscriptionHandler<
        INonAllocSubscribable,
        IInvokableMultipleArgs>)
		subscription);
```

## Creating INonAllocSubscribable

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a non alloc subscribable with multiple arguments
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocBroadcasterMultipleArgs(loggerResolver);
```

## Implementing INonAllocSubscribable

```csharp
#region INonAllocSubscribable

public void Subscribe(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableMultipleArgs>
		subscription)
{
	if (!subscription.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	var subscriptionState = (INonAllocSubscriptionState<IInvokableMultipleArgs>)subscription;

	subscriptionElement.Value = (INonAllocSubscription)subscriptionState;

	subscription.Activate(this, subscriptionElement);
}

public void Unsubscribe(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableMultipleArgs>
		subscription)
{
	if (!subscription.ValidateTermination(this))
		return;

	var poolElement = ((INonAllocSubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscription.Terminate();
}

IEnumerable<
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableMultipleArgs>>
		INonAllocSubscribable.AllSubscriptions
{
	get
	{
		var allSubscriptions = new INonAllocSubscriptionHandler<
			INonAllocSubscribable,
			IInvokableMultipleArgs>
			[subscriptionsAsIndexable.Count];

		for (int i = 0; i < allSubscriptions.Length; i++)
			allSubscriptions[i] = (INonAllocSubscriptionHandler<
				INonAllocSubscribable,
				IInvokableMultipleArgs>)
				subscriptionsAsIndexable[i].Value;

		return allSubscriptions;
	}
}

#endregion
```
