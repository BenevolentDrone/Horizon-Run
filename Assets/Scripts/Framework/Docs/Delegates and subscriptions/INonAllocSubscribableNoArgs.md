# INonAllocSubscribable

Represents an interface for a non-allocating subscribable with no arguments. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version, see [`ISubscribableNoArgs`](ISubscribableNoArgs.md)

## Methods

Method | Description
--- | ---
`void Subscribe(INonAllocSubscription subscription)` | Subscribes to the event with the given subscription
`void Unsubscribe(INonAllocSubscription subscription)` | Unsubscribes from the event with the given subscription

## Using INonAllocSubscribable

### Subscribe a subscription

```csharp
INonAllocSubscribable foo;

void Bar(object[] arguments);

//Create a subscription with multiple arguments
INonAllocSubscription subscription = DelegatesFactory.BuildSubscriptionMultipleArgs(
    Bar,
    loggerResolver)

foo.Subscribe(subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribable foo;

INonAllocSubscription subscription;

foo.Unsubscribe(subscription);
```

## Creating INonAllocSubscribable

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a subscribable with no arguments
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocPinger(loggerResolver);
```

## Implementing INonAllocSubscribable

```csharp
#region INonAllocSubscribable

public void Subscribe(INonAllocSubscription subscription)
{
	var subscriptionHandler = (INonAllocSubscriptionContext<>)subscription;

	if (!subscriptionHandler.ValidateActivation(this))
		return;

	var subscriptionElement = subscriptionsPool.Pop();

	subscriptionElement.Value = subscription;

	subscriptionHandler.Activate(this, subscriptionElement);
}

public void Unsubscribe(INonAllocSubscription subscription)
{
	var subscriptionHandler = (INonAllocSubscriptionContext<>)subscription;

	if (!subscriptionHandler.ValidateTermination(this))
		return;

	var poolElement = ((INonAllocSubscriptionState<IInvokableNoArgs>)subscription).PoolElement;

	poolElement.Value = null;

	subscriptionsPool.Push(poolElement);

	subscriptionHandler.Terminate();
}

public void Unsubscribe(IPoolElement<INonAllocSubscription> subscription)
{
	subscription.Value = null;

	subscriptionsPool.Push(subscription);
}

#endregion
```
