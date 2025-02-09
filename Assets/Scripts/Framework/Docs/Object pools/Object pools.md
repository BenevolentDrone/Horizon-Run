# Object Pools Framework Documentation

## Overview

The Object Pools framework provides a robust solution for object pooling in Unity. It improves performance by reusing objects rather than creating and destroying them repeatedly. This framework is divided into several subcategories, each addressing specific use cases.

## Folder Structure

The framework is organized as follows:

- **Allocation callbacks**: Contains callbacks related to object allocation, used for pre- and post-allocation hooks.
- **Arguments**: Provides strongly-typed pool argument support for custom object initialization when retrieving objects from pools.
- **Decorator pools**: Implements the decorator pattern to add additional functionality (like logging, monitoring, etc.) to base pool implementations.
- **Factories**: Offers factory classes for creating pool instances and managing configuration.
- **Managed pools**: Contains advanced pool implementations that handle object lifecycle and resource management automatically.
- **Regular pools**: Provides basic pooling functionalities (e.g., recycling objects) with simple interfaces such as IPool<T>.

## Key Interfaces and Components

### Regular Pools

- **IPool<T>**

  ```csharp
  namespace HereticalSolutions.Pools
  {
      public interface IPool<T>
      {
          T Pop();
          T Pop(IPoolPopArgument[] args);
          void Push(T instance);
      }
  }
  ```

  Defines the fundamental operations:
  - `Pop()`: Retrieves an object from the pool.
  - `Pop(args)`: Retrieves an object with initialization parameters.
  - `Push()`: Returns an object to the pool.

### Managed Pools

Managed Pools automate lifecycle management, ensuring that objects are properly reinitialized and cleaned up, minimizing memory leaks and resource mismanagement.

### Decorator Pools

Decorator Pools wrap around base pool implementations to add functionality such as logging, monitoring, or validation without modifying the underlying pool logic.

### Pool Factories

Factories are used to simplify the creation and configuration of pool instances. They provide a centralized approach to managing pools across the application.

### Allocation Callbacks

This component handles events related to the allocation and deallocation of pool objects, allowing custom actions during lifecycle events.

### Pool Arguments

Custom pool argument classes facilitate passing initialization parameters to pooled objects, providing flexibility and type safety.

## Usage Example

Below is an example of how to use a Regular Pool:

```csharp
// Creating a pool instance (implementation dependent)
IPool<GameObject> pool = new RegularGameObjectPool();

// Retrieving an object without extra arguments
GameObject obj = pool.Pop();

// Retrieving an object with arguments
IPoolPopArgument[] args = new IPoolPopArgument[] {
    new PositionArgument(Vector3.zero),
    new RotationArgument(Quaternion.identity)
};
GameObject objWithArgs = pool.Pop(args);

// Returning an object to the pool
pool.Push(obj);
```

## Best Practices

- **Object Reuse**: Always return objects to the pool after usage to minimize garbage collection overhead.
- **Initialization**: Use pool arguments to initialize objects with the required state upon retrieval.
- **Monitoring**: Utilize decorator pools for production systems to monitor pool usage and detect issues like leaks or over-allocation.
- **Lifecycle Management**: Prefer Managed Pools when object cleanup and disposal are critical.

## Contributing

When contributing to or extending this framework, please:

1. Follow existing design patterns (Factory, Decorator, etc.)
2. Write unit tests to cover any new functionality
3. Update documentation as necessary
4. Ensure thread safety and performance are maintained

## Future Improvements

- Enhanced logging and telemetry via additional decorator layers
- Dynamic resizing and more complex pool growth strategies
- Extended support for multi-threaded scenarios
- Deeper integration with Unity's job system for high-performance pooling
