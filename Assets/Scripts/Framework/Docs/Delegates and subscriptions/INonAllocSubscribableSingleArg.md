# INonAllocSubscribableSingleArg

Represents an interface for a non-allocating subscribable with a single argument. Inherits from [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version see [`ISubscribableSingleArg`](ISubscribableSingleArg.md).

## Methods

Method | Description
--- | ---
`void Subscribe<T>(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>> subscription)` | Subscribes to the event with a specific value type and a generic subscription handler
`void Subscribe(Type valueType, INonAllocSubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)` | Subscribes to the event with a specific value type and a non-generic subscription handler
`void Unsubscribe<T>(INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>> subscription)` | Unsubscribes from the event with a specific value type and a generic subscription handler
`void Unsubscribe(Type valueType, INonAllocSubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)` | Unsubscribes from the event with a specific value type and a non-generic subscription handler
`IEnumerable<INonAllocSubscriptionHandler<INonAllocSubscribable, IInvokableSingleArgGeneric<T>>> GetAllSubscriptions<T>()` | Gets all subscriptions for a specific value type with a generic subscription handler
`IEnumerable<INonAllocSubscription> GetAllSubscriptions(Type valueType)` | Gets all subscriptions for a specific value type with a non-generic subscription handler
`IEnumerable<INonAllocSubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>> AllSubscriptions { get; }` | Gets all subscriptions with a non-generic subscription handler

## Using INonAllocSubscribableSingleArg

### List all subscriptions

```csharp
INonAllocSubscribableSingleArg foo;

var allSubscriptions = foo.AllSubscriptions;
```

### List all subscriptions of type T

```csharp
INonAllocSubscribableSingleArg foo;

var allSubscriptions = foo.GetAllSubscriptions<T>();

var allSubscriptions = foo.GetAllSubscriptions(typeof(T));
```

### Subscribe a subscription

```csharp
INonAllocSubscribableSingleArg foo;

void Bar(T argument);

//Create a subscription with a single generic argument
var subscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
	Bar,
	loggerResolver);

foo.Subscribe<T>(
	(INonAllocSubscriptionHandler<
		INonAllocSubscribable, IInvokableSingleArgGeneric<T>>)
		subscription);

foo.Subscribe(
	typeof(T),
	(INonAllocSubscriptionHandler<
		INonAllocSubscribableSingleArg,
		IInvokableSingleArg)
		subscription);
```

### Unsubscribe a subscription

```csharp
INonAllocSubscribableSingleArg foo;

INonAllocSubscription subscription;

foo.Unsubscribe<T>(
	(INonAllocSubscriptionHandler<
		INonAllocSubscribable, IInvokableSingleArgGeneric<T>>)
		subscription);

foo.Unsubscribe(
	typeof(T),
	(INonAllocSubscriptionHandler<
		INonAllocSubscribableSingleArg,
		IInvokableSingleArg>)
		subscription);
```

## Creating INonAllocSubscribableSingleArg

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//An object repository is needed for the broadcaster to select the concrete broadcaster for a given argument type
IReadOnlyInstanceRepository repository;

//Create a non alloc subscribable with a single argument
INonAllocSubscribableSingleArg foo = DelegatesFactory.BuildNonAllocBroadcasterWithRepository(
    repository,
    loggerResolver);

//Create a non alloc subscribable with a single generic argument
INonAllocSubscribableSingleArg foo = DelegatesFactory.BuildNonAllocBroadcasterGeneric<T>(loggerResolver);
```

## Implementing INonAllocSubscribableSingleArg

```csharp
#region INonAllocSubscribableSingleArg

public void Subscribe<T>(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	var valueType = typeof(T);

	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribable)broadcasterObject;

	broadcaster.Subscribe(subscription);
}

public void Subscribe(
	Type valueType,
	INonAllocSubscriptionHandler<
		INonAllocSubscribableSingleArg,
		IInvokableSingleArg>
		subscription)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

	broadcaster.Subscribe(valueType, subscription);
}

public void Unsubscribe<T>(
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>
		subscription)
{
	var valueType = typeof(T);

	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribable)broadcasterObject;

	broadcaster.Unsubscribe(subscription);
}

public void Unsubscribe(
	Type valueType,
	INonAllocSubscriptionHandler<
		INonAllocSubscribableSingleArg,
		IInvokableSingleArg>
		subscription)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

	broadcaster.Unsubscribe(valueType, subscription);
}

IEnumerable<
	INonAllocSubscriptionHandler<
		INonAllocSubscribable,
		IInvokableSingleArgGeneric<T>>>
		INonAllocSubscribableSingleArg.GetAllSubscriptions<T>()
{
	var valueType = typeof(T);

	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribable)broadcasterObject;

	return broadcaster.AllSubscriptions;
}

public IEnumerable<INonAllocSubscription> GetAllSubscriptions(Type valueType)
{
	if (!broadcasterRepository.TryGet(
			valueType,
			out object broadcasterObject))
		throw new Exception(
			$"INVALID VALUE TYPE: \"{valueType.Name}\"");

	var broadcaster = (INonAllocSubscribable)broadcasterObject;

	return broadcaster.AllSubscriptions;
}

IEnumerable<
	INonAllocSubscriptionHandler<
		INonAllocSubscribableSingleArg,
		IInvokableSingleArg>>
		INonAllocSubscribableSingleArg.AllSubscriptions
{
	get
	{
		var result = new List<INonAllocSubscriptionHandler<
			INonAllocSubscribableSingleArg,
			IInvokableSingleArg>>();

		foreach (var key in broadcasterRepository.Keys)
		{
			var broadcasterObject = broadcasterRepository.Get(key);

			var broadcaster = (INonAllocSubscribableSingleArg)broadcasterObject;

			result.AddRange(broadcaster.AllSubscriptions);
		}

		return result;
	}
}

#endregion
```
