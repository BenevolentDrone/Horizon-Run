# Address system design considerations

## Path

* Primary source of asset naming and its location in the memory project hierarchy
	* For instance, "Assets/Levels/Level1" is a path to a Level1 asset

### Collision detection and resolving

* Path manager should check paths against allocated ones and throw errors if collisions occur
	* To avoid collisions for assets in the editor, paths should be generated as relative to project root folder
	* To avoid collisions in resource page files, paths should contain prefixes

### Path splitting

* Paths can be spilt by default delimiter ('/')
	* For instance, "Assets/Levels/Level1" is split into "Assets", "Levels" and "Level1"

### Symlinks and wildcards

* Paths may include '.', '..', '*' and '?' wildcard characters
* Paths may guide to other paths by using simlinks

## Guid

* Each path should contain Guid to be stored, retrieved and compared without string path

### Collision detection and resolving

* Path manager should check guids against allocated ones and generate new ones if necessary

## File versioning

### Overwrites

* Files sho uld be able to overwrite each other upon load, effectively replacing resources (assets, levels, etc.)
	* For instance, just like in WADs, you can load up a resource page and the E1M1 from the new page replaces E1M1 from the old page

### Variants

* The same file may have multiple versions with different assets referred by the same path/guid
	* For instance, the same muzzle flash VFX may have 3-4 different versions and the desired one is chosen with a dice roll

## Domains

* The path and the guid are relevant in particular domain only
	* For instance, "Effects/MuzzleFlashes" path is valid in VFX pool but is useless elsewhere

## Resource page files

* Files are serialized in binary files of a particular extension that can be loaded and unloaded in runtime. These page files are essentially WADs

### File format

* The resource page files consist of a header, a compressed B+ tree of inodes and a byte arrayof data
* The inodes contain file descriptors, metadata and address of the data in the byte array

### Peeking

* The resource page file can be scanned for inodes to map resources to resource pages. The pages containing the same file are either merged into an array if the file is a variant or the latest is selected if the file gets overwritten

### RAM presentation

* Once a resource from inode is requested, it returns either a byte array or a MemoryStream containing the file data

### Compression

* Each resource page file is compressed with LZ4 algorithm to save disk space and load faster

## Runtime resource management

* Runtime resource manager contains instances of resources needed for runtime or editor
* The resources can be loaded, created or deleted in tunrime on demand
* The resource manager makes use of asset importers and import post processors to convert byte arrays or MemoryStreams to instances of concrete types (materials, textures, levels etc.)

### Working with resource file pages

* Runtime resource manager does not load resource pages on its own. HOWEVER, once the certain resource that is not instantiated yet is requested, it may inquire on resource pages that contain the resource, load them, load the resource then unload them
	* Need to figure out how to do this in batches so resource pages are not loaded and unloaded for each resource