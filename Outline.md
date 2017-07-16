# 2D Platformer Tutorial for Unity 2017

This is very much a WIP.  I'm trying to make a tutorial helpful to a range of experience levels.  Please let me know if you have any suggestions - I'm creating this live at [twitch.tv/HardlyDifficult](https://twitch.tv/HardlyDifficult) or email nick@HardlyDifficult.com

[TODO some intro and link to the demo]
[TODO image size how to manage this well????]


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
<details><summary>What's a sprite sheet?</summary>

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

A GameObject is something which appears in the game's Hierarchy.  Every GameObject has a transform.  It may also hold other GameObjects and it may includes various components.  

<hr></details>
<details><summary>What's a Component?</summary>

A component is a set of logic (i.e. code) which may be added to a GameObject and is exposed in the Inspector.  A GameObject may have any number of components and those components may expose various properties to customize the behaviour for a specific object.  Unity has a number of components available out of the box, we will be using several Unity components in this tutorial and will be making many custom components as well.

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

Note: there may be visual artifacts which we will address below.

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



## Disable Anti Aliasing

Update "Quality" settings, disabling Anti-Aliasing to prevent some visual artifacts that happen when using sprite sheets.

<details><summary>How</summary>

<img src="http://i.imgur.com/omFI4DD.png" width=50% />
Not different levels for different build types

<hr></details>
<details><summary>Why</summary>

<img src="http://i.imgur.com/vY5YmVj.png" width=50% />

<hr></details>





## Sprite Mesh Type: Full Rect

Update the sprite sheet's import settings to use mesh type full rect since we will be using tiling.

<details>
<summary>
How
</summary>
<img src="http://i.imgur.com/Dhe3Nzt.png" width=50% />
<hr></details>

<details>
<summary>
Why
</summary>
Prevents artifacts when creating tiled sprites.
<img src="http://i.imgur.com/e9jE83B.png" width=50% />
<hr></details>





## Sprite Filter Mode: Point

Update the sprite sheet's import settings to use filter mode point, again prevent some visual artifacts that appear when using sprite sheets.

<details>
<summary>
How
</summary>
 - Set Mesh Type to Full Rect
<img src="http://i.imgur.com/B0nqf75.png" width=50% />
<hr></details>


<details>
<summary>
Why
</summary>
Random lines will show up on screen without this
<img src="http://i.imgur.com/ZKqg5JP.png" width=50% />
<hr></details>









## Set a 5:4 Aspect ratio

On the game tab, change the aspect ratio to 5:4 as we want to simplify and always show the same amount of the world on screen.

<details>
<summary>How</summary>

<img src="http://i.imgur.com/MTnZtu4.png" width=50% />
<hr></details>
<details>
<summary>Why</summary>
Challenge of aspect ratios is different ratios see different amounts of the world.  This is a fixed screen game so we choose an arbitrary target to design for.  When building, we can select specific resolutions to support.

When laying the scene for an aspect ratio, it will automatically scale for different resolutions.
<hr></details>





## Camera Size: 10

Select the camera in the scene and update the size to 10, effectively zooming out a bit.

<details>
<summary>How</summary>
<img src="http://i.imgur.com/PmeoqG7.png" width=50% />
<hr></details>

<details>
<summary>Why</summary>
This defines how much of the world is visible vertically.  Than the aspect ratio determines how much to display horizontally.

With the two locked, we can design a scene without any camera movement and be sure everyone has the same experience.
<hr></details>




## Create Platform with child sprites

Create a new parent game object for the child.


<details>
<summary>How</summary>

 - Right click in "Hierarchy" and "Create Empty"
 - Rename to 'Platform'
 - Ensure the transform is at defaults (position 0, rotation 0, scale 1)

<img src="http://i.imgur.com/FAkZf1H.png" width=50% />

 - Drag and drop the sprite onto 'Platform' (it should appear indented under 'Platform' and also have a default transform)
 

<img src="http://i.imgur.com/UB6JDgt.png" width=50% />
<hr></details>

<details>
<summary>Why</summary>
aoeu
<hr></details>



## Add edges

Add sprites with rounded edges to the left and right of the platform.

<details>
<summary>
How?
</summary>

 - Rename the gameObject to 'PlatformWithEdges'
 - Drag drop into the Prefabs folder to create a new prefab

 - Drag the edge sprites into the hierarchy under the 'PlatformWithEdges' gameObject 
 -- When you do this, it will warn you that you will 'break' the prefab
 - Vertex snap by holding V, a box appears for each anchor point.  Hover over the top right and click and drag the box which appears.  It will snap perfectly with other anchor points in the world
 - Apply prefab

<img src="http://i.imgur.com/GNMGb0w.gif" width=50% />
 - Apply prefab
 <hr></details>



<details>
<summary>How</summary>
 - Rename
  - Drag drop new prefab
  - Delete one side
  - Drag in a new copy of PlatformWithEdges and repeat for the other side

Should have a total of 4 prefabs.
You can delete them all from the scene.

<img src="http://i.imgur.com/j1cz0aZ.png" width=50% />
<hr></details>

<details>
<summary>
Why?
</summary>
When something on the prefab changes we can revert the instances in the scene.  This applies any new settings or components we may have added without disturbing the transform it uses in the scene.  Unfortunately it would also reset things like the missing edge sprites - so one for each.
<hr></details>

## Create a connected platform

<details><summary>How?</summary>

Copy paste, delete the edges we don't need
 - Use a copy of PlatformWithRightEdge and PlatformWithLeftEdge


Adjust the 

 - Drag and drop the prefab to the hierarchy to instanciate a copy
 - Position side by side
 - Remove edges so each prefab can touch the other's middle segment
 (i.e. no rounded corner)


 - Rotations on the z axis (2d doesn't respond well to x or y rotations --- but remember that Unity is a 3D engine)
 - Use the tile mode width for the middle segment only

<hr></details>


## Copy paste to complete Level1 platforms

<details><summary>How?</summary>

When moving things around be sure the parent is selected.

Aim to have platforms extending off the screen a bit


Rinse and repeat of steps x,y,z
 - Note the white box in the scene view shows what will be visible in game.

 Can also look at the game tab anytime to see what the camera sees

<hr></details>
 


## Debugging

<details><summary>aoeu?</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

<hr></details>

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




## Add a character


## Enable gravity

## Colliders on platform parents

## Move left/right

## Jump

## Add Platformer Effect to platforms




























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
