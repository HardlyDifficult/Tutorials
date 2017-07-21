


# 2) Add a Character and Movement Mechanics TODO

Add a character to the scene.  Have him walk and jump, creating a basic platformer. TODO

TODO gif, demo build

## Configure the character sprite sheet

Add a sprite sheet for the character, slice it with a bottom pivot and set to point filter mode.  We are using [Kenney.nl's Platformer Characters](http://kenney.nl/assets/platformer-characters-1) 'PNG/Adventurer/adventurer_tilesheet.png'.


<details open><summary>How</summary>

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

Here's an example showing a character with a default 'Center' and one with the recommended 'Bottom' pivot.  They both have the same Y position.  Notice the the vertical position of each as well as how the rotation centers around the different pivot points:

<img src="http://i.imgur.com/AQY4FOT.gif" width=50% />

The pivot point you select is going to impact how we create animations and implement movement mechanics.  The significance of this topic should become more clear later in the tutorial.

</details>



## Add Character to the Scene with a Walk Animation

Drag the sprites for walking into the Hierarchy to create a Character and animation. Change Order in Layer to 2.  We are using adventurer_tilesheet_9 and adventurer_tilesheet_10.

<details open><summary>How</summary>

 - Hold Ctrl and select "adventurer_tilesheet_9" and "adventurer_tilesheet_10" sprites from the sprite sheet "adventurer_tilesheet".
 - Drag them into the Hierarchy.
 - When prompted, save the animation as Assets/Animations/CharacterWalk.anim.
 - In the 'Inspector', set the SpriteRenderer's 'Order in Layer' to 2.
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


















DieOnBumpers?



---------




====

Feet

<details><summary>How does Dot product work?</summary>

The Dot product is a fast operation which can be used to effeciently determine if two directions represented with Vectors are facing the same (or a similiar) way.

In the visualization below, we are rotating two ugly arrows.  These arrows are pointing in a direction and we are using Vector2.Dot to compare those two directions.  The Dot product is shown as we rotate around.

<img src="http://i.imgur.com/XrjcWQm.gif" width=200px />

A few notables about Dot products:

 - '1' means the two directions are facing the same way.
 - '-1' means the two directions are facing opposite ways.
 - '0' means the two directions are perpendicular.
 - Numbers smoothly transition between these points, so .9 means that the two directions are nearly identical.
 - When two directions are not the same, the Dot product will not tell you which direction an object should rotate in order to make them align - it only informs you about how similar they are at the moment.  

For this visualization, we are calculating the Dot product like so:

```csharp
Vector2.Dot(gameObjectAToWatch.transform.up, gameObjectBToWatch.transform.up);
```

</details>

## Prevent double jump


## Rotate to match the floor's angle
(or with jumping)

Create a Feet with isGrounded and Quaternion floorRotation. 
Create a RotateToMatchFloorWhenGrounded

## Ladders

LadderMovement, for character and spike ball.


Fly Guy too
 - Prevent walking into walls?

====



## animation walk speed
 TODO

# Character Animations
 - Jump
 - Climb
 - Idle
 - Dance

PlayerAnimator
Player DeathEffectThrobToDeath
other death effects?

Hammer

# Intro
Character fades in via AppearInSecondsAndFade

+ other intro effects
 - Cloud and animation


======

win condition

end of level / respawn and scene changes

## SuicideWhenPlayerDies.
(or with end of level sequence)

In game UI

ui...

points

=====

Level 2

..















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



## Debugging

<details><summary>TODO</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

<hr></details>

TODO link to web build and git / source for the example up to here



=====

TODO random questions to add
 - Why do some scripts have a check to disable and others do not.
 - GetComponent and the variations.
 - Debugging tip / talk about the defaults in code vs inspector.  Check your values.  Maybe we should be more explicit in the steps.