# 2D Platformer Tutorial for Unity 2017

TODO intro.

[Demo of the game](https://hardlydifficult.com/Kong/index.html) we are creating. TODO update to final build.

Target audience: we try to assume little without boring those with experience.  We expect you know some coding and can follow along with C# examples.  New concepts are introduced along the way to help beginners and intermediate developers.  No experience with Unity is required.

This is very much a WIP.  I'm trying to make a tutorial helpful to a range of experience levels.  Please let me know if you have any suggestions - I'm creating this live at [twitch.tv/HardlyDifficult](https://twitch.tv/HardlyDifficult) or email nick@HardlyDifficult.com

TODO
 - Image size is all over the place
 - say links are included when relevant er something.
 - Target character count on scripts is 65
 - Table of contents with anchors
 - Maybe copy paste for a fully expanded view all in one page (but note gifs make it not print friendly)
 - Lots of FAQs along the way, please consider these questions as you go, we try not to be redundant later on.
 - Stop saying cached for perf.

# 1) Create a game with enemies spawning

In chapter 1, we create a 2D game in Unity with spiked balls spawning at random intervals from an evil cloud.  They roll across platforms and fall from level to level.

TODO tutorial video link

<img src="http://i.imgur.com/31l9mA1.gif" width=50% />

TODO demo build

## Start a 2D project

Get Unity and start a 2D project. 

<details open><summary>How</summary>

 - [Download Unity](https://unity3d.com/), the free Personal edition has everything you need. 
 - Select "2D" when creating a new project.
 - Enter a name/directory - the other options can be left at defaults.

<img src="https://i.imgur.com/T2iZrmK.png" width=50% />

Familiarize yourself with the Unity Editor a bit.  This [guide from Unity](https://docs.unity3d.com/Manual/LearningtheInterface.html) is a nice, quick overview.

</details>




<details><summary>Why 2D?</summary>

Presenting the 2D vs 3D option when you create a new project suggests this is a significant choice.  It's not really... 2D just changes default settings on things like your camera.   Unity is a 3D engine, when creating 2D games your actually creating a 3D world where everything is very flat but the camera looks straight ahead and the only rotation in the world is around the z axis.  

[More on 2D vs 3D from Unity](https://docs.unity3d.com/Manual/2Dor3D.html).

<hr></details>


## Save scene

When you created the project, a default scene was created as well.  Save it as "Level1" as that's where this tutorial begins.

<details open><summary>How</summary>

 - File -> Save Scenes.
 - Create a "Scenes" directory, call it "Level1".

<hr></details>

<details><summary>What's a scene?</summary>

The Scene represents a collection of GameObjects and components (defined below) configured for a game level or menu screen.  For this tutorial we are starting by creating part of Level 1.  Level 2, the menu, and other UI screens will be saved as separate scenes.  You can switch scenes via the SceneManager, and will cover this later in the tutorial. 

<hr></details>




## Auto save script (optional)

Unity may crash.  I recommend adding an editor script which automatically saves everytime you hit play.

<details open><summary>How</summary>

 - Right click in the 'Project' Assets folder -> Create -> Folder and name it "Editor".
 - Create -> C# Script and name it "AutoSave".
 - Paste in the source code below.
 
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
  /// Called automatically before the game runs
  /// c/o InitializeOnLoad.  
  /// </summary>
  static AutoSave()
  {
    // Registers for play mode events.
    EditorApplication.playmodeStateChanged
      += OnPlaymodeStateChanged;
  }

  /// <summary>
  /// Called when the play mode changes.
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

You can confirm the save is working by noting the * in Unity's title.  This * indicates unsaved changes and should now go away everytime you click play.

<hr></details>
<details><summary>What about performance?</summary>

As an editor script, this logic is not included in the game you release.  Saving is incremental so there is very little time wasted when there is nothing new to save.  Unless you're one of the lucky ones who never sees Unity crash, this script is absolutely worth the time tradeoff.


<hr></details>
<details><summary>Why is the folder name important?</summary>

Unity uses [special folder names](https://docs.unity3d.com/Manual/SpecialFolders.html) for various features.  

Everything under a folder named "Editor" is an editor script, including files in any subdirectories.  e.g. this script could be saved as Assets/Editor/AutoSave.cs or Assets/Code/Editor/Utils/AutoSave.cs  There are two special folder names that work anywhere in your folder hierarchy like this:
 - "[Editor](https://docs.unity3d.com/Manual/ExtendingTheEditor.html)": These scripts are only executed when in the Unity Editor (vs built into the game).
 - "[Resources](https://docs.unity3d.com/ScriptReference/Resources.html)": Assets bundled with the game, available for loading via code.  Note Unity [recommends using resources only for prototypes](https://unity3d.com/learn/tutorials/temas/best-practices/resources-folder) - the preferred solution is [AssetBundles](https://docs.unity3d.com/Manual/AssetBundlesIntro.html). 
 
The following are additional special folder names only considered when in the root Assets directory. e.g. Assets/Gizmos is a special directory but Assets/Code/Gizmos could hold anything.
 - "[Standard Assets](https://docs.unity3d.com/Manual/HOWTO-InstallStandardAssets.html)": These are optional assets and scripts available from Unity that you can add to your project anytime by right clicking in the Assets directory and selecting from 'Import Package'.
 - "[Editor Default Resources](https://docs.unity3d.com/ScriptReference/EditorGUIUtility.Load.html)": A resources directory only available to Editor scripts so you can have assets appear in the editor for debugging without increasing the game's built size.
 - "[Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html)": Editor-only visualizations to aid level design and debugging in the 'Scene' view.
 - "[Plugins](https://docs.unity3d.com/Manual/Plugins.html)": Dlls to include, typically from 3rd party libraries.
 - "[StreamingAssets](https://docs.unity3d.com/Manual/StreamingAssets.html)": Videos or other archives for your game to stream.

Be sure you do not use folders with these names anywhere in your project unless specifically for that Unity use case. e.g. Assets/Code/Editor/InGameMapEditor.cs may be intended to be part of the game but would be flagged as an Editor only script instead.

<hr></details>
<details><summary>What's InitializeOnLoad</summary>

InitializeOnLoad is an attribute which enables the script.  The static constructor of any class with this attribute is executed before anything else in the game.

InitializeOnLoad is an editor only script and found under the UnityEditor namespace.

</details>


<details><summary>What's a C# attribute?</summary>

Attributes in C# are metadata added to classes, fields, or methods that may be queried by other classes.  In the AutoSave script, InitializeOnLoad, a Unity specific attribute, is used to ensure the static constructor on our AutoSave class is called when the game begins.

There are many [standard C# attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/index) and [Unity specific attributes](http://www.tallior.com/unity-attributes/) that may be used.  Here are a few random examples:

```csharp
using UnityEngine;
using UnityEngine.Networking;

// Tells unity that this component only works
// if the GameObject also has a SpriteRenderer
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
<details><summary>What's a C# delegate?</summary>

A delegate in C# is an object representing method(s) to call at a later time. You may encounter delegates under the following names: Events, Action, Func, and delegate. Under the hood these are all implemented with a 'multicast delegate'.  

When a method is added to a delegate to be called later, this is referred to as 'subscribing'.  Multicast delegate means that any number of methods may subscribe to the same delegate.  We use += when subscribing so not to overwrite any other subscribers.

```csharp
EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
```

If the owner of the delegate (in the example above that's EditorApplication) may outlive the subscriber, the subscriber should unsubcribe when it's destroyed.  Also, any time you are no longer interested in future updates, unsubscribe.  We do this with -= to remove our method and leave any remaining methods subscribed.

```csharp
EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
```

Events are a common use case for delegates.  For example, you may have a GameManager with a field for Points include an event "onPointsChange".  Other components/systems in the game, such as Achievements and the UI, may subscribe to the onPointsChange event.  When a player earns points, a method in Achievements is then called which can consider awarding a high score achievement and a method in the UI is called to refresh what the player sees on-screen.  This way those components only need to refresh when something has changed as opposed to checking the current state each frame.

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


## Import Platform art

Add a sprite sheet for the platform to the Assets directory.  We are using [Kenney.nl's Platformer Pack Redux](http://kenney.nl/assets/platformer-pack-redux) 'spritesheets/spritesheet_ground.png'.

<details open><summary>How</summary>


 - Right click in the 'Project' Assets directory -> Create Folder named "Art"  (optional).
   - You can rename folders by selecting and pressing F2.
 - Drag/drop the sprite sheet (or an entire folder of art) into the folder you just created.
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

<details open><summary>How</summary>

- Select the sprite sheet in the 'Project' tab (Assets/Art/spritesheet_ground).
- In the 'Inspector', set 'Sprite Mode' to 'Multiple'.
- Click 'Sprite Editor' and apply changes when prompted.

<img src="http://i.imgur.com/duYuVMy.png" width=50% />

- Click the 'Slice' menu item
  - Type: Grid By Cell Count
  - Column & Row: 8 & 16

<img src="http://i.imgur.com/hA2cMfv.png" width=50% />

- Click 'Slice' button
- Click 'Apply' and close the Sprite Editor 

<hr></details>
<details><summary>What does slicing achieve?</summary>

Slicing is the process of defining each individual sprite in a sprite sheet.  Once sliced, you can access each sprite as if it were a unique asset.

After you have sliced, white lines appear in the 'Sprite Editor'.  These lines show you how the sprite sheet is cut, boxing in each individual sprite.  Any whitespace as shown in this example is ignored (i.e. it does not generate blank sprites as well).

<img src="http://i.imgur.com/NawupLS.png" width=50% />

After closing the 'Sprite Editor' and applying changes you can expand the sprite sheet in Assets to see each sprite it created.

<img src="http://i.imgur.com/Qq0nn2B.png" width=50% />

<hr></details>




## Add the platform's middle segment to the scene

Add a sprite to the scene representing the middle segment of a platform.  We are using spritesheet_ground/spritesheet_ground_72.

Note: there may be visual artifacts which we will address below.

<details open><summary>How</summary>

 - Click the arrow on the sprite sheet in your Assets/Art directory (this displays each individual sliced image).
 - Click and drag the platform sprite you want to use into the 'Hierarchy' tab.  We are using spritesheet_ground_72. 
 
<img src="http://i.imgur.com/kZC4i6d.png" width=50% />

This creates a GameObject with a SpriteRenderer component for that sprite.

<hr></details>
<details><summary>What's a GameObject, Transform, and Component?</summary>

Everything you see and interact with in a game is driven by GameObjects.  Typically a GameObject represents a single logical object in the world (e.g. a character).  It may be composed of child GameObjects, each responsible for part of the display and/or behaviour. It may also hold various components.  

A component is a set of logic (i.e. code) which may be added to a GameObject, or child GameObject, and is exposed in the 'Inspector' tab for the GameObject you have selected in the 'Hierarchy'.  A GameObject may have any number of components and those components may by configured to customize the behaviour for that specific object.  

Unity has a number of components available out of the box, we will be using several Unity components in this tutorial and will be making many custom components as well.

A Transform component manages the GameObject's position, rotation and scale.  Every GameObject, including child GameObjects, have a Transform. Occasionally you will encounter a GameObject that has nothing rendered on screen.  In these cases the Transform is often completely ignored but may not be removed.

<hr></details>
<details><summary>What's a SpriteRenderer?</summary>

SpriteRenderer is a Unity component which renders a sprite on screen.  Select the GameObject in the 'Hierarchy' to view the SpriteRenderer component for this object in the 'Inspector'.  Here several options are available for modifying how the sprite is rendered.  For example:

 - Sprite: This is the sprite image to render.  It was populated automatically when you created the GameObject with drag/drop.
 - Color: White is the default, displaying the sprite as it was created by the artist.  Changing this color modifies the sprite's appearance.  You can also use the alpha value here to make a sprite transparent.

<img src="http://i.imgur.com/4w3P1nx.png" width=50% />

<hr></details>



## Sprite Draw Mode: Tiled

Update the platform's SpriteRenderer to Draw Mode: Tiled and adjust the width so we can begin to design the level.

Note: a warning may appear in the inspector and there may be visual artifacts which we will address below.

<details open><summary>How</summary>

 - Select the "spritesheet_ground_72" GameObject in the 'Hierarchy'.
 - In the 'Inspector', under the SpriteRenderer component:
   - Change 'Draw Mode' to 'Tiled'
   - An option for 'Width' appears, try increasing this to about 10 (but don't change height).

<img src="http://i.imgur.com/MIgzjdO.png" width=50% />

You should see the platform sprite get wider, repeating it's pattern.

<hr></details>
<details>
<summary>Why not use Transform scale?</summary>

Using transform scale to change the width cause the sprite displayed to stretch.  We are using tiling so the sprite repeats instead:

<img src="http://i.imgur.com/ejbs3RK.png" width=50% />

<hr></details>



## Sprite Filter Mode: Point

Update the sprite sheet's import settings to use filter mode point, preventing some visual artifacts.

<details open><summary>How</summary>

 - Select the sprite sheet in the 'Project' tab (Assets/Art/spritesheet_ground).
 - In the 'Inspector', set 'Filter Mode' to 'Point (no filter)' and apply when prompted.

<img src="http://i.imgur.com/B0nqf75.png" width=50% />

<hr></details>
<details><summary>Why use Point and not the default of Bilinear?</summary>

Filter mode of Bilinear or Trilinear blurs the image a bit in attempt to make smooth lines.  Often for a 2D game, we want control down to the pixel and this effect is not desirerable.  Here's an example with the character sprite we will be using:

<img src="http://i.imgur.com/AYyx3Ma.png" width=150px />

<img src="http://i.imgur.com/8wMlM1S.png"  width=150px />

For sprite sheets, often each object is touching the one next to it.  Filter Mode Point prevents blending happening between one sprite and it's neighbor.  The blending that occurs with other modes besides Point may lead to random lines showing up on screen.  For example:

<img src="http://i.imgur.com/ZKqg5JP.png" width=50% />

<hr></details>


## Sprite Mesh Type: Full Rect

Update the sprite sheet's import settings to use mesh type full rect since we will be using tiling.

<details open><summary>How</summary>

 - Select the sprite sheet.
 - In the 'Inspector', set 'Mesh Type: Full Rect'.

<img src="http://i.imgur.com/Dhe3Nzt.png" width=50% />

<hr></details>

<details><summary>Why use Full Rect and not the default of Tight?</summary>

When using tiling on a sprite, Unity recommends updating the sprite sheet to use 'Full Rect'.  I don't have an example of issues that may arrise from using 'Tight' instead, but here is the warning from Unity recommending 'Full Rect':

<img src="http://i.imgur.com/e9jE83B.png" width=50% />

<hr></details>



## Disable Anti Aliasing

Disable Anti-Aliasing, preventing some visual artifacts.

<details open><summary>How</summary>

 - Menu 'Edit' -> 'Project Settings' -> 'Quality'
 - In the 'Inspector' change 'Anti Aliasing' to 'Disabled'
 - Repeat this for each quality 'Level' supported
   - Click on the row to modify (e.g. 'Very High')
   - Update 'Anti Aliasing' if needed

The highlighted Level is what you are testing with ATM.  It will default to Ultra.  The green checkboxes represent the default quality level for different build types.  In this example I'm testing with Ultra, using Ultra by default for PC builds, and High by default for WebGL builds.  To avoid artifacts, I disable Anti Aliasing in every level and then switch back to Ultra.

<img src="http://i.imgur.com/omFI4DD.png" width=50% />

<hr></details>
<details><summary>What is Anti Aliasing and why disable it?</summary>

Anti Aliasing is a technique used to smooth jagged edges as shown here:

<img src="https://qph.ec.quoracdn.net/main-qimg-10856ecbea4f439fb9fb751d41ff704a" width=50% />

Like changing the filter mode to Point, we do this when working with sprites because we often want control over images down to the pixel.

Anti-aliasing may lead to unexpected gaps or distortions when sprites are side by side.  Here is an example that appears when using tiling and Anti Aliasing is enabled:

<img src="http://i.imgur.com/vY5YmVj.png" width=50% />

<hr></details>





## Set a 5:4 Aspect ratio

Change the aspect ratio to 5:4 as we want to simplify and always show the same amount of the world on screen.

<details open><summary>How</summary>

 - In the 'Game' tab, near the top, change 'Free Aspect' to '5:4'.

<img src="http://i.imgur.com/MTnZtu4.png" width=50% />

Switch back to the 'Scene' tab.  The white box here represents the area that players will see.

<img src="http://i.imgur.com/eIq2LD2.png" width=50% />

 - File -> Build Settings
 - Select the desired platform and click 'Player Settings'.
 - In the 'Inspector' tab, set the supported resolution or aspect ratio (this will be different for different platform types).

<img src="http://i.imgur.com/zTHTwHt.png" />

<img src="http://i.imgur.com/UHP5YVL.png" />

TODO review
Open player settings via "File"->"Build Settings".  Select the platform you want to build for and then click "Player Settings..."
<img src="http://i.imgur.com/nWDCAwX.png" width=50% />
For PC, we can select specific supported aspect ratios 
<img src="http://i.imgur.com/Xoxw0Xs.png" width=50% />

<hr></details>
<details><summary>Why use a fixed aspect ratio?</summary>

We are building a game with a fixed display.  The camera is not going to follow the character which will simplify the game and level design for this tutorial.  With a fixed aspect ratio we can design a scene without any camera movement and be sure everyone has the same experience.

Different resolutions will scale the display larger or smaller but everyone will see the same amount of the world.

5:4 was an arbitrary choice, use anything you'd like.

<hr></details>



## Camera Size: 10

Update the camera size to about 10 to zoom out a bit.

<details open><summary>How</summary>

 - In the 'Hierarchy' select the 'Main Camera'
 - In the 'Inspector' change 'Size' to 10

<img src="http://i.imgur.com/PmeoqG7.png" width=50% />

In the Game tab, the platform should look smaller now.  In the Scene, the white box representing the viewable area has grown.

<hr></details>
<details><summary>Why change 'Size' and not camera position?</summary>

2D games by default use 'Projection: Orthographic'.  This means that the camera does not consider perspective, the ability to see more of the world the further it is from your eye. The amount of the world visible with a perspective camera is driven by it's position.  

For an Orthographic camera, the amout of the world visible is driven by a special 'Size' property. 'Size' defines how much of the world is visible vertically.  Then the aspect ratio is used to determine how much to display horizontally.  

<hr></details>



## Create Platform GameObject

Create a new parent GameObject for the platform sprite.


<details open><summary>How</summary>

 - Right click in 'Hierarchy' and 'Create Empty'.
 - Rename to "Platform".
 - Ensure the Transform is at defaults (position 0, rotation 0, scale 1) for both the Platform and the sprite spritesheet_ground_72.

<img src="http://i.imgur.com/FAkZf1H.png" width=50% />

 - Drag and drop the sprite onto the "Platform" GameObject (it should appear indented under "Platform" in the Hierarchy)

 <img src="http://i.imgur.com/XOve0Ap.png" width=50% />

<hr></details>
<details><summary>Why not use a single GameObject instead?</summary>

Most of the platforms we will be creating require multiple different sprites to display correctly.  We tackle this in the next section.  Even for platforms which are represented with a single sprite, it's nice to be consistent across all of our platforms.

The implications of using a parent GameObject or not will be more clear when we start to add game mechanics later in the tutorial.

<hr></details>
<details><summary>How is the sprite position calulated when it's a child?</summary>

When a GameObject is a child of another GameObject, it's position, rotation, and scale are the combination of the child's Transform and the parent's Transform (via matrix multiplication).  

Typically all Transform updates during the game and in level design are done to the parent GameObject.  Child Transforms are often static offsets from the center of the parent GameObject.  e.g. we'll be adding rounded edges to the platform, which will require an x offset so they are positioned next to the middle segment.

<hr></details>



## Add edges

Add sprites with rounded edges to the left and right of the platform.  We are using spritesheet_ground_79 and spritesheet_ground_65.

<details open><summary>How</summary>

 - Copy the "Platform" GameObject, paste and rename to "PlatformWithEdges".
 - You may want to move these to separate them on-screen (making it easier to configure each).  When you do, be sure the parent GameObject is selected and not the child sprite.
 - Drag the each of edge sprites from the 'Project' tab Assets/spritesheet_ground into the 'Hierarchy' under the "PlatformWithEdges" GameObject (they should appear indented).  We're using spritesheet_ground_79 and spritesheet_ground_65.
 - Confirm that each of the child sprites are still at 0 position, 0 rotation, and 1 scale.  The edge sprites may have an X position.
 - Select the rounded edge sprite (one of the child GameObjects) and use the move tool to position it away from the other sprites.
 - With the edge still selected, hold V to enable Vertex Snap mode.  A box appears for each anchor point (e.g. the corners of the sprite).  Hover over the top right corner and click and drag the box which appears.  It will snap perfectly with other anchor points in the world as show below.

<img src="http://i.imgur.com/GNMGb0w.gif" width=50% />

 - Repeat for both edges, creating smooth corners on both sides of the platform.
 - Copy paste "PlatformWithEdges", rename to "PlatformWithRightEdge" and delete it's left edge.  Do the same to create a "PlatformWithLeftEdge".

There should now be four GameObjects in the world, as shown below.

<img src="http://i.imgur.com/j1cz0aZ.png" width=50% />

<hr></details>




## Create a connected platform

Our level design calls for the bottom platform to rotate half way through.  Create two Platform GameObjects and position and rotate their parents' GameObjects so that they appear connected.

<details open><summary>How</summary>

 - Use two copies of the Platform GameObject (without edges) and move their parent GameObjects so that the sprites appear near the bottom of the screen side by side. 
 - Raise the right Platform a little above the left.
 - Select the child sprite in each and increase the tiled 'Width' to about 15 so that the platforms combined cover more than the width of the screen.
 - Select the parent GameObject for the Platform on the right and modify the Transform's rotation Z value to about 4.
 - Drag and drop the child GameObject out of the Platform you just rotated so it stands alone. (it will still appear at the same position/rotation). 
 - Hold V to enable Vertex Snap, hover over the bottom left corner and drag the box which appears to connect perfectly with the other platform.
 - Copy paste the Transform position from the child you just placed to it's original parent GameObject.
 - Drag and drop the sprite back into the original parent GameObject.
 - Confirm the child GameObject's Transform is positioned at 0.

<img src="http://i.imgur.com/iJ4fdYQ.gif" />

<hr></details>
<details><summary>Why not use a single GameObject for this bottom platform?</summary>

Up next we will be adding colliders to these platforms.  There are several ways this could be handled, as is always the case with GameDev. We will be placing BoxCollider2Ds on our Platforms' parent GameObjects.  This works great when the parent is a middle sprite segment along with a rounded corner sprite - but does not work as well when the platform changes it's rotation half way through.

<hr></details>
<details><summary>Why extend the platform beyond the edge of the screen?</summary>

The width of the world players are going to see is fixed so you could argue that extending over the edge is not necessary.  I recommend this to ensure there are no unexpected gaps at the edge and to leave some flexibility for future mechanics, including:

 - Allow some enemies to continue off screen and use the platform we can't see before returning to the game.
 - Screen shake.  This works by moving the camera up/down/left/right a bit.  Having the platforms extend beyond the edge of the screen allows us to do that without exposing gaps.

<hr></details>


## Add BoxCollider2D to the Platforms

Add a BoxCollider2D component with a .1 edge radius to each of the parent Platform GameObjects in the scene.

<details open><summary>How</summary>

 - Select a platform's parent GameObject.
 - Add Component -> "BoxCollider2D".
 - Set 'Edge Radius' to '.1'
 - Click 'Edit Collider' and click/drag the box which appears so that the outer green line encapsulates the platform.
    - Click and then hold Alt while adjusting the sides to pull both sides in evenly.

<img src="http://i.imgur.com/rHhHSXL.gif" />

 - For the bottom platforms which are actually two connected parent platform GameObjects - allow the colliders to overlap some for a smooth experience when entities are walking from one to the next.

<img src="http://i.imgur.com/cgfqZhY.gif" />

</details>
<details><summary>What is a Collider?</summary>

[Colliders](https://docs.unity3d.com/Manual/CollidersOverview.html) are components placed on GameObjects to define their shape for the purposes of physical collisions.  The collider shape may or may not align with the visuals on screen.

Typically colliders match the shape of the art on screen.  For example, they are used to keep the character from falling through the floor or walking through walls, and to cause the character to die when they hit an enemy.

Colliders may also be used as 'triggers' to detect something happening near an object without causing a physical reaction.  For example, an entity could have a second collider twice as large as the entity itself and use that to know when danger is approaching - causing the entity to run the other way.

</details>
<details><summary>Why not place colliders on the child GameObjects instead?</summary>

Well, you could!  With GameDev, you'll find there are almost always various ways you could achieve a goal and pros/cons to each.  

Since we are using BoxCollider2D and an Edge radius, getting our sprites to connect with a smooth surface for entities to walk over would be more challenging when the colliders are on the child sprite GameObjects instead of the parent Platform.  

<img src="http://i.imgur.com/QTjSEt7.png" width=50% />

Additionally, fewer colliders may improve your game's performance - however the difference here will not be noticeable.

</details>


## Rinse and repeat to complete Level1 platforms

At this point we have covered everything you need to match the Level1 platform layout.  You can match the layout we used or come up with your own.

Refer to the Game tab to confirm your layout as they player will see it.

<details open><summary>How</summary>

The basic steps are:

 - Copy a parent Platform to start from.
 - Modify the tile 'Width' as needed.  Platforms should extend off the screen a bit.
 - Use Vertex Snap to position the edge sprites.
 - Update the colliders to match the platform visuals on screen.  They will need to be updated if the width or edges were changed.
 - Move and rotate the sprite by modifying the parent GameObject, leaving the children at position and rotation 0, with the exception of the corner sprites which have an X position.

Optionally, you can rename the platform GameObjects and organize your platforms by placing them in a parent GameObject.  e.g.:

 - Click and drag to re-arrange the platform GameObjects so they appear in the same order in the hierarchy as they do in game.
 - Rename each to represent it's position - e.g. "Level2"
 - Create an Empty GameObject, name it "Platforms".  Ensure that it is a position 0.
 - Select all of your existing platforms (the parent GameObjects) and click and drag them onto "Platforms".

<img src="http://i.imgur.com/f8GFdBD.png" width=50% />

<hr></details>



## Create a Spike Ball GameObject

Add a sprite and GameObject for the spike ball.  Set filter mode to Point.  We are using We are using [Kenney.nl's Jumper Pack](http://kenney.nl/assets/jumper-pack) 'PNG/enemy/spikeball1'.

<details open><summary>How</summary>

 - Drag/drop the sprite into Assets/Art.
 - Set the 'Filter Mode: Point (no filter)'.
 - Drag the sprite into the 'Hierarchy' tab.  Rename to 'Spike Ball'.

</details>



## Change Order in Layer to -1

Update the Spike Ball's SpriteRender Order in Layer to -1.

<details><summary>How</summary>

 - Select the Spike Ball's GameObject
 - In the 'Inspector', set the SpriteRenderer's 'Order in Layer' to '-1'.

<img src="http://i.imgur.com/TSqk7hb.png" width=50% />

</details>
<details><summary>What does Order in Layer do?</summary>

When multiple sprites are overlapping, Order in Layer is used to determine which one is on top of the other.  So if the spike ball sprite has Order in Layer '-1' and everything else uses the default Order in Layer '0', the spike ball will always appear behind of other sprites in the world.

Order in Layer may be any int value, positive or negative. Here's an example showing the character sprite we will be using with Order in Layer '-1' and with '2'... sitting on a platform which still has the default Order in Layer '0'.

<img src="http://i.imgur.com/QCHPLDf.png" width=50% />

</details>

## Add a Rigidbody2D

Add a Rigidbody2D component to the spike ball to enable gravity.

<details open><summary>How</summary>

 - Select the Spike Ball's GameObject.
 - In the 'Inspector', click 'Add Component' and select "Rigidbody2D".

Hit play and watch the spike ball fall through the platforms and out of view:

<img src="http://i.imgur.com/PuWWL3z.gif" width=50px />

</details>
<details><summary>What's a Rigidbody2D?</summary>

A rigidbody is a core component for the Unity physics engine, Rigidbody2D is the 2D version of this component (vs 3D).  It's added to GameObjects which may be manipulated by physics during the game.

Physics refers to the logic in a game engine which moves objects based on forces such as gravity. We'll be using rigidbodys on all moving objects in this game. 

</details>


## Add a CircleCollider2D

Add a CircleCollider2D component to the spike ball so it hits the platform and rolls.

<details open><summary>How</summary>

 - Select the Spike Ball's GameObject.
 - 'Add Component' and select "CircleCollider2D".
 - Modify the radius so the collider is around the main body and not the spikes.

<img src="http://i.imgur.com/ozPgCi4.gif" width=200px />

Hit play to watch the spike ball fall onto a platform and roll.

<img src="http://i.imgur.com/x4a848N.gif" width=200px />

</details>
<details><summary>Why shrink the collider?</summary>

It's optional, use what you think creates the best experience.

When we added the CircleCollider2D, it defaulted to surround the entire sprite.  This may be the right experience, it's up to how you want the game to play.  I'm suggesting that we pull the collider in a bit, this will cause the spike ball to roll on its body with the spikes digging into platforms instead of rolling on the tips of each spike as shown here:

<img src="http://i.imgur.com/ov1F5Fo.gif" width=200px />

<img src="http://i.imgur.com/WRLQITb.gif" width=200px />

On a related note, seting the 'Order in Layer' to '-1' ensures that the spikes are behind the platform.  Without this the spikes would be ontop:

<img src="http://i.imgur.com/8cgB7jZ.gif" width=200px />


</details>



## Add invisible bumpers

Add additional BoxCollider2Ds offscreen to cause the spike balls to quickly turn around and roll back on-screen.

<details open><summary>How</summary>

 - Right click in 'Hierarchy' -> 'Create Empty'.  Rename to "Bumper"
 - 'Add Component' and select "BoxCollider2D"
 - Increase the collider Size X to about 20
 - Move the GameObject off screen, near one of the platforms, like this:

<img src="http://i.imgur.com/gNQUfh6.png" width=150px />

 - Rotate the Transform Z to about -30.
 - Reposition so the edge of our bumper overlaps the platform.

<img src="http://i.imgur.com/mUv143i.png" width=150px />

Hit play, the spike ball should hit the bumper and quickly reverse and then accelerate the other direction: 

<img src="http://i.imgur.com/vMjWoia.gif" width=150px />

 - Copy paste the bumper and modify it's position and rotation so that each platform that may send a spike ball offscreen has a bumper.
 - Remove the bumper from the bottom left as we do not want spike balls to roll back on screen after reaching this point.
 - Optionally move each Bumper GameObject under a new parent empty GameObject "Bumpers" to keep the 'Hierarchy' organized, like we had done for Platforms.

 Your screen with bumpers should look something like this:

<img src="http://i.imgur.com/cAl0od4.png" />

</details>


## Add a C# script to get the ball moving

For the game, we  want the ball spawning in the top left.  But that's a flat surface so the ball does not roll down platforms.  Give it an initial Velocity of (3, 0) and AngularVelocity of -500.

<details open><summary>How</summary>

 - In the 'Project' tab Assets/Code directory, create a new directory "Components/Movement".
 - Right click -> Create -> C# script and name it "InitializeRigidbodyOnStart".
 - Select the spike ball, click 'Add Component' and select "InitializeRigidbodyOnStart".
 - Double click the script to open it in Visual Studio (or Monodevelop).
 - Copy the source below:

 ```csharp
 using UnityEngine;

/// <summary>
/// When a GameObject is added to the scene, give it a starting
/// velocity and angular speed.
/// 
/// This script only impacts the GameObject on Start. 
/// Physics drag will slow these down over time.  Gravity may 
/// speed them up again.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class InitializeRigidbodyOnStart : MonoBehaviour
{
  /// <summary>
  /// Cause the GameObject's rigidbody to start with this spin.
  /// </summary>
  [SerializeField]
  float startingAngularVelocity = -500;

  /// <summary>
  /// Cause the GameObject's rigidbody to start with this 
  /// velocity.
  /// </summary>
  [SerializeField]
  Vector2 startingVelocity = new Vector2(3, 0);

  /// <summary>
  /// Unity event called when the GameObject is first spawned
  /// in the world.
  /// </summary>
  protected void Start()
  {
    // Update the GameObject's rigidbody initial settings
    Rigidbody2D myBody = GetComponent<Rigidbody2D>();
    myBody.angularVelocity = startingAngularVelocity;
    myBody.velocity = startingVelocity;
  }
}

```

</details>
<details><summary>Why not use a "SpikeBall" component instead?</summary>

You could, but...  

Unity encourages component based solutions, where you aim to offer a single mechanic per component.  Here's a good [wikipedia article on component based](https://en.wikipedia.org/wiki/Component-based_software_engineering) design.  Briefly, the advantages to this approach are:

 - Each script or component focuses on a single feature or mechanic, simplifying and making it easier to debug.
 - Components may be reused between different object types.  If we had one master SpikeBall component and then created a similar enemy with a few different mechanics, reusing logic would be more challanging and we might copy paste parts to our new enemy compoment instead. 

</details>
<details><summary>What's velocity and angularVelocity?</summary>

A GameObject with a rigidbody may be moved with forces.  The Unity Physics engine uses these forces as inputs in order to calculate the object's position and rotation, considering other things in the world such as a wall blocking your path.  

Unity follows the [Newton's Laws of Motion](https://en.wikipedia.org/wiki/Newton%27s_laws_of_motion) - e.g. an object either remains at rest or continues to move at a constant velocity, unless acted upon by a force.

There are various APIs for manipulating forces on a rigidbody.  This script will be setting initial values for:

 - Velocity: the desired movement direction and speed.  Abstent any additional forces, 'Drag' decreases the velocity every frame until it reaches 0.
 - Angular velocity: degrees per second to rotate the object.  Abstent any additional forces, 'Angular drag' will decrease this until it reaches 0.

</details>
<details><summary>What's SerializeField and why not use public instead?</summary>

[SerializeField] exposes the object's field (data) in the 'Inspector' tab.  The default value seen in the C# script becomes the default in the Inspector - however when the script runs, the value is whatever you set for that object in the Inspector. This allows you to change values per-object or have different values for a component which is used on various different object types.  You can also change values in the Inspector at runtime, which can be helpful while debugging.

Read [more about Serialization in Unity](https://docs.unity3d.com/Manual/script-Serialization.html).

Any public field is a SerializeField by default.  If you do not want a public field to be exposed in the inspector, you can add the [NonSerialized] attribute (from the System namespace).  

So why not just public instead of [SerializeField]?

The fields in question are often only leveraged inside the component itself.  Other components may not interact with these fields directly.  In those scenarios, I prefer to follow the Object-Oriented programming best practice of [data encapsulation](https://en.wikipedia.org/wiki/Encapsulation_(computer_programming)) - meaning we only expose public fields when we want other classes to interact with them.

</details>
<details><summary>What's RequireComponent do?</summary>

[RequireComponent] is an Unity attribute used to let the editor know that this component requires another component on the same GameObject.

```csharp
[RequireComponent(typeof(ComponentThatMustBeOnThisGameObject))]
public class MyComponent ...
```

When you add a component in the inspector which requires another, and the required component is not already on that GameObject, Unity will automatically add it for you.

</details>
<details><summary>What is MonoBehaviour / how is Start() called?</summary>

Most of the scripts that you create in Unity will derive from MonoBehaviour.  [MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html) is the base class for a GameObject component (scripts on objects in your world).  It allows you to execute logic every Update (each frame) and respond collision events, etc.

There are a lot of events available to MonoBehaviours.  In this example we are using Start which is called once per-object, when that object is first spawned in the world.

Note that when implementing MonoBehaviour events, you do not use 'override' nor subscribe to the event.  Unity uses reflection based on the method signature instead to improve performance.  This creates an unintuative pattern for C# delevelopes but allows Unity to eliminate unncessary calls.  This optimization normally in development would be considered overkill but for a game engine this kind of thing adds up, particularly since there are typically hundreds of MonoBehaviours in the world.

TODO flow of events https://docs.unity3d.com/uploads/Main/monobehaviour_flowchart.svg

</details>
<details><summary>Why use protected on the Unity event?</summary>

Protected is an access modifier in C# which ensures that the only way to call that method, or field, is from the same class or from a class that derives from it.  Unity will find events such as Update() based on the signature, ignoring the access modifier - allowing you to use anything you'd like.

Why protected and not private?

When you are using inheritence and both the child and parent classes need to include an event such as Update(), Unity will only call the child's implementation.  This can make it easy to miss that some events in the parent class have been overwritten (vs complemented by) the child.

I recommend using protected on every Unity event so that the compiler can help avoid this mistake.  In the event the parent and child classes both have protected Update(), you will get a compile warning about the conflict.  

If you want both child and parent called, update the methods as follows:

```csharp
using UnityEngine;

public class Test : MonoBehaviour
{
  protected virtual void Update()
  {
    // Parent update logic
  }
}

public class AChildOfTest : Test
{
  protected override void Update()
  {
    base.Update();
    // Child update logic to run after the parent's Update
  }
}
```

If you want the child to replace the parent's update method (so that the parent's Update is never called), update the method like so:

```csharp
using UnityEngine;

public class Test : MonoBehaviour
{
  protected void Update()
  {
    // Parent update logic
  }
}

public class AChildOfTest : Test
{
  protected new void Update()
  {
    // Child update logic to be run instead of the parent's Update
  }
}
```

What if it's not a parent class?

I recommend always using protected on Unity events.  A class may not be a parent at the moment but code constantly changes and matures.  This is a best practice to help avoid potential issues in the future.  If the class never becomes a parent, the method is effectively treated as private.  There is no performence or other runtime impact from using protected.

Why not always make the methods virtual?

Performance.  There is a runtime cost to marking a method as virtual, even if there are no overrides.

Why not public instead?

Encapsulation.  If we were to make these methods public, it suggests that other components may call the events directly.  I've yet to encounter a use case where it's appropriate to do that - you should rely only on Unity to call these events to keep your code clean.

</details>

## Add a C# script to destroy balls that roll off

After the ball rolls off the bottom platform, destroy the GameObject.

<details open><summary>How</summary>

 - Create a C# script "SuicideOutOfBounds" under Assets\Code\Components\Death.
 - Select the spike ball, click 'Add Component' and select SuicideOutOfBounds.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Destroy this GameObject if it falls off screen.
/// </summary>
public class SuicideOutOfBounds : MonoBehaviour
{
  /// <summary>
  /// Anything with a transform Y position less than 
  /// this is considered out of bounds.
  /// 
  /// To calculate this dynamically, try:
  /// Camera camera = Camera.main;
  /// const float maxEntityHeight = 2;
  /// float outOfBoundsYPosition =
  ///   -camera.orthographicSize
  ///   + camera.transform.position.y
  ///   - maxEntityHeight;
  /// </summary>
  const float outOfBoundsYPosition = -12;

  /// <summary>
  /// Unity event called each frame.
  /// </summary>
  protected void Update()
  {
    // If this is lower than the camera can see, 
    // then it has fallen out of bounds
    if(transform.position.y < outOfBoundsYPosition) 
    { 
      Destroy(gameObject);
    }
  }
}
```

Play and the ball should now destroy itself when it falls off screen:

<img src="http://i.imgur.com/xcqUO8I.gif" />

</details>
<details><summary>Why bother, the GameObject is already off screen?</summary>

When a GameObject is off screen, there is no attempt to render it so your GPU is not wasting time but Unity is still processing Physics and logic for any components on the GameObject.  In this case, once the GameObject has fallen off the bottom it will never return to the game.  

We destroy it to save performance while the game is running.  Without this script, the endless stream of balls spawning and then falling off would be a 'memory leak'.  This means that you are wasting resources and over time the performance of your game will get worse.

</details>
<details><summary>What is Destroy and why not Destroy(this)?</summary>

Destroy is a Unity method to remove something from the scene.  You can:

 - Destroy a component, causing the component to be removed from that GameObject (and stopping future event calls such as Update).  
 - Destroy a GameObject, causing that entire GameObject to be removed from the scene.

For example:

```csharp
using UnityEngine;

public class MyComponent : MonoBehaviour
{
  public bool shouldThisComponentStop;
  public bool shouldThisGameObjectBeRemoved;

  protected void Update()
  {
    if(shouldThisComponentStop)
    {
      // Remove MyComponent from this GameObject
      Destroy(this); 
    }
    if(shouldThisGameObjectBeRemoved)
    {
      // Destroy this entire GameObject from the scene
      Destroy(gameObject);
    }
  }
}
```

</details>
<details><summary>What about an object pool?</summary>

An object pool is an optimization technique which may be appropriate to use but we are not implementing it here for simplicity.  Additionally the performance gain for a game like this would be negligible.

What is an object pool?

Instantiate (creating a new GameObject) is one of the most expensive calls you can make.  An object pool is the programming term for reusing objects instead of destroying and creating new ones.  

For this example, instead of destroying a spike ball that falls off screen we would instead have it respawn at the top and go through the entire level again.

When should an object pool be used?

Objects which destroy and spawn again several times may warrent an object pool. There is overhead associated with having and using an object pool so it is not recommended for absolutely everything.  For example, a boss which is going to surface once in a game may not be a good choice to include in an object pool.

How is an object pool implemented?

Basically anytime we spawn a GameObject, we ask the object pool if there is one already available for us to reuse.  And when we would have destroyed a GameObject, we would instead do gameObject.SetActive(false) and add it to the object pool's list of available objects.  

For more, see [Catlike Coding's Object Pool tutorial](http://catlikecoding.com/unity/tutorials/object-pools/).

</details>



## Add the Evil Cloud to the scene

Add a sprite and GameObject for the evil cloud, place it in the top left and scale the size.  Set order in layer to 1. We are using [Kenney.nl's Jumper Pack](http://kenney.nl/assets/jumper-pack) 'PNG/enemy/cloud' with filter mode Point.

<details open><summary>How</summary>

 - Drag/drop the sprite into Assets/Art.
 - Set the 'Filter Mode: Point (no filter)'.
 - Drag the sprite into the 'Hierarchy' tab. Rename to "Evil Cloud".
 - Move the cloud to the top left of the screen.
 - Scale the cloud (evenly on all dimensions) to fit nicely.

<img src="http://i.imgur.com/fecCBCq.gif" width=200px />

</details>
<details><summary>What does changing scale Z do?</summary>

Nothing (for 2D games).  When we are scaling, in order to not distort the art we only need to ensure X and Y scales match.  Z could be left at the default of 1, but I prefer to keep it in sync with X and Y as well as Unity's scale tool will do this by default.

</details>




## Add a C# script to spawn balls

Have balls spawn periodically at the evil cloud and fall down the platforms.

<details open><summary>How</summary>

 - Create a C# script Assets/Code/Components/Life/Spawner.
 - Add the Spawner component to the evil cloud GameObject.
 - Edit the Spawner script, paste the following code.

 ```csharp
using System.Collections;
using UnityEngine;

/// <summary>
/// Instantiates a prefab at this object's location periodically.
/// </summary>
public class Spawner : MonoBehaviour
{
  /// <summary>
  /// Set to the prefab to instantiate.
  /// </summary>
  [SerializeField]
  GameObject thingToSpawn;

  /// <summary>
  /// How long to wait before spawning begins.
  /// </summary>
  [SerializeField]
  float initialWaitTime = 2;

  /// <summary>
  /// The least amount of time between each spawn.
  /// </summary>
  [SerializeField]
  float minTimeBetweenSpawns = .5f;

  /// <summary>
  /// The most amount of time between each spawn.
  /// </summary>
  [SerializeField]
  float maxTimeBetweenSpawns = 10;

  /// <summary>
  /// Unity event called when the GameObject is first spawned
  /// in the world.
  /// </summary>
  protected void Start()
  {
    // Starts a Coroutine which executes a spawn script over
    // a period of time.
    StartCoroutine(SpawnEnemies());
  }

  /// <summary>
  /// The spawn script.  Called initially on Start, then 
  /// executed as a Unity Coroutine over time.
  /// </summary>
  /// <returns>Used by Coroutines to manage time.</returns>
  IEnumerator SpawnEnemies()
  {
    // Pre conditions
    Debug.Assert(thingToSpawn != null,
      "thingToSpawn has not been set");
    Debug.Assert(initialWaitTime >= 0,
      "initialWaitTime should not be negative");
    Debug.Assert(minTimeBetweenSpawns >= 0,
      "minTimeBetweenSpawns should not be negative");
    Debug.Assert(maxTimeBetweenSpawns >= minTimeBetweenSpawns,
      "maxTimeBetweenSpawns should be >= minTimeBetweenSpawns");

    // Wait before first spawn, 0 or more seconds
    yield return new WaitForSeconds(initialWaitTime);

    // Loop until this is destroyed
    while(true)
    {
      // Spawn thingToSpawn at this GameObject's location
      Instantiate(thingToSpawn, transform.position, Quaternion.identity);

      // Sleep a random amount of time before the next spawn
      float sleepTime = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
      yield return new WaitForSeconds(sleepTime);
    }
  }
}
 ```

 - After the code has been saved, create a prefab for "Spike Ball":
    - Create a folder Assets/Prefabs.
    - Select the Spike Ball GameObject and click/drag it to the folder.

<img src="http://i.imgur.com/w4leZGk.gif" width=250px />

 - Delete the "Spike Ball" GameObject from the 'Hierarchy', removing it from the scene but leaving our prefab in-tact.
 - Select the "Evil Cloud" GameObject and then click/drag the prefab you just created onto the "Thing To Spawn" field in the 'Inspector'.

<img src="http://i.imgur.com/scu8YUR.gif" width=250px />

Click play to see the spawner in action:

<img src="http://i.imgur.com/ZJSulAj.gif" width=200px /> 

</details>
<details><summary>What's a prefab?</summary>

A prefab is a file representing a configured GameObject.  This includes any child GameObjects as well as Components and their settings from the Inspector. 

This allows things like our spawner to instantiate a GameObject with the appropriate components and configurations, without knowing any details about the specific object type it is spawning.  More [on prefabs from Unity](https://docs.unity3d.com/Manual/Prefabs.html).

</details>
<details><summary>What is a Coroutine / WaitForSeconds?</summary>

A Coroutine allows you to define a sequence which takes more than a single frame to execute.  It's implemented with a C# enumerator which Unity will then execute over time.  For example:

```csharp
using System.Collections;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
  protected void Start()
  {
    StartCoroutine(ExampleCoroutine());
  }

  IEnumerator ExampleCoroutine()
  {
    print("Launch in T minus 3 seconds");
    yield return new WaitForSeconds(1);
    print("Launch in T minus 2 seconds");
    yield return new WaitForSeconds(1);
    print("Launch in T minus 1 seconds");
    yield return new WaitForSeconds(.75f);
    print("Almost there!");
    yield return new WaitForSeconds(.25f);
    print("Go go go");
  }
}
```

When Start is called, the first line is printed ("Launch in T minus 3 seconds") immediatally.  Then we 'yield return' how long until the next line should be excuted.

'yield' before the return is a special C# keyword used with enumerators.  It is marking your location in the method, allowing another class (in this example, Unity's internal logic), to resume the method from where it left off.

WaitForSeconds is a Unity class used to define how long before the enumerator should be resumed.  There are similar classes available: WaitForSecondsRealtime, WaitForEndOfFrame, WaitForFixedUpdate, WaitUntil, and WaitWhile to give you more control over when the Coroutine is resumed.

Coroutines may be canceled before it's complete by calling StopCoroutine or StopAllCoroutines.  When a GameObject is destroyed, any Coroutines it had started are stopped.

</details>
<details><summary>What does Instantiate do?</summary>

Instantiate clones a GameObject or prefab, creating a new GameObject in the scene.  There are a few variations of the call you could use.

To clone using the original's Transform (position, rotation, scale):
```csharp
Instantiate(thingToSpawn);
```

To clone and set a position and rotation:
```csharp
Instantiate(thingToSpawn, Vector3.zero, Quaternion.identity);
```

To clone and set a parent for this GameObject:

```csharp
Instantiate(thingToSpawn, gameObject);
```

</details>
<details><summary>How do you choose a random number?</summary>

Unity provides a convientent static class for getting random data.  For example:

```csharp
float randomNumber0To1 = UnityEngine.Random.value;
float randomNumberNeg10p5ToPos5 = UnityEngine.Random.Range(-10.5f, 5f);
Quaternion randomRotation = UnityEngine.Random.rotation;
```

How is [UnityEngine.Random](https://docs.unity3d.com/ScriptReference/Random.html) different from System.Random?

In addition to providing APIs which are convenient for games (such as .rotation), the UnityEngine.Random is accessed statically while the System.Random requires you to create an object first.

Since the Unity random class has the same name as the System random class, I try to consistently use the fully qualified name like this:

```csharp
UnityEngine.Random.Range(-1f, 1f);
```

The reason for this is if you have 'using System' in the file, the compile may throw an error.  For example:

```csharp
using System;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
  protected void Start()
  {
    // This line is a compile error
    float randomNumber = Random.Range(-1f, 1f); 

    // This line works correctly
    float randomNumber = UnityEngine.Random.Range(-1f, 1f);
  }
}
```

</details>
<details><summary>What does Debug.Assert do?</summary>

Debug.Assert is a used to confirm an assumption your code is making.  If the assumption does not hold (i.e. if the contents of the Debug.Assert evaluate to false), then the assert fails and an error is presented in the Unity console for you to investigate.

```csharp
Debug.Assert(confirmThisIsTrue);
```

You can optionally include a message to be displayed when the assert fails.  e.g.:

```csharp
Debug.Assert(confirmThisIsTrue, "confirmThisIsTrue must be true");
```

Debug.Assert is there to help identify problems sooner.  If the assert fails it does not prevent other code from being executed - however you can select 'Error Pause' in the 'Console' to better see what is happening at that moment.

Debug.Assert does not execute in release / the built version of your game.  In other words there is no performance impact to your final game by including these checks.

TODO pre conditions and post conditions

</details>

## Update the collision matrix

Add an Enemy layer for the balls and change the collision matrix to allow them to travel through other enemies in the world.

<details open><summary>How</summary>

 - Edit -> Project Settings -> Tags and Layers.
 - Under 'Layers' add "Enemy" to one of the empty 'User Layer' slots.

<img src="http://i.imgur.com/spZG3NZ.png" width=100px />

 - Select the "Spike Ball" prefab under the 'Project' tab Assets/Prefabs.
 - In the inspector, click the dropdown next to 'Layer' in the top right and select "Enemy".

 <img src="http://i.imgur.com/KPvq22a.png" width=150px />

 - Edit -> Project Settings -> Physics 2D.
 - Under the 'Layer Collision Matrix', uncheck the box where "Enemy" meets "Enemy".
   - If you hover to confirm, a tooltip should appear saying 'Enemy / Enemy'.

 <img src="http://i.imgur.com/JkjXpZN.png" width=150px />

</details>
<details><summary>What's a Layer and how's it different from a Tag?</summary>

A layer is a number representing a category or type of object in your game which may be compared to a LayerMask.  The Unity editor allows you to associate a string with this value as well for convienence.  Layers can be used to effeciently include or exclude objects based off of their type.  For this reason, the physics matrix in Unity works with layers.

To determine if a layer is included with in a LayerMask, you can do it like the following example.  Comparing to a LayerMask uses 'bit shifting' and a 'bitwise and' which are not intuitive.  Later in the tutorial we'll create an extension method so we don't have to look at this ever again.

```csharp
using UnityEngine;

public class MyComponent : MonoBehaviour
{
  protected void Start()
  {
    LayerMask mask = LayerMask.GetMask(new[] { "Water", "UI" });
    if((mask.value & 1 << gameObject.layer) > 0)
    {
      // This gameObject is included in the LayerMask
    } 
    else
    {
      // This gameObject is NOT in the LayerMask
    }
  } 
}
```

A tag is also a way of categorizing objects, but by string.  It's useful for more targeted use cases, such as identifying the MainCamera and the Player.  

To check the tag, use CompareTag as shown here:

```csharp
using UnityEngine;

public class MyComponent : MonoBehaviour
{
  protected void Start()
  {
    if(gameObject.CompareTag("Player"))
    {
      // This gameObject is a Player
    }
    else
    {
      // This gameObject is NOT a Player
    }
  }
}
```

Every GameObject has both one layer and one tag.

</details>
<details><summary>What does the collision matrix impact?</summary>

The collision matrix defines which GameObjects may collide with what other GameObjects, based off of the GameObjects' layers.

A checked box indicates that collisions are supported.  Uncheck to disable collisions between those layers.  When unchecked, collisions between GameObjects with those layers are completely disabled - allowing objects to pass through each other as if the other didn't exist.  

Every possible combination of layers is exposed as a checkbox in settings, including a layer coming in contact with itself.  Remember that layers are defining a category or object type, so by disabling the 'Enemy' layer from coming in contact with itself - we are preventing one ball from colliding with another in the world while still allowing them to roll over platforms.

</details>


## Test!

That's it for chapter 1!  Your game should now look a lot like the gif at the top.  You can compare to our  [demo build](https://hardlydifficult.com/PlatformerTutorialPart1/index.html) and review the [Unity Project / Source Code for Chapter 1](https://github.com/hardlydifficult/Unity2DPlatformerTutorial/tree/Part1). 

Additionally to review, you may want to:
 - Try adjusting the variables in Spawner to get a reasonable flow of enemies.
 - Try adjusting the initial velocity values for the spike ball.
   - Consider adding randomness to these values as well.
 - Try adjusting the bumper position angles so balls return to the screen promptly / smoothly. 
 - Try adjusting the size of colliders, ensure that objects appear to be touching the ground reasonably.
 - Cut a test build and try it outside of the Unity editor environment.






== 

TODO paypal, patreon, and twitch links.