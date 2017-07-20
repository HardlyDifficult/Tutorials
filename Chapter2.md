
TODO review pics given part 1 changes


# 2) Add a Character and Movement Mechanics

Add a character to the scene.  Have him walk and jump, creating a basic platformer.


## Configure the character sprite sheet

Add a sprite sheet for the character, slice it with a bottom pivot and set to point filter mode.  We are using [Kenney.nl's Platformer Characters](http://kenney.nl/assets/platformer-characters-1) 'PNG/Adventurer/adventurer_tilesheet.png'.


<details><summary>How</summary>

 - Drag/drop the sprite sheet into Assets/Art.
 - Set 'Sprite Mode: Multiple'.
 - Click 'Sprite Editor' 
   - Cell Count, 9 rows 3 columns.
   - Pivot: Bottom
 - Set the 'Filter Mode: Point (no filter)'.

<img src="http://i.imgur.com/BuIsVWD.png" width=50% />

Note we won't be tiling the character sprite, so the default of Mesh Type: Tight is okay.

</details>
<details><summary>What's Pivot do?</summary>

A pivot point is the main anchor point for the sprite.  By default, pivot points are the center of the sprite.  

For the character we are moving the pivot point to the bottom.  This allows us to position and rotate the character starting at the feet / the bottom of the sprite.  

Here's an example showing a character with a default center and one with the recommended bottom pivot.  They both have the same y position.  Notice the the vertical position of each as well as how the rotation centers around the different pivot points:

<img src="http://i.imgur.com/AQY4FOT.gif" width=50% />

The pivot point you select is going to impact how we create animations and implement movement mechanics.  The significance of this topic should become more clear later in the tutorial.

</details>



## Add Character to the Scene with a Walk Animation

Drag the sprites for walking into the Hierarchy to create a Character and animation.  We are using adventurer_tilesheet_9 and adventurer_tilesheet_10.

<details><summary>How</summary>

 - Hold Ctrl and select "adventurer_tilesheet_9" and "adventurer_tilesheet_10" sprites from the sprite sheet "adventurer_tilesheet".
 - Drag them into the Hierarchy.
 - When prompted, save the animation as Assets/Animations/CharacterWalk.anim.
 - Rename the GameObject to "Character" (optional).

<img src="http://i.imgur.com/k7bSlCp.gif" width=50% />
 

This simple process created:
 - The character's GameObject.
 - A SpriteRenderer component on the GameObject defaulting to the first selected sprite.
 - An Animation representing those 2 sprites changing over time.
 - An Animator Controller for the character with a default state for the Walk animation.
 - An Animator component on the GameObject configured for the Animator Controller just created.

Click Play to test - your character should be walking (in place)! 

<img src="http://i.imgur.com/2bkJdtS.gif" width=100px />

<hr></details>
<details><summary>What's the difference between Animation and Animator?</summary>

An animat**ion** is a collection of sprites on a timeline, creating an animated effect similiar to a flip book.  Animations can also include transform changes, fire events for scripts to react to, etc to create any number of effects.

An animat**or** controls which animations should be played at any given time.  An animator uses an animator controller which is a state machine used to select animations.

We will be diving into more detail on both of these later in the tutorial.  

<hr></details>
<details><summary>What's a state machine / animator state?</summary>

A state machine is a common pattern in development where logic is split across several states.  The state machine selects one primary state which owns the experience until the state machine transitions to another state.

Each animator state has an associated animation to play.  When you transition from one state to another, Unity switches from one animation to the next.  Later in the tutorial we will trigger animator state changes via code.

<hr></details>


## Change Order in Layer to 2

Update the Character's Order in Layer to 2.

<details><summary>How</summary>

 - Select the Character's GameObject
 - In the 'Inspector', set the SpriteRenderer's 'Order in Layer' to 2.

<img src="http://i.imgur.com/Zhgy28L.png" width=50% />

</details>
<details><summary>What does Order in Layer do?</summary>

When multiple sprites are overlapping, Order in Layer is used to determine which one is on top of the other.  So if the character sprite has Order in Layer '2' and everything else uses the default Order in Layer '0', the character will always appear in front of other sprites in the world.

Order in Layer may be any int value, positive or negative. Here's an example showing the character with Order in Layer '-1' and with '2'... sitting on a platform which still has the default Order in Layer '0'.

<img src="http://i.imgur.com/QCHPLDf.png" width=50% />

</details>

## Add a Rigidbody2D

Add a Rigidbody2D component to the character to enable gravity.

<details><summary>How</summary>

 - Select the Character's GameObject.
 - In the 'Inspector', click 'Add Component' and select "Rigidbody2D".

Hit play and watch the character fall through the platforms and out of view.

<img src="http://i.imgur.com/ZJPkmt9.gif" width=50px />

</details>


## Add a CapsuleCollider2D 

Add a CapsuleCollider2D component and size it for the character.

<details><summary>How</summary>

 - Select the Character's GameObject.
 - Click 'Add Component' and select "CapsuleCollider2D".
 - Click 'Edit Collider' and adjust to fit the character.
   - Click and then hold Alt while adjusting the sides to pull both side in evenly.

<img src="http://i.imgur.com/KFwBZeo.gif" width=100px />

Until we add colliders on the platforms (in the next section), playing the game will not look any different.

</details>
<details><summary>How do I know what size to make the collider?</summary>

The collider does not fit the character perfectly, and that's okay.  In order for the game to feel fair for the player we should lean in their favor.  When designing colliders for the character and enemies, we may prefer to make the colliders a little smaller than the sprite so that there are no collisions in game which may leave the player feeling cheated.

Because the character is constantly in motion, and its limbs are in different positions, the collider won't always fit the character. For that reason we use a collider focused around the body which works for all positions of the character.

In addition to killing the character when he comes in contact with an enemy, the collider is used to keep the character on top of platforms.  For this reason it's important that the bottom of the collider aligns with the sprite's feet.

</details>
<details><summary>Why not use a collider that outlines the character?</summary>

Bottom line, it's not worth the trouble.  Unity does not provide good tools for more accurate collisions on animating sprites.  Implementing this requires a lot of considerations and may be difficult to debug.

Most of the time the collisions in the game would not have been any different if more detailed colliders were used.  Typically 2D games use an approach similiar to what this tutorial recommends. It creates a good game feel and the simplifications taken have become industry standard.

</details>






## Freeze rotation

Freeze the character's rotation so he does not fall over.

Note: The character will stand straight up even on slanted platforms.  This will be addressed below when we write the movement controllers for the character.

<details><summary>How</summary>

 - Select the character.
 - In the Rigidbody2D component, expand 'Constraints'.
 - Check 'Freeze Rotation'.

<img src="http://i.imgur.com/uXxDSwD.png" width=128px />

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







## Move left/right

Create a WalkMovement with desiredWalkDirection and movementSpeed.  Set myBody.velocity.
Create a PlayerController which desiredWalkDirection = Input.GetAxis("Horizontal").

Start character at top and walk down level.

==

## Rotate to match the floor's angle

Create a Feet with isGrounded and Quaternion floorRotation. 
Create a RotateToMatchFloorWhenGrounded

## Restrict movement to stay on screen

Create KeepWalkMovementOnScreen





- facing direction (flip sprite)
- walk speed (maybe with jump)





-----

# Add the spike ball enemy

Import, create GameObject with a Rigidbody2D and a CircleCollider2D.

oaeu

Create KillOnContactWithPlayer

Create spawner.

DieOutOfBounds.

Create bumpers.

DieOnBumpers.

SuicideWhenPlayerDies.

Fly Guy too?

---------


## Jump

JumpMovement

## Add Platformer Effect to platforms

## Ladders

LadderMovement, for character and spike ball.

-------

# Character Animations
 - Jump
 - Climb
 - Idle
 - Dance

PlayerAnimator
DeathEffectThrobToDeath

# Intro
Character fades in via AppearInSecondsAndFade

+ other intro effects
 - Cloud and animation























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







aoeu


## Debugging

<details><summary>TODO</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

<hr></details>

TODO link to web build and git / source for the example up to here


