# 2D Platformer Tutorial for Unity 2017

This is very much a WIP.  I'm trying to make a tutorial helpful to a range of experience levels.  Please let me know if you have any suggestions - I'm creating this live at [twitch.tv/HardlyDifficult](https://twitch.tv/HardlyDifficult) or email nick@HardlyDifficult.com

[TODO some intro]

[Demo of the game](https://hardlydifficult.com/Kong/index.html) we are creating.

Target audience: we expect you know some coding and can follow along with C# examples.  New concepts are introduced along the way to help beginners and intermediate developers.  No experience with Unity or other game engines is required.

[TODO image size how to manage this well????]
todo review consistent use of GameObject


# 1) Create a new 2D project

Get Unity, start a 2D project and save before we get started.

<details><summary>Where to get Unity</summary>

 - [Download Unity](https://unity3d.com/), the free Personal edition has everything you need. 
 - Select "2D" when creating a new project.
 - Enter a name/directory - the other options can be left at defaults.

<img src="https://i.imgur.com/T2iZrmK.png" width=50% />

<hr></details>
<details><summary>Why 2D?</summary>

Presenting the 2D vs 3D option when you create a new project suggests this is a significant choice.  It's not really... 2D just changes default settings on things like your camera.   Unity is a 3D engine, when creating 2D games your actually creating a 3D world where everything is very flat but the camera looks straight ahead and the only rotation in the world is around the z axis.  

<hr></details>





## Save scene

When you created a project, a default scene was created as well.  Save it as 'Level1', as that's where this tutorial begins.

<details><summary>How</summary>

 - File -> Save Scenes.
 - Create a "Scenes" directory, call it 'Level1'.

<hr></details>
<details><summary>What's a scene?</summary>

The Scene represents a collection of game objects and components configured for a game level or menu screen.  For this tutorial we are starting by creating part of Level 1.  Level 2, the menu, and other UI screens will be saved as separate scenes.  You can switch scenes via the SceneManager, and will cover this later in the tutorial. [TODO link to switch scene section]

<hr></details>




## Auto save script (optional)

Unity may crash.  I recommend adding an editor script which automatically saves everytime you hit play.

<details><summary>How</summary>

 - Right click in the Project Assets folder -> Create -> Folder and name it "Editor".
 - Create -> C# Script and name it "AutoSave.cs".
 - Paste in the source code below.
 
Unity APIs used:

- [InitializeOnLoad](https://docs.unity3d.com/ScriptReference/InitializeOnLoadAttribute.html) attribute enables the script
 - On [EditorApplication.playmodeStateChanged](https://docs.unity3d.com/ScriptReference/EditorApplication-playmodeStateChanged.html) events:
   - Save with [EditorSceneManager.SaveOpenScenes()](https://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.SaveOpenScenes.html)
   - You can do nothing if [EditorApplication.isPlaying](https://docs.unity3d.com/ScriptReference/EditorApplication-isPlaying.html), avoiding extra save calls

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Auto saves the scene and project everytime you click play.
/// 
/// This happens before the game runs so even if that run causes 
/// a crash, your work is safe.
/// </summary>
[InitializeOnLoad]
public class AutoSave
{
  /// <summary>
  /// Called automatically c/o InitializeOnLoad.  Registers for
  /// play mode events.
  /// </summary>
  static AutoSave()
  {
    EditorApplication.playmodeStateChanged 
      += OnPlaymodeStateChanged;
  }

  /// <summary>
  /// When the play mode changes, consider saving.
  /// </summary>
  static void OnPlaymodeStateChanged()
  {
    if(EditorApplication.isPlaying)
    { // If currently playing, don't save
      return;
    }

    // Save!  
    EditorSceneManager.SaveOpenScenes();
  }
}
```

<hr></details>
<details><summary>What about performance?</summary>

As an editor script, this logic is not included in the game you release.  Saving is incremental, so there is very little time wasted when there is nothing new to save.  Unless you're one of the lucky ones who never sees Unity crash, this script is absolutely worth the time tradeoff.


<hr></details>
<details><summary>Why is the folder name important?</summary>

Unity uses [special folder names](https://docs.unity3d.com/Manual/SpecialFolders.html) for various features.  

Everything under a folder named "Editor" is an editor script, including files in any subdirectories.  e.g. this script could be saved as Assets/Editor/AutoSave.cs or Assets/Code/Editor/Utils/AutoSave.cs  There are two special folder names that work anywhere in your folder hierarchy like this:
 - "[Editor](https://docs.unity3d.com/Manual/ExtendingTheEditor.html)": These scripts are only executed when in the Unity Editor (vs built into the game).
 - "[Resources](https://docs.unity3d.com/ScriptReference/Resources.html)": Assets bundled with the game, available for loading via code.  Note Unity [recommends using resources only for prototypes](https://unity3d.com/learn/tutorials/temas/best-practices/resources-folder) - the preferred solution is [AssetBundles](https://docs.unity3d.com/Manual/AssetBundlesIntro.html). 
 
The following are additional special folder names only considered when in the root Assets directory. e.g. Assets/Gizmos is a special directory but Assets/Code/Gizmos could hold anything.
 - "[Standard Assets](https://docs.unity3d.com/Manual/HOWTO-InstallStandardAssets.html)": These are optional assets and scripts available from Unity you can add to your project anytime.
 - "[Editor Default Resources](https://docs.unity3d.com/ScriptReference/EditorGUIUtility.Load.html)": A resources directory only available to Editor scripts so you can have assets appear in the editor for debugging without increasing the game's built size.
 - "[Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html)": Editor-only visualizations to aid level design and debugging.
 - "[Plugins](https://docs.unity3d.com/Manual/Plugins.html)": Dlls to include, typically from 3rd party libraries.
 - "[StreamingAssets](https://docs.unity3d.com/Manual/StreamingAssets.html)": Videos or other archives for your game to stream.

Be sure you do not use folders with these names anywhere in your project unless specifically for that Unity use case. e.g. Assets/Code/Editor/InGameMapEditor.cs may be intended to be part of the game but would be flagged as an Editor only script instead.

<hr></details>
<details><summary>What's a C# attribute?</summary>

Attributes in C# are metadata added to classes, fields, or methods that may be queried by other classes.  In this example InitializeOnLoad, a Unity specific attribute, is used to ensure the static constructor on our AutoSave class is called when the game begins.

There are many [standard C# attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/index) and [Unity specific attributes](http://www.tallior.com/unity-attributes/) that may be used.  Here are a few random examples:

```csharp
using UnityEngine;
using UnityEngine.Networking;

// Tells unity that this component only works
// if the game object also has a SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class MyClassName : MonoBehaviour
{
  // Tells unity this field can be modified
  // in the inspector
  [SerializeField]
  // Limits the values you can enter 
  // in the inspector
  [Range(1, 10)]
  int count;

  // Used for multiplayer games to sync 
  // method calls
  [ClientRpc]
  void MyMethod() { }
}
```


<hr></details>
<details><summary>What's a C# static constructor?</summary>

Every object in C# may include a static constructor, this applies to static and non-static classes.  A static constructor is guaranteed to be called once (and only once).  The constructor will run before the first object is instantiated, a field is accessed, or a method is called (i.e. it happens before you touch the class).  You never call the static constructor directly.

A static constructor is a private static method named the same as the class, with no parameters and no return type.

```csharp
public class MyClassName 
{
  static MyClassName() 
  {
    // This is executed once automatically, before we do 
    // anything else with MyClassName.
  }
}
```
<hr></details>
<details><summary>What's a C# static method?</summary>

A static method in C# is one that may be called without first instantiating an object for that class.  Static methods may only access static data in that class (static data is data which is shared across all objects).

```csharp
public class MyClassName
{
  static int callCount;
  public static void MyMethod()
  {
    callCount++; 
  }
}

public class AnotherClass
{
  public void Run()
  {
    // Call MyMethod without creating 
    // an object for MyClassName.
    MyClassName.MyMethod();
  }
}
```

<hr></details>
<details><summary>What's a C# delegate?</summary>

A delegate in C# is an object representing method(s) to call at a later time.  Events, Action, Func, and delegates are implemented as a 'multicast delegate'.  This means that any number of methods may subscribe to the same delegate.

We use += when registering for an event so not to overwrite any other subscribers.

```csharp
EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
```

If the owner of the delegate (in the example above that's EditorApplication) may outlive the subscriber you should unsubcribe from the event OnDestroy, or any time you are no longer interested in future updates.  We do this with -= to remove our method and leave the remaining methods in-tact (if there are any).

```csharp
EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
```

Events are a common use case.  You may have a GameManager tracking points include an event onPointsChange.  Other components/systems in the game, such as Achievements and the points UI, may subscribe to onPointsChange.  When a player earns points, a method in Achievements is then called which can consider awarding a high score achievement and a method in the points UI is called to refresh what the player sees on-screen.  This way those components only need to refresh when something changed as opposed to checking the current state each frame.

```csharp
using System;
using UnityEngine;

public static class GameManager
{
  public static event Action onPointsChange;
  static int _points;
  public static int points
  {
    get
    {
      return _points;
    }
    set
    {
      _points = value;
      if(onPointsChange != null)
      {
        onPointsChange();
      }
    }
  }
}

public class MyCustomComponent : MonoBehaviour
{
  protected void Awake()
  {
    GameManager.onPointsChange 
      += GameManager_onPointsChange; 
  }

  protected void OnDestroy()
  {
    GameManager.onPointsChange
      -= GameManager_onPointsChange;
  }

  void GameManager_onPointsChange()
  {
    // React to points changing
  }
}
```

<hr></details>


# 2) Create platforms 

Create platforms and lay them out for Level1.  After this section, the game should look something like this:

[TODO screenshot]



## Import art

Add a sprite sheet for the platform to the Asset directory.  We are using [Kenney.nl's Platformer Pack Redux](http://kenney.nl/assets/platformer-pack-redux) 'spritesheets/spritesheet_ground.png'.

<details><summary>How</summary>


 - Right click in the Project Assets directory -> Create Folder named 'Art'  (optional)
   - You can rename folders by selecting and pressing F2
 - Drag/drop the sprite sheet (or an entire folder of art) into the folder you just created
   - If you have a zip file, you may need to unzip to a temp directory before drag/drop will work.

<hr></details>
<details><summary>What's a sprite / sprite sheet?</summary>

A sprite is an image, used in 2D games and for UI.  They may represent an object, part of an object, or a frame of an entity's animation, etc.  

A sprite sheet is a single image file that contains multiple individual sprites.  The sheet may use these sprites to represent different frames for an animation or to hold a collection of various object types (as is the case here).

<hr></details>
<details><summary>Can I use my own art?</summary>

Of course, this tutorial only assumes that you are using sprites.  You can build your own sprite sheet or use individual sprites, but this tutorial is geared towards a 2D game and some things may not work out well if you try using 3D models instead.

<hr></details>





## Slice sprite sheet

Slice the sprite sheet in order to access each individual sprite within.

<details><summary>How</summary>

- Select the sprite sheet in the Project tab (Assets/Art/spritesheet_ground)
- In the Inspector, set Sprite Mode to Multiple
- Click Sprite Editor (apply changes when prompted)

<img src="http://i.imgur.com/duYuVMy.png" width=50% />

- Click the "Slice" menu item
  - Type: Grid By Cell Count
  - Column & Row: 8 & 16
- Click "Slice" button
- Click "Apply" and close the Sprite Editor

<img src="http://i.imgur.com/hA2cMfv.png" width=50% />

<hr></details>
<details><summary>What does slicing achieve?</summary>

Slicing is the process of defining each individual sprite in a sprite sheet.  Once sliced, you can access each sprite as if it were a unique asset.

<hr></details>
<details><summary>Could I use other slice method Types?</summary>

The goal is to slice the sprite sheet, any method your comfortable with is fine.  Options include:

 - "Automatic" which sometimes works (but does not for the spritesheet used in this example).
 - "Grid By Cell Size" allows you to enter the size of each sprite in the sheet directly, e.g. 128x128.
 - "Grid By Cell Count" allows you to simply count how many rows and columns you see in the sheet (vs maybe guess and checking the Cell Size).


<hr></details>
<details><summary>How do I know I've sliced correctly?</summary>

 - After you have sliced, white lines appear in the "Sprite Editor".  These lines show you how the sprite sheet is cut, boxing in each individual sprite.  Any whitespace as shown in this example is ignored (i.e. it does not generate blank sprites as well).

<img src="http://i.imgur.com/NawupLS.png" width=50% />

 - After closing the "Sprite Editor" and Applying changes you can expand the sprite sheet in Assets and see each sprite it created.

<img src="http://i.imgur.com/Qq0nn2B.png" width=50% />

<hr></details>




## Drag sprite into scene

Add a sprite to the scene representing the middle segment of a platform.  We are using 'spritesheet_ground/spritesheet_ground_72'.

Note: there may be visual artifacts which we will address below.

<details><summary>How</summary>

 - Click the arrow on the sprite sheet in your Assets/Art directory (this displays each individual sliced image).
 - Click and drag the platform sprite you want to use into the Hierarchy.  We are using 'spritesheet_ground_72'. This creates a GameObject with a SpriteRenderer component for that sprite.

<img src="http://i.imgur.com/kZC4i6d.png" width=50% />

<hr></details>
<details><summary>What's a GameObject?</summary>

Everything you see and interact with in a game is driven by game objects.  Typically a game object represents a single logical object in the world.  It may be composed of child game objects, each responsible for part of the display and/or behaviour.

A GameObject is something which appears in the game's "Hierarchy" tab.  Every GameObject has a transform.  It may also hold other GameObjects and it may includes various components.  


<hr></details>
<details><summary>What's a Transform?</summary>

A transform manages the GameObject's position, rotation and scale.  Every GameObject, including child GameObjects, have a transform.

Occasionally you will encounter a GameObject that has nothing rendered on screen.  In these cases the transform is often completely ignored but may not be removed.

<hr></details>
<details><summary>What's a Component?</summary>

A component is a set of logic (i.e. code) which may be added to a GameObject (or child GameObject) and is exposed in the Inspector.  A GameObject may have any number of components and those components may expose various properties to customize the behaviour for a specific object.  Unity has a number of components available out of the box, we will be using several Unity components in this tutorial and will be making many custom components as well.

<hr></details>
<details><summary>What's a SpriteRenderer?</summary>

SpriteRenderer is a Unity component which takes a sprite asset to render on screen.  Select the game object in the Hierarchy to view the component in the Inspector.  Here sereval options are available for modifying how the sprite is rendered.  For example:

 - Sprite: This is the sprite image to render.  It was populated automatically when you created the game object with drag/drop.
 - Color: White is the default, displaying the sprite as it was created by the artist.  Changing this color modifies the sprite's appearance.  You can also use the Alpha value here to make a sprite transparent.
 - Order in Layer: When multiple sprites are overlapping, this is used to determine which one is on top of the other.

<img src="http://i.imgur.com/4w3P1nx.png" width=50% />

<hr></details>



## Sprite Draw Mode: Tiled

In the scene, update the SpriteRenderer to Draw Mode: Tiled and adjust the width so we can begin to design the level's platforms.

Note: a warning will appear in the inspector and there may be visual artifacts which we will address below.

<details><summary>How</summary>

 - Select the 'spritesheet_ground_72' game object.
 - In the Inspector, under the SpriteRenderer component:
   - Change Draw Mode to Tiled 
   - An option for width appears, try increasing this to about 10 (but don't change height).

<img src="http://i.imgur.com/MIgzjdO.png" width=50% />

<hr></details>
<details>
<summary>Why not use Transform scale?</summary>

Using transform scale to change the width cause the sprite displayed to stretch.  We are using tiling so the sprite repeats instead.

<img src="http://i.imgur.com/ejbs3RK.png" width=50% />

<hr></details>



## Sprite Filter Mode: Point

Update the sprite sheet's import settings to use filter mode point, preventing some visual artifacts.

<details><summary>How</summary>

 - Select the sprite sheet in the "Project" tab (Assets/Art/spritesheet_ground)
 - In the "Inspector", set "Filter Mode" to "Point (no filter)"

<img src="http://i.imgur.com/B0nqf75.png" width=50% />

<hr></details>
<details><summary>Why use Point and not the default?</summary>

This (and most) sprite sheets have each object touching the one next to it.  Filter Mode Point prevents blending happening between one sprite and it's neighbor.  The blending that occurs with other modes besides Point may lead to random lines showing up on screen.  For example:

<img src="http://i.imgur.com/ZKqg5JP.png" width=50% />

<hr></details>


## Sprite Mesh Type: Full Rect

Update the sprite sheet's import settings to use mesh type full rect since we will be using tiling.

<details><summary>How</summary>

 - Select the sprite sheet in the "Project" tab (Assets/Art/spritesheet_ground)
 - In the "Inspector", set "Mesh Type" to "Full Rect"

<img src="http://i.imgur.com/Dhe3Nzt.png" width=50% />

<hr></details>

<details><summary>Why use Full Rect and not the default?</summary>

When using tiling on a sprite, Unity recommends updating the sprite sheet to use "Full Rect".  I don't have an example of issues that may arrise from using "Tight" instead, but here is the warning from Unity recommending "Full Rect":

<img src="http://i.imgur.com/e9jE83B.png" width=50% />

<hr></details>



## Disable Anti Aliasing

Disable Anti-Aliasing, preventing some visual artifacts.

<details><summary>How</summary>

 - Menu "Edit" -> "Project Settings" -> "Quality"
 - In the "Inspector" change "Anti Aliasing" to "Disabled"
 - Repeat this for each quality level supported
   - Click on the row to modify (e.g. "Ultra")
   - Update "Anti Aliasing" if needed

The currently highlighted 'Level' is what you are testing with ATM.  It will default to Ultra.  The green checkboxs represent the default quality level for different build types.  In this example I'm testing with Ultra, using Ultra by default for PC builds, and High by default for WebGL builds.  To avoid artifacts, I disable Anti Aliasing in every level and then switch back to Ultra.

<img src="http://i.imgur.com/omFI4DD.png" width=50% />

<hr></details>
<details><summary>What is Anti Aliasing and why disable it?</summary>

Anti Aliasing is a technique used to smooth jagged edges. 

<img src="https://qph.ec.quoracdn.net/main-qimg-10856ecbea4f439fb9fb751d41ff704a" width=50% />

When working with sprites, we often want control over images down to the pixel.  Anti-aliasing may lead to unexpected gaps or distortions.  Here is an example that appears when using tiling:

<img src="http://i.imgur.com/vY5YmVj.png" width=50% />

<hr></details>





## Set a 5:4 Aspect ratio

Change the aspect ratio to 5:4 as we want to simplify and always show the same amount of the world on screen.

<details><summary>How</summary>

 - On the "Game" tab, near the top, change "Free Aspect" to "5:4"

<img src="http://i.imgur.com/MTnZtu4.png" width=50% />

 - Switch back to the "Scene" tab.  The white box here represents the area that players will see.

<img src="http://i.imgur.com/eIq2LD2.png" width=50% />

<hr></details>
<details><summary>Why use a fixed aspect ratio?</summary>

We are building a game with a fixed display.  The camera is not going to move.  In order to design levels without requiring camera effects, we choose a specific aspect ratio to build for.  Different resolutions will scale the world larger or smaller... but everyone will see the same amount of the world.

5:4 was an arbitrary choice, use anything you'd like.

<hr></details>
<details><summary>Does this impact players of the game as well?</summary>

No, this option only impacts what you see in the editor.  When we cut a build for players, we'll update the supported resolutions to match.  TODO see xxx

<hr></details>



## Camera Size: 10

Update the camera size to about 10, effectively zooming out a bit.

<details><summary>How</summary>

 - In the "Hierarchy" select the "Main Camera"
 - In the "Inspector" change "Size" to 10

<img src="http://i.imgur.com/PmeoqG7.png" width=50% />

<hr></details>
<details><summary>Why change 'Size'?</summary>

This defines how much of the world is visible vertically.  Than the aspect ratio determines how much to display horizontally.  With those two values fixed, we can design a scene without any camera movement and be sure everyone has the same experience.

<hr></details>
<details><summary>Why not use the camera position instead?</summary>

2D games by default use "Projection: Orthographic".  This means that the camera does not consider perspective (the ability to see more of the world the further it is from your eye) which ensures that we see the edge of the screen head on rather than at an angle. 

The amount of the world visible with a perspective camera is driven by the Z location.  For an Orthographic, it's driven by a special "Size" property.

We don't want perspective in a 2D game because in order to make this possible the edges may appear distorted. For example:

<img src="http://i.imgur.com/5xCIowM.png" width=50% />
<img src="http://i.imgur.com/6rqvWDA.png" width=50% />

<hr></details>





## Create Platform GameObject

Create a new parent game object for the platform sprite.


<details><summary>How</summary>

 - Right click in "Hierarchy" and "Create Empty"
 - Rename to 'Platform'
 - Ensure the transform is at defaults for both the Platform and the sprite 'spritesheet_ground_72' (position 0, rotation 0, scale 1)

<img src="http://i.imgur.com/FAkZf1H.png" width=50% />

 - Drag and drop the sprite onto 'Platform' (it should appear indented under 'Platform')

 <img src="http://i.imgur.com/XOve0Ap.png" width=50% />

<hr></details>
<details><summary>Why not use a single GameObject instead?</summary>

Most of the platforms we will be creating require multiple different sprites to display correctly.  We tackle this in the next section.  Even for platforms which are represented with a single sprite, it's nice to be consistent across all our platforms.

<hr></details>
<details><summary>How is the sprite position calulated?</summary>

When a GameObject is a child of another GameObject, it's position is the combination of the child's position and the parent's position.  

Typically all transform updates during the game and in level design are done to the parent GameObject.  Child transforms are often static offsets from the center of that objects location.  e.g. we'll be adding rounded edges to the platform, which will require a x offset so they are positioned next to the middle segment.

<hr></details>



## Add edges

Add sprites with rounded edges to the left and right of the platform.

<details><summary>How</summary>

 - Copy the 'Platform' GameObject, paste and rename to 'PlatformWithEdges'.
 - You may want to use the move tool to separate the position of these objects on-screen.  When you do this, be sure the parent GameObject is selected and not the child sprite.
 - Drag the each of edge sprites from the "Project" tab Assets/spritesheet_ground into the "Hierarchy" under the 'PlatformWithEdges' GameObject (they should appear indented).  We're using 'spritesheet_ground_79' and 'spritesheet_ground_65'
 - Select an edge (one of the child GameObjects) and use the move tool to position it away from the other sprites.
 - Select an edge and hold V to enable Vertex Snap mode, a box appears for each anchor point.  Hover over the top right and click and drag the box which appears.  It will snap perfectly with other anchor points in the world.

<img src="http://i.imgur.com/GNMGb0w.gif" width=50% />

 - Repeat for both edges, creating smooth corners on both sides of the platform.
 - Copy paste 'PlatformWithEdges', rename to 'PlatformWithRightEdge' and delete it's left edge.  Do the same to create a 'PlatformWithLeftEdge'.

There should be four GameObjects in the world now, as shown below.

<img src="http://i.imgur.com/j1cz0aZ.png" width=50% />

<hr></details>



## Create a connected platform

Our level design calls for the bottom platform to rotate half way through.  Create two Platform GameObjects, one of which is rotated and position their parent GameObjects so they appear connected.

<details><summary>How</summary>

 - Use two copies of Platform (without edges) and move their parent GameObjects so that the sprites appear near the bottom of the screen side by side. Raise the right Platform a little above the left.
 - Select the child sprite in each and increase the Tiled "Width" to about 15 so that the platforms cover more than the width of the screen.
 - Select the parent GameObject for the Platform on the right and modify the Rotation Z value to about 4.
 - Drag and drop the child GameObject out of the Platform the right so it stands alone.  
 - Hold V to enable Vertex Snap, hover over the bottom left corner and drag the box which appears to connect perfectly with the other platform.
 - Copy paste the transform position from the child you just placed to it's original parent.
 - Drag and drop the sprite back into the original parent GameObject.
 - Confirm the child GameObject is positioned at 0.

<img src="http://i.imgur.com/iJ4fdYQ.gif" />

<hr></details>
<details><summary>Why not use a single GameObject for the bottom platform?</summary>

Soon in the tutorial we will be adding colliders to these platforms.  There are several ways this could be handled, as is always the case with GameDev, but the approach we will be using places BoxCollider2Ds on our Platform's parent GameObjects.  This works great when the parent is a middle sprite segment along with a rounded corner sprite - but does not work as well when the platform changes it's rotation half way through.


<hr></details>
<details><summary>Why extend the platform beyond the edge of the screen?</summary>

The width of the world players are going to see is fixed so you could argue that extending over the edge is not necessary.  I recommend this to ensure there are no unexpected gaps at the edge.  Additional some enemies in this game will continue off screen and use some of the platform we can't see before returning to the game.

Additionally we will be adding a screen shake feature.  This works by moving the camera up/down/left/right a bit.  Having the platforms extend beyond the edge of the screen allows us to do that without exposing unexpected gaps.

<hr></details>




## Rinse and repeat to complete Level1 platforms

At this point we have covered everything you need to match the Level1 platform layout.  You can match the layout we used or come up with your own.

Refer to the "Game" tab to confirm your layout.

The basic steps are:

 - Copy a parent Platform to start from.
 - Modify the tile "Width" as needed.  Platforms should extend off the screen a bit.
 - Use Vertex Snap to position the edge sprites.
 - Move and rotate the sprite by modifying the parent GameObject, leaving the children at position and rotation 0, with the exception of the corner sprites which have an X value.


TODO screenshots














<br><br><br><br><br>^^^^ ReadyForReview ^^^^ (below is WIP)<br><br><br><br><br>

















# 3) Add a Character and Movement Mechanics

Add a character to the scene.  Have him walk and jump, creating a basic platformer.


## Add a character sprite sheet

Add a sprite sheet for the character, slice it and set to point filter mode.  We are using [Kenney.nl's Platformer Characters](http://kenney.nl/assets/platformer-characters-1) 'PNG/Adventurer/adventurer_tilesheet.png'.


<details><summary>How</summary>

 - Drag/drop the sprite sheet into the "Project" tab Assets/Art folder.
 - Set "Sprite Mode" to "Multiple".
 - Click Sprite Editor and "Slice" by Cell Count, 9 rows 3 columns.
 - Set the "Filter Mode" to "Point".

 Note we won't be tiling the character sprite, so the default of "Mesh Type: Tight" is okay.

</details>




## Add Character to the Scene with a Walk Animation

Drag the sprites for walking into the Hierarchy to create a Character and animation.  We are using adventurer_tilesheet_0, adventurer_tilesheet_9, and adventurer_tilesheet_10.

<details><summary>How</summary>

 - Select 'adventurer_tilesheet_0', 'adventurer_tilesheet_9', and 'adventurer_tilesheet_10' sprites from the sprite sheet 'adventurer_tilesheet'.
 - Drag them into the Hierarchy.
 - When prompted, save the animation as Assets/Animations/Character/Walk.anim
 - Rename the GameObject to 'Character'.

This simple process created:
 - The character's GameObject.
 - A SpriteRenderer component on the GameObject defaulting to the first selected sprite.
 - An Animation representing those 3 sprites changing over time.
 - An Animation Controller for the character with a default state for the Walk animation.
 - An Animator component on the GameObject configured for the Animation Controller just created.

Click Play to test - your character should be walking!  

TODO gif this process

<hr></details>
<details><summary>What's the difference between Animation and Animator?</summary>

An animat**ion** is a collection of sprites on a timeline, creating an animated effect similiar to a flip book.  Animations can also include transform changes, fire events for scripts to react to, etc to create any number of effects.

An animat**or** controls which animations should be played at any given time.  An animator uses an animator controller which is a state machine used to select animations.

We will be diving into more detail on both of these later in the tutorial.  TODO link.

<hr></details>
<details><summary>What's a state machine / animator state?</summary>

A state machine is a common pattern in development to simplify the management of a complex system.  At any given momement a single state is in charge of the experience.  Transitions from one state to another may be triggered via script.

With animations each animator state has an associated animation to play.  When you transition from one state to another Unity will smoothly switch from one animation to the next.

<hr></details>




## Add a Rigidbody2D

Add a Rigidbody2D component to the character to enable gravity.

<details><summary>How</summary>

 - In the "Hierarchy" tab, select the Character's GameObject.
 - In the "Inspector" tab, click "Add Component" and select 'Rigidbody2D'.

 Hit play and watch the character fall through the platforms and out of view.

</details>
<details><summary>What's a Rigidbody2D?</summary>

A rigidbody is a core component for the Unity physics engine.  It's added to GameObjects which may be manipulated by physics during the game.

Physics refers to the logic in a game engine which moves objects based on forces such as gravity. We'll be using rigidbody's on all moving objects in this game. 

</details>



## Add BoxCollider2D to the Platforms

Add a BoxCollider2D component to each of the parent Platform GameObjects in the scene.

<details><summary>How</summary>

TODO words
 - Select parent
 - Add BoxCollider2D component
 - Set edge radius to .11
 - Click "Edit Collider", eyeball the outer line
 - For the bottom platform which is actually two connected - allow the colliders to overlap some.

</details>
<details><summary>Why not a PolygonCollide2D?</summary>

TODO

</details>
<details><summary>Why not place colliders on the child GameObjects?</summary>

TODO

</details>










aoeu


## Debugging

<details><summary>TODO</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

<hr></details>

TODO link to web build and git / source for the example up to here

<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>







## Move left/right

## Jump

## Add Platformer Effect to platforms





# Character Animations



























<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<hr>
<br>



## Build settings
<details>
<summary>
How
</summary>
Open player settings via "File"->"Build Settings".  Select the platform you want to build for and then click "Player Settings..."
<img src="http://i.imgur.com/nWDCAwX.png" width=50% />
For PC, we can select specific supported aspect ratios 
<img src="http://i.imgur.com/Xoxw0Xs.png" width=50% />
<hr></details>
