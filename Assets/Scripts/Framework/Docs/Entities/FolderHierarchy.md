# [Work in progress]

---

# Folder hierarchy

## TL;DR

- Split the business logic by domains (or categories) first and create corresponding folders for them. When the new tech requirements for game features arrive the biggest issue is not how to add something new but rather **how to remove something old without breaking everything else**. The more you split the logic the easier it is to remove or replace it. Removing inventory from the game should not affect the player's movement logic, for instance (unless you have components / systems that utilize both - more on that later)
- Create a folder for each world the entities containing the components of your domain should belong to. Controls, sprites, meshes etc. belong to the `View world`, while AI, physics, pathfinding etc. belong to the `Simulation world`, etc.
- In each folder create subfolders dedicated to particular subdomain features. For instance, in the `Navigation`'s `Simulation world` folder you may have subfolders for `Agent`, `Surface`, `Teleport` etc. This should look like `Navigation/Simulation world/Agent`, `Navigation/Simulation world/Surface`, `Navigation/Simulation world/Teleport`, `Navigation/View world/Debug` etc.
- For each subfolder create `Components` and `Systems` folders. Entities are described in settings files that are uniform so there's no need for `Entities` folder. That way the `RigidbodyComponent` would be located in `Physics/Simulation world/Rigidbodies/Components` folder and `ContactResolverSystem` would be placed in `Physics/Simulation world/Contact resolving/Systems` folder
- All the components and systems that are shared between different subdomains should be placed in `Components` and `Systems` folders that are located in the corresponding world folder. That way the `PositionComponent` would be located in `Position/Simulation world/Components` folder
- All the components and systems that are shared between different worlds should be placed in `Components` and `Systems` folders that are located in `Shared` folder located on the same level as other world folders. That way the `PersistentIDComponent` would be located in `Persistence/Shared/Components` folder
- Remember that presenters and controls belong to the `View world`
- If the logic belongs to multiple domains then
	1. Decide which domain is the 'holder' and which is/are 'dependencies'. Reduced walking speed from heavy items in the inventory system belongs to the 'inventory' domain while 'locomotion' is a dependency because:
		- If you remove the inventory logic alltogether the locomotion logic should still work
		- If there are different types of locomotion provided by other domains (like walking, driving, flying) then removing the inventory logic should not affect them
		- On the contrary if you remove 'locomotion' logic alltogether then you'd be left with the inventory but no means to move at all
		- Chosing the primary domain is as easy as chosing which would require the **least** amount of work to remove and affect the **least** amount of other domains
	2. Add the subfolder to the corresponding world's folder that bears the dependency's name (i.e. `Inventory/Simulation world/Locomotion/`) and fill the `Components` and `Systems` folders with the logic that is shared between the domains