This is very much a WIP.  I'm trying to make a tutorial helpful to a range of experience levels.  Please let me know if you have any suggestions - I'm creating this live at twitch.tv/HardlyDifficult or email nick@HardlyDifficult.com






# Create a new 2D project

<details><summary>How</summary>

 - [Download Unity](https://store.unity.com/download), the free Personal edition has everything you need. 
 - Select "2D" when creating a new project.

<img src="https://i.imgur.com/T2iZrmK.png" width=50% />

*The new project screen*


</details>
<details><summary>Why 2D?</summary>

Presenting the 2D vs 3D option when you create a new project suggests this is a significant choice.  It's not really... 2D just changes default settings on things like your camera.   Unity is a 3D engine, when creating 2D games your actually creating a 3D world where everything is very flat but the camera looks straight ahead and the only rotation in the world is around the z axis.  

</details>





## Save scene

<details><summary>How</summary>

File -> Save Scene
Create a Scenes directory, call it Level1

</details>
<details><summary>What's a scene?</summary>

The Scene represents a collection of game objects and components configured for a game level or menu screen.  For this tutorial we are starting by creating part of Level 1.  Level 2, the menu, and other UI screens will be saved as separate scenes.  You can switch scenes via the SceneManager, and will cover this later in the tutorial.

</details>




## Auto save script

Unity may crash.  I recommend adding an editor script which automatically saves everytime you hit play.

<details><summary>How</summary>

An editor script like this must be saved in Assets under a folder named "Editor".  

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Auto saves the scene and project everytime you click play.
/// 
/// This happens before the game runs so even if that run causes a crash, your work is safe.
/// </summary>
[InitializeOnLoad]
public class AutoSave
{
  /// <summary>
  /// Called automatically c/o InitializeOnLoad.  Registers for play mode events.
  /// </summary>
  static AutoSave()
  {
    EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
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

</details>
<details><summary>Why?</summary>

It's not unusual to see Unity crash several times per day.  This script has been a lifesaver.  It's saved me hours that would have been wasted re-configuring gameObjects.

As an editor script, this logic is not included in the game you release.  

Editor folders --- general special folder name weirdness in Unity.  Could be Assets/Editor/AutoSave.cs or Assets/Code/Editor/Utils/AutoSave.cs

</details>





# Create platforms 

Goal: Create the platforms for level 1





## Import art

Add your sprite image files to the Asset directory.

<details><summary>How</summary>

<img src="https://i.imgur.com/lvN6QmZ.png" width=20% />

 - Right click in the Project Assets directory
 - Create new folder
   - You can rename folders by selecting and pressing F2
 - Drag/drop the sprite sheet (or entire folder of art) into the folder you just created


</details>
<details><summary>Why</summary>

aoeu

</details>





## Slice sprite sheet

Slice the sprite sheet in order to access each sprite within.

<details><summary>How</summary>

<img src="http://i.imgur.com/duYuVMy.png">

- Set Sprite Mode to Multiple
- Click Sprite Editor (apply changes when prompted)

<img src="http://i.imgur.com/hA2cMfv.png">

- Click the "Slice" menu item
  - Type: Grid By Cell Count
  - Column & Row: 8 & 16
- Click "Slice" button
- Click "Apply" and close the Sprite Editor

</details>
<details><summary>Why</summary>

Full Rect is needed for the tiling effect we will be applying to platform sprites.

</details>





## Drag sprite into scene

Add a sprite to the scene representing the middle segment of a platform.

<details><summary>How</summary>

<img src="http://i.imgur.com/E2lLY3h.png">

 - Click the arrow on the spritesheet in your Assets/Art directory (this displays each individual sliced image)
 - Click and drag the platform sprite you want to use into the Hierarchy

</details>





## Disable Anti Aliasing

Update "Quality" settings, disabling Anti-Aliasing to prevent some visual artifacts that happen when using spritesheets.

<details><summary>How</summary>

<img src="http://i.imgur.com/omFI4DD.png">
Not different levels for different build types

</details>
<details><summary>Why</summary>

<img src="http://i.imgur.com/vY5YmVj.png">

</details>





## Sprite Mesh Type: Full Rect

Update the sprite sheet's import settings to use mesh type full rect since we will be using tiling.

<details>
<summary>
How
</summary>
<img src="http://i.imgur.com/Dhe3Nzt.png">
</details>

<details>
<summary>
Why
</summary>
Prevents artifacts when creating tiled sprites.
<img src="http://i.imgur.com/e9jE83B.png">
</details>





## Sprite Filter Mode: Point

Update the sprite sheet's import settings to use filter mode point, again prevent some visual artifacts that appear when using sprite sheets.

<details>
<summary>
How
</summary>
 - Set Mesh Type to Full Rect
<img src="
http://i.imgur.com/B0nqf75.png">
</details>


<details>
<summary>
Why
</summary>
Random lines will show up on screen without this
<img src="http://i.imgur.com/ZKqg5JP.png">
</details>





## Sprite Draw Mode: Tiled

In the scene, update the sprite to draw mode tiled and change the width so we can begin to design the level.

<details>
<summary>How</summary>
 - Draw Mode: Tiled
 - Width: 10-ish, no change to height
<img src="http://i.imgur.com/MIgzjdO.png">
</details>
<details>
<summary>Why</summary>
TODO
For tiling vs stretching.
</details>





## Set a 5:4 Aspect ratio

On the game tab, change the aspect ratio to 5:4 as we want to simplify and always show the same amount of the world on screen.

<details>
<summary>How</summary>

<img src="http://i.imgur.com/MTnZtu4.png">
</details>
<details>
<summary>Why</summary>
Challenge of aspect ratios is different ratios see different amounts of the world.  This is a fixed screen game so we choose an arbitrary target to design for.  When building, we can select specific resolutions to support.

When laying the scene for an aspect ratio, it will automatically scale for different resolutions.
</details>





## Camera Size: 10

Select the camera in the scene and update the size to 10, effectively zooming out a bit.

<details>
<summary>How</summary>
<img src="http://i.imgur.com/PmeoqG7.png">
</details>

<details>
<summary>Why</summary>
This defines how much of the world is visible vertically.  Than the aspect ratio determines how much to display horizontally.

With the two locked, we can design a scene without any camera movement and be sure everyone has the same experience.
</details>




## Create Platform with child sprites

Create a new parent game object for the child.


<details>
<summary>How</summary>

 - Right click in "Hierarchy" and "Create Empty"
 - Rename to 'Platform'
 - Ensure the transform is at defaults (position 0, rotation 0, scale 1)

<img src="http://i.imgur.com/FAkZf1H.png">

 - Drag and drop the sprite onto 'Platform' (it should appear indented under 'Platform' and also have a default transform)
 

<img src="http://i.imgur.com/UB6JDgt.png">
</details>

<details>
<summary>Why</summary>
aoeu
</details>



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

<img src="http://i.imgur.com/GNMGb0w.gif">
 - Apply prefab
 </details>



<details>
<summary>How</summary>
 - Rename
  - Drag drop new prefab
  - Delete one side
  - Drag in a new copy of PlatformWithEdges and repeat for the other side

Should have a total of 4 prefabs.
You can delete them all from the scene.

<img src="http://i.imgur.com/j1cz0aZ.png">
</details>

<details>
<summary>
Why?
</summary>
When something on the prefab changes we can revert the instances in the scene.  This applies any new settings or components we may have added without disturbing the transform it uses in the scene.  Unfortunately it would also reset things like the missing edge sprites - so one for each.
</details>

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

</details>


## Copy paste to complete Level1 platforms

<details><summary>How?</summary>

When moving things around be sure the parent is selected.

Aim to have platforms extending off the screen a bit


Rinse and repeat of steps x,y,z
 - Note the white box in the scene view shows what will be visible in game.

 Can also look at the game tab anytime to see what the camera sees

</details>
 


## Debugging

<details><summary>aoeu?</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

</details>

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
<img src="http://i.imgur.com/nWDCAwX.png">
For PC, we can select specific supported aspect ratios 
<img src="http://i.imgur.com/Xoxw0Xs.png">
</details>