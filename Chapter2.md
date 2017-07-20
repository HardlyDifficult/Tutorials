


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

An animat**or** controls which animations should be played at any given time.  An animator uses an animator controller which is a state machine used to select animations.

A state machine is a common pattern in development where logic is split across several states.  The state machine selects one primary state which owns the experience until the state machine transitions to another state.  Each animator state has an associated animation to play.  When you transition from one state to another, Unity switches from one animation to the next.  

We will be diving into more detail about animations and animators later in the tutorial.  

<hr></details>


## Add a rigidbody and CapsuleCollider2D

Add Rigidbody2D and CapsuleCollider2D components to the character to enable gravity.  Adjust the collider size as needed.

<details open><summary>How</summary>

 - Select the Character's GameObject.
 - In the 'Inspector', click 'Add Component' and select "Rigidbody2D".
 - Click 'Add Component' and select "CapsuleCollider2D".
 - Click 'Edit Collider' and adjust to fit the character.
   - Click and then hold Alt while adjusting the sides to pull both sides in evenly.

<img src="http://i.imgur.com/KFwBZeo.gif" width=100px />

Hit play and the character should now land on a platform... but may fall over:

<img src="http://i.imgur.com/T0fdwa1.gif" width=150px />

</details>

<details><summary>How do I know what size to make the collider?</summary>

The collider does not fit the character perfectly, and that's okay.  In order for the game to feel fair for the player we should lean in their favor.  When designing colliders for the character and enemies, we may prefer to make the colliders a little smaller than the sprite so that there are no collisions in game which may leave the player feeling cheated.

Because the character is constantly in motion, and its limbs are in different positions, the collider won't always fit the character. For that reason we use a collider focused around the body which works for the different character stances.

In addition to killing the character when he comes in contact with an enemy, the collider is used to keep the character on top of platforms.  For this reason it's important that the bottom of the collider aligns with the sprite's feet.

</details>
<details><summary>Why not use a collider that outlines the character?</summary>

Bottom line, it's not worth the trouble.  Unity does not provide good tools for more accurate collisions on animating sprites.  Implementing this requires a lot of considerations and may be difficult to debug.

Most of the time the collisions in the game would not have been any different if more detailed colliders were used.  Typically 2D games use an approach similiar to what this tutorial recommends. It creates a good game feel and the simplifications taken have become industry standard.

</details>




## Freeze rotation

Freeze the character's rotation so he does not fall over.

Note: The character will stand straight up even on slanted platforms.  This will be addressed below when we write the movement controllers for the character.

<details open><summary>How</summary>

 - Select the character.
 - In the Rigidbody2D component, expand 'Constraints'.
 - Check 'Freeze Rotation'.

<img src="http://i.imgur.com/uXxDSwD.png" width=128px />

</details>




## Move left/right

aoeu

Note the character will always be looking right, even while walking left.  


<details open><summary>How</summary>

TODO

Create a WalkMovement with desiredWalkDirection and movementSpeed.  Set myBody.velocity.
Create a PlayerController which desiredWalkDirection = Input.GetAxis("Horizontal").

Start character at top and walk down level.

 - Create a C# script "WalkMovement" under Assets/Code/Components/Movement.
 - Select the Character GameObject and add the WalkMovement component.
 - Paste in the following code:
 
 ```csharp
 using UnityEngine;
using System;

/// <summary>
/// Controls the entity's walk movement.
/// 
/// Another component drives walk via desiredWalkDirection.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class WalkMovement : MonoBehaviour
{
  /// <summary>
  /// Set by another component to inform this component 
  /// it should walk. Positive to walk right, negative to 
  /// walk left.  The magnitude is the walk speed.
  /// </summary>
  [NonSerialized]
  public float desiredWalkDirection;

  /// <summary>
  /// Used to control movement.
  /// </summary>
  /// <remarks>
  /// Cached here for performance.
  /// </remarks>
  Rigidbody2D myBody;
  
  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();

    Debug.Assert(myBody != null);
  }

  /// <summary>
  /// A Unity event, called every x ms of game time.
  /// 
  /// Adds velocity to the rigidbody to move it horizontally.
  /// </summary>
  /// <remarks>
  /// With this approach, forces may not be used to impact the 
  /// X on this entity.  E.g. if we wanted a fan which slowly 
  /// pushed characters to the left, the force added would be 
  /// lost here.  This matches the Unity example character asset 
  /// as enabling forces on both dimentions cause movement to 
  /// feel strange or leads to other experience problems which 
  /// quickly complicate the code (but possible of course, 
  /// just thing things through).
  /// </remarks>
  protected void FixedUpdate()
  {
    // Calculate the desired horizontal movement given 
    // the input desiredWalkDirection.
    float desiredXVelocity 
      = desiredWalkDirection * Time.fixedDeltaTime;

    // Any y velocity is preserved, this allows gravity
    // to continue working.
    myBody.velocity = new Vector2(desiredXVelocity, myBody.velocity.y);
  }
}
```

 - Create a C# script "PlayerController" under Assets/Code/Components/Movement.
 - Select the Character GameObject and add the PlayerController component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Wires up user input, allowing the user to 
/// control the player in game with a keyboard.
/// </summary>
[RequireComponent(typeof(WalkMovement))]
public class PlayerController : MonoBehaviour
{
  /// <summary>
  /// A multiple to increase/decrease how quickly
  /// the object walks.
  /// </summary>
  [SerializeField]
  float walkSpeed = 100; 

  /// <summary>
  /// Used to cause the object to walk.
  /// </summary>
  /// <remarks>
  /// Cached here for performance.
  /// </remarks>
  WalkMovement walkMovement;

  /// <summary>
  /// A Unity event, called once before the GameObject
  /// is instantiated.
  /// </summary>
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    Debug.Assert(walkMovement != null);
  }

  /// <summary>
  /// A Unity event, called every x ms of game time.
  /// 
  /// Consider moving.
  /// </summary>
  /// <remarks>
  /// Moving uses an input state, and therefore may be captured 
  /// on Update or FixedUpdate, we use FixedUpdate since physics 
  /// also runs on FixedUpdate, so trying to do this on update would
  /// require an extra cache (w/o benefit).
  /// </remarks>
  protected void FixedUpdate()
  {
    // Consider moving left/right based off keyboard input.
    walkMovement.desiredWalkDirection 
      = Input.GetAxis("Horizontal") * walkSpeed;
  }
}
```

</details>
<details><summary>What is an Input 'Axis' and how are they configured?</summary>

Unity offers several ways of detecting keyboard/mouse/controller input.  'Axis' is the recommended approach.  Each input Axis may be configured in the inspector:

 - Edit -> Project Settings -> Input.
 - In the 'Inspector', you will find a list of supported input types.

<img src="http://i.imgur.com/T2BJvBm.png" width=100px />

You can add, remove, rename, and configure the inputs for your game.  Inputs may also be reconfigured by the player at runtime.  For more information about the various options, see [Unity's description of the InputManager](https://docs.unity3d.com/Manual/class-InputManager.html).  We will be using the defaults for this tutorial.

</details>
<details><summary>Why FixedUpdate instead of Update?</summary>

Update occurs once per rendered frame.  FixedUpdate occurs at a regular interval, every x ms of game time.  FixedUpdate may run 0 or more times each frame.

FixedUpdate is preferred for mechanics which require some level of consistency or apply changes incrementally.  Physics in Unity are processed in FixedUpdated.  So when manipulating physics for the game such as we are here by changing velocity on the rigidbody, we do this on FixedUpdate to match Unity's expectatations. 


</details>
<details><summary>Why multiple by Time.fixedDeltaTime?</summary>

It's optional. Anytime you make a change which includes some speed, such as walking, we multiply by the time elapsed so motion is smooth even when the frame rate may not be.  While using FixedUpdate, the time passed between calls is always the same - so Time.fixedDeltaTime is essentially a constant.  

If speed is beinging processed in an Update, you must multiply by Time.deltaTime for a smooth experience.  While in FixedUpdate, you could opt to not use Time.fixedDeltaTime, however leaving it out may lead to some confusion as fields which are configured for FixedUpdate may have a different order of magnitude than fields configured for use in Update.

Additionaly you may choose to adjust the time interval between FixedUpdate calls while optimizing your game.  By consistently multiplying by the delta time, you can adjust the interval for FixedUpdate without changing the game play.

</details>
<details><summary>Why set velocity instead of using AddForce?</summary>

AddForce is a way of increasing a rigidbody's velocity instead of manipulating the velocity directly.

You could use AddForce instead.  Maybe give it a try and see how it feels.  Adding force instead of setting velocity will create a different game feel, in terms of how objects accelerate.


</details>
<details><summary>Why not combine these into a single class?</summary>

As discussed in chapter 1, Unity encourages a component based solution.  This means that we attempt to make each component focused on a single mechanic for feature.  Doing so simplifies debugging and enables reuse.  For example, we will be creating another enemy type which will use the same WalkMovement component created for the character above.

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







==

## Rotate to match the floor's angle

Create a Feet with isGrounded and Quaternion floorRotation. 
Create a RotateToMatchFloorWhenGrounded

## Restrict movement to stay on screen

Create KeepWalkMovementOnScreen





- facing direction (flip sprite)
- walk speed (maybe with jump)
 - kill player


# Add the spike ball enemy


Create KillOnContactWithPlayer

DieOnBumpers?

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



## Debugging

<details><summary>TODO</summary>

* Check the children gameObjects in the prefab.  They should all be at 0 position (except for the edge segments which have an x value), 0 rotation, and 1 scale.

<hr></details>

TODO link to web build and git / source for the example up to here





 - Ladders
 - Kill on bumpers when below character
 - Death effect (when killed via hammer)
 - Suicide when player dies (like restart level effects)


