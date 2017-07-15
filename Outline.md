
# Create a new 2D project

<details>
<summary>How</summary>
https://store.unity.com/download


<img src="https://i.imgur.com/T2iZrmK.png" width=50% /></details>

<details>
<summary>Why</summary>
Unity is a 3D engine.  2D just sets default settings.
</details>





# Create a platform with collider to walk on


## Import sprite art

<details>
<summary>How</summary>
<img src="https://i.imgur.com/lvN6QmZ.png" width=20% />

 - Right click in the Project Assets directory
 - Create new folder
   - You can rename folders by selecting and pressing F2
 - Drag/drop the sprite sheet (or entire folder of art) into the folder you just created
</ul>

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

## Update sprite sheet Mesh Type to Full Rect

<details>
<summary>
How
</summary>
 - Set Mesh Type to Full Rect
<img src="http://i.imgur.com/Dhe3Nzt.png">
</details>

<details>
<summary>
Why
</summary>
Prevents artifacts when creating tiled sprites.
http://i.imgur.com/e9jE83B.png
</details>

## Update sprite sheet Filter Mode to Point

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


## Add a platform sprite to the scene
<img src="http://i.imgur.com/E2lLY3h.png">
<li>Click the arrow on the spritesheet in your Assets/Art directory (this displays each individual sliced image)
<li>Click and drag the platform sprite you want to use into the Hierarchy

## Change draw mode to Tiled and extend Width

<details>
<summary>How</summary>
<li>Draw Mode: Tiled
<li>Width: 10-ish, no change to height
http://i.imgur.com/MIgzjdO.png
</details>
<details>
<summary>Why</summary>
TODO

