


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

Create a script 'WalkMovement' to control the rigidbody and a script 'PlayerController' to feed user input to the WalkMovement component.

Note the character will always be looking right, even while walking left.  He can also walk off the screen and push the balls around.  

<details open><summary>How</summary>

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

The character should walk around, but there is clearly work to be done:

<img src="http://i.imgur.com/xOpivgJ.gif" />

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


## Kill the player when he hits a ball

When the player comes in contact with a spiked ball, kill him!

<details open><summary>How</summary>

 - Create a C# script "LayerMaskExtensions" under Assets/Code/Utils.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Provides additional convenience methods for Unity's LayerMask.
/// </summary>
public static class LayerMaskExtensions
{
  /// <summary>
  /// Determines if the layer is part of this layerMask.
  /// </summary>
  /// <param name="mask">
  /// The layer mask defining which layers should be included.
  /// </param>
  /// <param name="layer">
  /// The layer to check against the mask.
  /// </param>
  /// <returns>
  /// True if the layer is part of the layerMask.
  /// </returns>
  /// <remarks>
  /// This method is used to wrap the bit logic below as 
  /// it's not an intuitive read.
  /// </remarks>
  public static bool Includes(
    this LayerMask mask,
    int layer)
  {
    return (mask.value & 1 << layer) > 0;
  }
}
```

 - Create a C# script "KillOnContactWith" under Assets/Code/Components/Death.
 - Select the Spike Ball prefab and add the KillOnContactWith component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Kills anything which collides with this GameObject
/// if the thing that hit us is included in the provided LayerMask.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KillOnContactWith : MonoBehaviour
{
  /// <summary>
  /// Defines which layers will be killed on contact.
  /// </summary>
  [SerializeField]
  LayerMask layersToKill;

  /// <summary>
  /// A Unity event called anytime an object hits 
  /// this GameObject's collider.
  /// 
  /// Consider killing the thing we touched.
  /// </summary>
  /// <param name="collision">
  /// The thing we touched.
  /// </param>
  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    ConsiderKilling(collision.gameObject);
  }

  /// <summary>
  /// Checks if we should kill the object just touched, 
  /// if so Destroy that GameObject.
  /// </summary>
  /// <param name="gameObjectWeJustHit">
  /// The gameObject just touched.
  /// </param>
  void ConsiderKilling(
    GameObject gameObjectWeJustHit)
  {
    // Compare the GameObject's layer to the LayerMask
    if(layersToKill.Includes(gameObjectWeJustHit.layer) == false)
    { // This object gets to live.
      return;
    }

    // Kill it!
    Destroy(gameObjectWeJustHit);
  }
}
```

 - Edit -> Project Settings -> Tags and Layers.
 - Create a custom Layer for 'Player'.
 - Select the Character GameObject and change its Layer to 'Player'.
 - Select the Spike Ball prefab, update 'Layers To Kill' to 'Player'.

<img src="http://i.imgur.com/wrkb3eJ.png" width=100px />

Hit play to watch the player die:

<img src="http://i.imgur.com/gKEl8wE.gif" width=200px />

For now, to test again stop and hit play again.  We'll respawn the player later in the tutorial.


</details>
<details><summary>What is a C# extension method and why use it?</summary>

Extension methods are a way of adding additional methods to a class you don't own.  In this example, Unity has a class LayerMask.  That class does not offer an easy way to determine if a layer is part of that LayerMask.  Using extensions, we are able to create an 'Includes' method that then can be used as if Unity had written it for us.

This allows us to focus on intent and forget the gory details.  For example this statement:

```csharp
if((layersToKill.value & 1 << gameObjectWeJustHit.layer) > 0) 
...
```

Can now be written like so, which should be easier for people to follow.

```csharp
if(layersToKill.Includes(gameObjectWeJustHit.layer)) 
...
```

</details>
<details><summary>What is this '& 1 <<' black magic?</summary>

Bitwise operations... which are beyond the scope of this tutorial.  To learn more, google 'bit shifting' and 'bitwise and'.

</details>



## Rotate the character when he walks the other way

Flip the character's sprite when he switches between walking left and walking right.

<details open><summary>How</summary>

 - Create a C# script "RotateWalkingEntity" under Assets/Code/Components/Movement.
 - Select the character GameObject and add the RotateWalkingEntity component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Rotates an entity based on it's current horizontal velocity.
/// 
/// This causes entities to face the direction they are walking.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class RotateWalkingEntity : MonoBehaviour
{
  /// <summary>
  /// The rotation that's applied when looking left (vs right).
  /// </summary>
  /// <remarks>
  /// Cached here for performance.
  /// </remarks>
  static readonly Quaternion backwardsRotation = Quaternion.Euler(0, 180, 0);

  /// <summary>
  /// Used to control movement.
  /// </summary>
  /// <remarks>
  /// Cached here for performance.
  /// </remarks>
  Rigidbody2D myBody;

  /// <summary>
  /// The direction we are currently walking, 
  /// used to know when we turn around.
  /// </summary>
  /// <remarks>
  /// Defaults to true as our entities are configured facing right.
  /// </remarks>
  bool _isGoingRight = true;

  /// <summary>
  /// The direction we are currently walking.
  /// When changed, flips the rotation so the entity is facing forward.
  /// </summary>
  bool isGoingRight
  {
    get
    {
      return _isGoingRight;
    }
    set
    {
      if(isGoingRight == value)
      { // The value is not changing
        return;
      }

      // Flip the entity
      transform.rotation *= backwardsRotation;
      _isGoingRight = value;
    }
  }

  /// <summary>
  /// A Unity event, called before this GameObject is instantiated.
  /// </summary>
  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    // Check if we are currently facing right
    isGoingRight = Vector2.Dot(Vector2.right, transform.right) > 0;
    Debug.Assert(myBody != null);
  }

  /// <summary>
  /// A Unity event, called each frame.
  /// 
  /// Updates the entities rotation.
  /// </summary>
  protected void Update()
  {
    float xVelocity = myBody.velocity.x;
    // If there is any horizontal movement
    if(Mathf.Abs(xVelocity) > 0.001)
    { 
      // Determine the current walk direction
      // This may rotate the sprite c/o
      // the smart property above.
      isGoingRight = xVelocity > 0;
    }
  }
}
```

</details>

<details><summary>What's a C# smart property?</summary>

In C#, data may be exposed as either a Field or a Property.  Fields are simply data as one would expect.  Properties are accessed in code like a field is, but they are capable of more.

In this example, when isGoingRight changes between true and false, the GameObject's transform is rotated so that the sprite faces the correct direction.  Leveraging the property changing to trigger the rotation change is an example of logic in the property making it 'smart'.

There are pros and cons to smart properties.  For example, one may argue that including the transform change when isGoingRight is modified hides the mechanic and makes the code harder to follow.  There are always alternatives if you prefer to not use smart properties.  For example:

```csharp
bool isGoingRightNow = xVelocity > 0;
if(isGoingRight != isGoingRightNow) 
{
  transform.rotation *= backwardsRotation;    
  isGoingRight = isGoingRightNow;
}
```

</details>

<details><summary>What's a Quaternion?</summary>

A Quaternion is how rotations are stored in a game engine.  They represent the rotation with (x, y, z, w) values, stored in this fashion because that it is an effecient way to do the necessary calculations when rendering on object on screen.

You could argue that this is overkill for a 2D game as in 2D the only rotation that may be applied is around the Z axis, and I would agree.  However remember that Unity is a 3D game engine.  When creating a 2D game, you are still in a 3D environment.  Therefore under the hood, Unity still optimizes its data for 3D.

Quaternions are not easy for people to understand.  When we think of rotations, we typically think in terms of 'Euler' (pronounced oil-er) rotations.  Euler rotations are degrees of rotation around each axis, e.g. (0, 0, 30) means rotate the object by 30 degrees around the Z axis.

In the inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quatenions directly or use Euler and then convert it back to Quatenion for storage.

Given a Quatenion, you can calculate the Euler value like so:

```csharp
Quaternion myRotationInQuaternion = transform.rotation;
Vector3 myRotationInEuler = myRotationInQuaternion.eulerAngles;
```

Given an Euler value, you can calculate the Quatenion:

```csharp
Quaternion rotationOfZ30Degrees = Quaternion.Euler(0, 0, 30);
```

Quaternions may be combined using Quaternion multiplication:

```csharp
Quaternion rotationOfZ60Degrees 
  = rotationOfZ30Degrees * rotationOfZ30Degrees;
```

</details>

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
<details><summary>Why not compare to 0 when checking if there is no movement?</summary>

In Unity, numbers are represented with the float data type.  Float is a way of representing decimal numbers but is a not precise representation like you may expect.  When you set a float to some value, internally it may be rounded ever so slightly.

The rounding that happens with floats allows operations on floats to be executed very quickly.  However it means we should never look for exact values when comparing floats, as a tiny rounding issue may lead to the numbers not being equal.

In the example above, as the velocity approaches zero, the significance of if the value is positive or negative, is lost.  It's possible that if we were to compare to 0 that at times the float may oscilate between a tiny negative value and a tiny positive value causing the sprite to flip back and forth.

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






- animation walk speed

## Restrict movement to stay on screen

Create KeepWalkMovementOnScreen

## Rotate to match the floor's angle

Create a Feet with isGrounded and Quaternion floorRotation. 
Create a RotateToMatchFloorWhenGrounded



- Sound effects









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


