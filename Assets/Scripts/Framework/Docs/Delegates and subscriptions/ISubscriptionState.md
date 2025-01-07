# INonAllocSubscriptionState\<TInvokable\>

Represents the state of a subscription. `TInvokable` should be inherited from `IInvokable` interface family

## Methods

Method | Description
--- | ---
`TInvokable Invokable { get; }` | Gets the invokable object associated with the subscription state
`IPoolElement<INonAllocSubscription> PoolElement { get; }` | Gets the pool element associated with the subscription state

## Using INonAllocSubscriptionState\<TInvokable\>

### Invoke the invokable

```csharp
INonAllocSubscription subscription;

var subscriptionState = (INonAllocSubscriptionState<IInvokableNoArgs>)subscription;

subscriptionState.Invokable.Invoke();
```

### Access the pool element

```csharp
INonAllocSubscription subscription;

var poolElement = ((INonAllocSubscriptionState<IInvokableNoArgs>)subscription).PoolElement;
```

## Creating INonAllocSubscriptionState\<TInvokable\>

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

void Foo();

//Create a subscription with no arguments
INonAllocSubscriptionState<IInvokableNoArgs>
	bar = DelegatesFactory.BuildSubscriptionNoArgs(
		Foo,
		loggerResolver);

void Foo(T value);

//Create a subscription with a single generic argument
INonAllocSubscriptionState<IInvokableSingleArgGeneric<T>>
	bar = DelegatesFactory.BuildSubscriptionSingleArgGeneric<T>(
		Foo,
		loggerResolver);

void Foo(object[] arguments);

//Create a subscription with multiple arguments
INonAllocSubscriptionState<IInvokableMultipleArgs>
	bar = DelegatesFactory.BuildSubscriptionMultipleArgs(
    	Foo,
    	loggerResolver)

```

## Implementing INonAllocSubscriptionState\<TInvokable\>

```csharp
private IInvokableNoArgs invokable;

private IPoolElement<INonAllocSubscription> poolElement;

#region INonAllocSubscriptionState

IInvokableNoArgs INonAllocSubscriptionState<IInvokableNoArgs>.Invokable =>
	(IInvokableNoArgs)invokable;

IPoolElement<INonAllocSubscription> INonAllocSubscriptionState<IInvokableNoArgs>.PoolElement => poolElement;

#endregion
```
