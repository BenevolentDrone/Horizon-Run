# [Work in progress]

---

# Entity manager

## TL;DR

- Entity manager has two major responsibilities:
	- It manages the lifetime, creation and destruction of entities in different entity worlds (`IEntityManager`)
 	- It stores the repository of entity world controllers and provides an interface for accessing them (`IContainsEntityWorlds`)
- Entity manager provides the ability to create entities that either live in one particular world (`world-local entities`) or the ones living in multiple worlds (`entities with a register`)
- Entity manager takes care of the entire process of creating, maintaining and disposing of entities, including the initialization and deinitialization logic
- Entity manager allows to modify existing `entities with a register` by adding, removing and modifying their representations in different worlds

## Spawning entities

### Entity manager

- To create a `world-local entity` the entity manager retrieves the corresponding `World controller` by a string ID argument and calls its `SpawnEntity` method
- To create an `entity with a register` the entity manager first allocates a unique ID for the entity that would be attached to it in each world it's present in
- Then it creates a `registry entity` in the `Registry world` that contains references to the entity in each world it's present in
- After that it calls the `SpawnEntity` method of each `World controller` that the entity should be present in and provides it with the ID that was allocated for the entity. Each `World controller` then creates a `world-local entity` with the provided ID and feeds the `registry entity` with a reference to it
- Finally, the `registry entity` together with its ID is added to the registry entity repository. Then the allocated ID is returned to the caller as a handle to the entity

### World controller

- To create an entity in the corresponding world the `World controller` makes use of the GoF `Prototype` pattern
- Each world controller (except the `Event world` controller) pairs its entity world with a `Prototype world` that contains `prototype entities` - a preallocated set of entities that can are used as a templates for new entities
- The world controller first retrieves a `prototype entity` from the `prototypes repository` by a string ID argument
- Then it performs a component-wise copy of the `prototype entity` to a freshly allocated entity in the entity world it manages
- If provided with an ID, the world controller attaches an `identifier component` to the entity so that it can be identified in any world it's present in by the same ID
- At this point the entity is ready to be returned, unless it requires additional initialization logic that cannot be covered by specifying its components and their values. When you spawn a soldier entity you need to give it a weapon, tell on which side it is, teleport them to spawn position, etc.
- For this purpose the `World controller` stores so called `Initialization systems` that run over each entity created and assemble it like a car on the assembly lane. Each initialization system should take care of a single aspect of the entity initialization process - a system responsible for installing wheels should not be attaching a propeller or painting the vehicle's hull
- Another case is when the entity should be spawned with some `context` - for instance, a tower placeholder was placed on the scene and the `World controller` has to spawn the tower entity exactly where the placeholder is placed, at the angle it's placed, with the banners it's sporting etc. For this purpose the `World controller` stores so called `Resolve systems` that run over each entity created before the `Initialization systems` and provide them with the context it's given
- A separate set of systems called `Deinitialization systems` are responsible for disassembly and cleanup of entities when they're no longer needed but cannot be deleted straight away without additional logic. If you're destroying a tree then you cannot have its branches and leaves floating in the air or having the forest to count for an inexisting tree in its tree counter variable
