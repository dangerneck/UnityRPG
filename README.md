# Unity RPG Project

This is a WIP RPG game project for Unity written in C#. 

>I have not touched this since December 2014 so it is a little aged and sloppy but some ideas are fun and the basic design isn't too bad.

The task was to create the functionality required to have the following features:

- NPCs with time-based schedules and the ability to move in and out of active scenes
- Flexible tree-structured dialog system
- Sufficiently robust structure for items to contain other items, for items to be persistent, and for all of this data available to be serialized at any moment for saving and loading.

This has been achieved with the following objects:

- **Game Manager** - handles global issues like time, absent NPC schedules, active game state, saving and loading, changing scenes, texture loading
- **Player** (and related objects) - a group of objects handling input, movement, control focus, camera logic, inventory (special case of a container)
-  **NPC** - handles dialog, scheduling, pathfinding
- **Container** - handles interaction to display contents, state changes to move objects between the scene and container

**Unity method note:**
> The `Start()` method is called by unity once the object is initialized.
> The `Update()` and `FixedUpdate()` are called roughly once per frame by unity.
> The `OnGUI()` method is called on GUI draw calls.



## Game Manager

This is a persistant object (created at game start and not destroyed until program closes). Initially it loads required textures and initialises game state. Each frame it calls the `HandleTime()` method.

The `HandleTime()` method uses the unity `deltaTime` to update the game state's calendar and at a certain interval runs `CheckAllAbsentSchedules()` to ensure NPCs not currently in the scene will be following their schedules. 

The `CheckAllAbsentSchedules()` method loops over all known NPCs in the global game state that are not within the scene, and updates their state with the current schedule item. If that schedule item's location is within the current scene the NPC is instantiated and handles itself from then on.

To handle scene change and scene initialization the Game Manager uses `ChangeScene()` and the unity builtin `OnLevelWasLoaded()`.

`OnLevelWasLoaded()` is called by unity once a unity scene has started. The Game Manager will look at all game objects (containers and NPCs) hard-coded in the unity scene and enable them. It then instantiates the player, items and NPCs defined within the game state.

`ChangeScene()` will force any NPCs moving between schedule items to snap to finishing them, then get the game state ready for the new scene. Then it calls the unity `Application.LoadLevel(sceneId)` to change scenes - this in turn calls `OnLevelWasLoaded()` once unity has finished it's business.

Saving and loading is done using a Json Serializer (NewtonSoft.Json with a Vector3 serializer added). Saving serializes the current game state into JSON then write that to a text file. Loading reads this file and deserializes the JSON into the game object in memory.

## Player

The player is a group of objects required for handling input, camera and movement. There is **PlayerControl** and **CameraHideColliderManager** (related to HideOnAbovePlayer and HideOnCameraLine).

> **Note:** The architecture of the player group is flawed mainly in that concerns are not separated properly, so it is not immediately obvious what object will handle what. This requires work.

### PlayerControl

This object handles input for player movement, player inventory, the current control focus of the game and camera position and player associated GUI elements.

Control focus is a variable on the `PlayerControl` object containing the id of the object currently claiming the control focus. Objects handling controls will check in their own `FixedUpdate()` or `Update()` methods whether they have control focus and handle it there. Control focus is by default on the `PlayerControl` object. 

Control focus is passed around using the `ClaimControlFocus()` method on the `PlayerControl` object.

When intialized this will position the visible player object and place the camera such that it is looking at the visible player object. Each frame the input and associated movement is handled and calculated with `HandleMove()` and `HandleGeneralInput()` or `HandleInventoryInput()` if the control focus is on the `Inventory` object which is a container object in the player state model.

`HandleGeneralInput()` handles opening and closing the inventory and any special actions. The only special action currently present is the 'grab' command which will add an object underneath the player to the `Inventory` container and remove it from the current scene.

`HandleInventoryInput()` allows moving a pointer through the inventory container to drop an object, doing the reverse of the 'grab' command. It handles calling the method to close the inventory.

`HandleMove()` reads the unity `Input.GetAxis()` to move the player object in the associated directoin, and adds gravity and collision detection.

The unity method `OnGUI()` is called on GUI draw calls and handles drawing the inventory textures if required.

### CameraHideColliderManager

This is a script applied to objects which might obscure the view of the player in a 3/4 top-down view. It detects if the player's extended bounds is within it's bounds and if so it disables it's renderer objects to no longer be drawn.

### HideOnAbovePlayer

This script is used to make it possible for the player to move within a multi-floored building or multi-layered area while having visibility. A single logical level is wrapped in a container object, that container object has this `HideOnAbovePlayer` script applied to it. To check, each frame a ray is cast from the player directly up, and if this object intersects with that ray it has its renderers disabled.

### HideOnCameraLine

This is used in the same way as `HideOnAbovePlayer` but instead of projecting a ray from the player up it projects from the camera to the player and if the object intersects it has its renderers disabled. This would be used for walls within a building, or any object which can come between the player and the camera.

That covers the majority of the player objects.

## NPC

The NPC object handles its associated dialog data and logic, schedule data and logic, movement and state export.

On intialization this object will read it's state from the game manager (using its `Name` as the id). This includes the two-layered schedule heirarchy and the dialog state and data. It also initializes the `Seeker` script for pathfinding which I used a library for.

On `Update()` opening and closing dialog is handled, as well as the `CheckSchedule()` and `HandleMove()` calls.

`CheckSchedule()` will check if the schedule item must change, and then how to go about it. It may be a schedule item they teleport to (disappearing suddenly), an item within the scene which they will pathfind to, or an item in a different scene where they will pathfind to a defined `Exit` in the scene and remove themselves.

`HandleMove()` handles moving along a path provided by the pathfinding script, checking collisions and applying gravity. Once it is close enough to the goal the `OnScheduledPathComplete()` method is called. That handles syncing the state of the NPC with the game once the schedule item has commenced and removing the NPC from the scene if necessary.

`OnGUI()` handles the drawing of dialog related GUI.

### Schedule

There is a repeating 7-day `WeeklySchedule` and a `LifeTimeSchedule`. If there are collisions between the two the `LifeTimeSchedule` always wins. 

These are both `List<ScheduleItem>` variables on the NPC state model.

See `/Assets/Scripts/ScheduleItem.cs` for the model.

### Dialog

The unit of dialog is the DialogItem (see `Assets/Scripts/DialogItem.cs` for the model). 
The `RequisiteFlag` is checked so that dialog options can be hidden or displayed based on arbitrary logic. The `OnSelect` Action allows code to be run on the dialog item's selection, usually the setting of flags to alter game state or alter current dialog options for flow through the dialog tree. 

The `DialogItem` is pretty self explanatory. 
The `DialogState` model holds the text of the dialog and potentially associated data (currently only `EmotionalState` to allow for different portraits or animations to occur during dialog).

Each NPC has a `List<DialogItem>` to represent available dialog, and the game state holds all possible `DialogItem`s for NPCs.

## Container

The container handles its contents and its interaction with the player and input.

On intialization it reads its state from the global game state.

On `Update()` it checks to see if it is being clicked on by casting a ray from mouse click into the scene and checking for intersection. If intersection is found it claims control focus and allows the player to scroll through and take items from the container into the `Inventory` container.

`OnGUI()` draws the container GUI if it is opened.

The `Open()` method claims control focus and sets the internal `Opened` bool to true, and the `Close()` method writes the container state to the game object and gives control focus back to the player.

The `ContainerModel` handles its capacity for objects and it's contents (a `List<ContainedObjectModel>`.

The `ContainedObjectModel` is the state data of an `Item` and is transferrable between containers and scenes.


- - -

## Thanks for reading

If theres anything useful to you in this project take it, if you have any questions or suggestions, contact me!





