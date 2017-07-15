
# Create a new 2D project

<details>
<summary>How</summary>
https://store.unity.com/download


<img src="https://i.imgur.com/T2iZrmK.png" width=50% /></details>

<details>
<summary>Why</summary>
Unity is a 3D engine.  2D just sets default settings.
</details>




Auto save script



# Create a platform with collider to walk on

Goal: Create the platforms for level 1

## Import art


<details>
<summary>How</summary>
<img src="https://i.imgur.com/lvN6QmZ.png" width=20% />

 - Right click in the Project Assets directory
 - Create new folder
   - You can rename folders by selecting and pressing F2
 - Drag/drop the sprite sheet (or entire folder of art) into the folder you just created


</details>

<details>
<summary>Why</summary>
aoeu
</details>

## Slice sprite sheet

<details>
<summary>How</summary>

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
<details>
<summary>Why</summary>
Full Rect is needed for the tiling effect we will be applying to platform sprites.
</details>


## Drag sprite into scene

<details>
<summary>
How
</summary>
<img src="http://i.imgur.com/E2lLY3h.png">

 - Click the arrow on the spritesheet in your Assets/Art directory (this displays each individual sliced image)
 - Click and drag the platform sprite you want to use into the Hierarchy

</details>


## Disable Anti Aliasing
<details>
<summary>
How
</summary>
<img src="http://i.imgur.com/omFI4DD.png">
Not different levels for different build types
</details>
<details>
<summary>
Why
</summary>
<img src="http://i.imgur.com/vY5YmVj.png">
</details>




## Sprite Mesh Type: Full Rect

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


<details>
<summary>How</summary>
<img src="http://i.imgur.com/PmeoqG7.png">
</details>



<details>
<summary>Why</summary>
This defines how much of the world is visible vertically.  Than the aspect ratio determines how much to display horizontally.

With the two locked, we can design a scene without any camera movement and be sure everyone has the same experience.
</details>



<details>
<summary>Additional considerations</summary>
 - Background color
</details>


## Create Prefab


<details>
<summary>How</summary>

 - Right click in "Hierarchy" and "Create Empty"
 - Rename to 'Platform'
 - Ensure the transform is at defaults (position 0, rotation 0, scale 1)

<img src="http://i.imgur.com/FAkZf1H.png">

 - Drag and drop the sprite onto 'Platform' (it should appear indented under 'Platform' and also have a default transform)
 - Create a 'Prefabs' directory under "Project" -> "Assets"
 - Drag the 'Platform' object from the "Hierarchy" to the 'Prefabs' directory.

<img src="http://i.imgur.com/UB6JDgt.png">
</details>

<details>
<summary>Why</summary>
Allows us to modify all platforms in the game easily.
</details>



## Add Edge sprites to prefab
 - Drag the edge sprites into the hierarchy under the 'Platform' gameObject
 - Vertex snap by holding V, a box appears for each anchor point.  Hover over the top right and click and drag the box which appears.  It will snap perfectly with other anchor points in the world

<img src="http://i.imgur.com/MYqFzYq.gifv">

http://i.imgur.com/CnC6ZXA.png

 - Apply prefab




## Add a copy of the prefab


## Modify each copies transform / width
 - Rotations
 - Use the tile mode width for the middle segment only

## Go nuts, make the level





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