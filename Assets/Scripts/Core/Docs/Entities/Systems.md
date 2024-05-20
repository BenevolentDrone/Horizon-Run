# [Work in progress]

---

# Systems

## World controller systems

### Initialization systems

- The purpose of initialization systems is to introduce changes to entities when they're created in the assembly line fashion. For instance, when you spawn a soldier entity you need to give it a weapon, tell on which side it is, teleport them to spawn position, etc.
- The naming of the system should be `{Domain}InitializationSystem`. For instance, `SpellcasterInitializationSystem`
- The system should be inherited from `ISystem<TEntity>` or `IDefaultEcsEntityInitializationSystem` for DefaultECS
- The system should first check whether the entity it's processing has the relevant components, otherwise it should return. For instance, ```if (!entity.Has<SpellcasterComponent>()) return;```
- The system should be listed in the initialization systems list for the corresponding world controller in the installer / composition root

### Resolve systems

- The purpose of resolve systems is to introduce context dependent changes to entities when they're created. For instance, a tower placeholder was placed on the scene and the `World controller` has to spawn the tower entity exactly where the placeholder is placed, at the angle it's placed, with the banners it's sporting etc.
- The naming of the system should be `{ResolveThisWay}ResolveSystem`. For instance, `CopyPositionFromGameObjectResolveSystem`
- The system should be inherited from `ISystem<TEntity>` or `IDefaultEcsEntityInitializationSystem` for DefaultECS
- The system should first check whether the entity it's processing has the resolve component specific to the world it's being processed in. For instance, `if (!entity.Has<ResolveSimulationComponent>()) return;`
- Then the system should check whether the entity it's processing has the relevant components, otherwise it should return. For instance, ```if (!entity.Has<PositionComponent>()) return;```
- The system should be listed in the resolve systems list for the corresponding world controller in the installer / composition root

### Deinitialization systems

//TODO

## Event world systems

### Event systems

- The purpose of event systems is to process event entities in a reactive manner. The `Event world` should be considered as a data stream, event entities should be considered as events and event systems should be considered as event listeners with their filters being the operators that select the events they're interested in
- The naming of the event system should be `{DoThis}On{EventName}EventSystem`. For instance, `UpdateSpellCooldownOnSpellAcquiredEventSystem`
- The system should be inherited from either `ISystem<TDelta>` or `AEntitySetSystem<TDelta>` for DefaultECS. The delta itself matters not because event systems should not care for the time it took for a frame to update - their logic is purely reactive. The reason the delta should be included is to allow event systems to be updated in conjunction with regular update systems (before or after them)
- The event system's filter should make use of general purpose event components like `EventTargetWorldLocalEntityComponent`, `EventPositionComponent` or `EventTimeComponent` together with event particular components like `SpellAcquiredEventComponent` etc.
- The event system's filter should have `.Without<EventProcessedComponent>()` operator for the following reasons:
	-  Some events may be "early returned" by predecessing event systems that process the same event. If you press the elevator button that is already glowing then obviously nothing should happen
	- Some events may arrive from different sources, like events received from client/server. There is no guarantee when it happens and in which order but those events still need to be processed only once. You don't want to spawn more enemies than the server told you to
	- Some events may be produced by predecessing event systems while the systems that process them may be earlier in the event system chain so those events need to be carried over to the next event system update cycle
	- Some events need to be processed additionally (for instance, sent to server/client or written to a log file) before being disposed of
	- Some events may be required to persist for a while
- The same event entity may (and in some cases should) be processed by multiple systems in a chain. For instance, when you give spellcaster a new spell you should first check whether he has another spell in the designated slot and get rid of it, then update the spell's data on mana cost, cooldowns and stuff and finally update the UI to reflect the changes
- Some systems should act like "gatekeepers" for the chains and prevent further processing of the event if asserts are not met. For instance, if you're trying to give a spell to a spellcaster that doesn't have a spell slot then you should not proceed with the chain. To do this the systems should add an `EventProcessedComponent` to the entity if the assert is not met and return
- The system should be listed in the event systems list, preferably uniting the entire chain for one particular event under the same `SequentialSystem<TDelta>` with a trailing `EventProcessedSystem<TEventComponent, TDelta>` generic system that automatically flags event entities as processed after the chain has completed its operation

## Simulation world systems

//TODO

## View world systems

### Presenter systems

- Presenter systems should update `presenter components` in a system manner - i.e. the same logic applies for each entity in the set
- The `presenter component`'s responsibilities are to maintain the references to model entities it works with and act like a bridge between them and the view components they're paired with
- That means that `presenter systems` are responsible for retrieving the data from the model entities and providing it to the `view components` and for retrieving the user input from the `view components` and providing it to the model entities
- Model logic neither knows nor cares about the view logic including view and presenter components, systems and entities. The same applies to view components and systems - they do not care what model entities to work with. This means that keeping track of model entity references, validating inputs from views and other sanitation chores should be the part of `presenter system` logic
- The system should be inherited from either `ISystem<TDelta>` or `AEntitySetSystem<TDelta>` for DefaultECS
- The system should be listed in the late update systems list. The presenter systems should be listed between input and visualization view systems

### View systems

- View systems should update `view components` in a system manner - i.e. the same logic applies for each entity in the set
- The `view component`'s responsibilities are to provide the user with a visual representation of the entity and to handle the user input
- The views should not contain references to model parts and should not contain any logic that is not directly related to the view itself. The data it needs to display should be provided by the `presenter systems` and the user input should be stored and polled by the `presenter systems` who then update the `model components` accordingly
- Any logic that can be done in a system should be done in a system unless the API restrictions or object-oriented / event-oriented nature of the view component requires it to be done in the view itself
- The system should be inherited from either `ISystem<TDelta>` or `AEntitySetSystem<TDelta>` for DefaultECS
- The system should be listed in the late update systems list. The view systems that receive user input should be listed in the early update systems list, followed by the presenter systems and then bu view systems that visualize the changes made by the presenter systems