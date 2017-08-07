# 2) Walk, jump, and die

Basic animations, player controller, effector, particle system

In chapter 2, we add a character to the scene.  He can walk and jump, and dies when hitting an enemy, creating a basic platformer.

This assumes you completed chapter 1, or you can download the project so far. (TODO link)

TODO tutorial video link

<img src="http://i.imgur.com/s0EO9f5.gif" width=300px />
TODO try retaking gif

demo build of level 2

## 2.2) Create an animated character

Add a GameObject for the character with a walk animation. Change the order in layer to 2.

<details><summary>How</summary>

Change the character's pivot point:

 - Select all the character sprites, we are using **adventurer_tilesheet_TODO**.
 - In the Inspector, change 'Pivot' to 'Bottom'.

<br>Create character:

 - Hold Ctrl to select both **adventurer_tilesheet_9** and **10**.
   - Drag them into the Hierarchy.
   - When prompted, save the animation as Assets/Animations/**CharacterWalk**.anim

<img src="http://i.imgur.com/jPvFvnq.gif" width=300px />

 - Select the GameObject just created:
   - Order in Layer: 2
 - Create an empty parent GameObject named "Character":
   - Add the sprite GameObject as a child.

<br>Add a collider:

 - Select the Character parent GameObject:
   - Add a **Rigidbody2D**.
     - Expand the 'Constraints' and Check 'Freeze Rotation: Z'.

<img src="http://i.imgur.com/uXxDSwD.png" width=300px />

   - Add a **CapsuleCollider2D** to the Character:
     - Click 'Edit Collider' and adjust to fit the character. You can click and then hold Alt while adjusting the sides to pull both sides in evenly.

<img src="http://i.imgur.com/KFwBZeo.gif" width=150px />


<hr></details><br>
<details><summary>What did that do?</summary>

For the character, we are moving the pivot point to the 'Bottom'.  This allows us to position and rotate the character starting at the feet instead of the center of his body.


Dragging multiple sprites into the Hierarchy created:
 - The character's GameObject.
 - A SpriteRenderer component on the GameObject defaulting to the first selected sprite.
 - An Animation representing those 2 sprites changing over time.
 - An Animator Controller for the character with a default state for the Walk animation.
 - An Animator component on the GameObject configured for the Animator Controller just created.

The order in layer ensures that the character appears on top of other sprites such as platforms when jumping.

We add the sprite to a parent Character GameObject so that any animations we create do not impact other things that may be attached to the Character.  Specifically is this tutorial this ensures that the hammer we equip is not impacted by the idle animation we will be creating.

Click Play to test - your character should be walking (in place)!

<img src="http://i.imgur.com/2bkJdtS.gif" width=100px />

<br>Add a collider:

The rigidbody and collider enable physics for the character.  We size the collider to be used for standing on platforms, colliding with enemies, and picking up items.

Add a constraint to the character's rigidbody to freeze its rotation.
The character shouldn't fall over anymore.  In fact he will stand straight up even on slanted platforms.  This will be addressed later in the tutorial.


<hr></details>
<details><summary>What's Pivot do?</summary>

A pivot point is the main anchor point for the sprite.  By default, pivot points are at the center of the sprite.

Here's an example showing a character with a default 'Center' pivot and one with the recommended 'Bottom' pivot.  They both have the same Y position.  Notice the the vertical position of each character as well as how the rotation centers around the different pivot points:

<img src="http://i.imgur.com/AQY4FOT.gif" width=320 />

The pivot point you select is going to impact how we create animations and implement movement mechanics.  The significance of this topic should become more clear later in the tutorial.

</details>
<details><summary>What's the difference between Animation and Animator?</summary>

An animat**ion** is a collection of sprites on a timeline, creating an animated effect similar to a flip book.  Animations can also include transform changes, fire events for scripts to react to, etc to create any number of effects.

An animat**or** controls which animations should be played at any given time.  An animator uses an animator controller which is a state machine used to select animations.

A state machine is a common pattern in development where logic is split across several states.  The state machine selects one primary state which owns the experience until the state machine transitions to another state.  Each animator state has an associated animation to play.  When you transition from one state to another, Unity switches from one animation to the next.

We will be diving into more detail about animations and animators later in the tutorial.

<hr></details>
<details><summary>How do I know what size to make the collider?</summary>

The collider does not fit the character perfectly, and that's okay.  In order for the game to feel fair for the player we should lean in their favor.  When designing colliders for the character and enemies, we may prefer to make the colliders a little smaller than the sprite so that there are no collisions in game which may leave the player feeling cheated.

As the character animates, its limbs may be in different positions.  The collider won't always fit the character and for that reason we use a collider focused around the body.

In addition to killing the character when he comes in contact with an enemy, the collider is used to keep the character on top of platforms.  For this reason it's important that the bottom of the collider aligns with the sprite's feet.

</details>
<details><summary>Why not use a collider that outlines the character?</summary>

Bottom line, it's not worth the trouble.  Unity does not provide good tools for more accurate collisions on animating sprites.  Implementing this requires a lot of considerations and may be difficult to debug.

Most of the time the collisions in the game would not have been any different if more detailed colliders were used.  Typically 2D games use an approach similar to what this tutorial recommends. It creates a good game feel and the simplifications taken have become industry standard.

</details>
<details><summary>Why freeze rotation and does freezing mean it can never change?</summary>

We freeze the character so he does not fall over on the slanted platforms like this:

<img src="http://i.imgur.com/T0fdwa1.gif" width=150px />

Adding constraints to the rigidbody only limits the Unity physics engine. Freezing the rigidbody position or rotation means that even if you got hit by a bus, you would not move or rotate.  However you could have a custom component set the position or rotation at any time.

Later in the tutorial we will be writing a script to rotate entities so that they align with platforms (i.e. their feet sit flat on the floor).

We use constraints to remove capabilities from Unity, allowing us more control where we need it.  Specifically here that means our character is not going to ever fall flat on his face.

</details>


## 2.5) Add a script to move left & right

Add a script to the character to be able to move left and right once a controller is added.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**WalkMovement**:

```csharp
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkMovement : MonoBehaviour
{
  [NonSerialized]
  public float desiredWalkDirection;

  [SerializeField]
  float walkSpeed = 100;

  Rigidbody2D myBody;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
  }

  protected void FixedUpdate()
  {
    float desiredXVelocity
      = desiredWalkDirection
        * walkSpeed
        * Time.fixedDeltaTime;

    myBody.velocity = new Vector2(
      desiredXVelocity,
      myBody.velocity.y);
  }
}
```

 - Add **WalkMovement** to the Character.

<hr></details><br>
<details><summary>What did that do?</summary>

Nothing yet.  This script enables movement, but requires a separate controller to function.

A controller (created in the next section) will set the desiredWalkDirection, then every FixedUpdate WalkMovement turn that into horizontal velocity on the rigidbody while preserving any vertical velocity (so not to interfere with gravity).

<hr></details>
<details><summary>What's a controller?  Why not read input here?</summary>

As discussed in chapter 1, Unity encourages a component based solution.  This means that we attempt to make each component focused on a single mechanic or feature.  Doing so simplifies debugging and enables reuse.  For example, we will be creating another enemy type which will use the same WalkMovement component created for the character above.

<hr></details>
<details><summary>Why set velocity instead of using AddForce?</summary>

AddForce is a way of impacting a rigidbody's velocity indirectly.  Anytime you interact with either AddForce or velocity, a similar mechanic could be made using the other.

Generally the game feel when using AddForce has more gradual changes and for many experiences that's great.  Although there are lots of options for tuning the forces experience, velocity simply gives you more direct control.

So that's to say you could use AddForce here instead.  Maybe give it a try and see how it feels.  We select velocity because we want the controls for moving left and right to feel crisp.  Later in the tutorial we will use AddForce, for the jump effect.

</details>
<details><summary>Why FixedUpdate instead of Update?</summary>

Update occurs once per rendered frame.  FixedUpdate occurs at a regular interval, every x ms of game time.  FixedUpdate may run 0 or more times each frame.

FixedUpdate is preferred for mechanics which require some level of consistency or apply changes incrementally.  Physics in Unity are processed in FixedUpdated.  So when manipulating physics for the game such as we are here by changing velocity on the rigidbody, we do this on FixedUpdate to match Unity's expectations.

</details>
<details><summary>Why multiply by Time.fixedDeltaTime?</summary>

It's optional. Anytime you make a change which includes some speed, such as walking, we multiply by the time elapsed so motion is smooth even when the frame rate may not be.  While using FixedUpdate, the time passed between calls is always the same - so Time.fixedDeltaTime is essentially a constant.

If speed is being processed in an Update, you must multiply by Time.deltaTime for a smooth experience.  While in FixedUpdate, you could opt to not use Time.fixedDeltaTime, however leaving it out may lead to some confusion as fields which are configured for FixedUpdate may have a different order of magnitude than fields configured for use in Update.

Additionally you may choose to adjust the time interval between FixedUpdate calls while optimizing your game.  By consistently multiplying by the delta time, you can adjust the interval for FixedUpdate without changing the game play.

</details>



## 2.6) Add a player controller

Add a script to the character to read user input and drive movement.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**PlayerController**:

```csharp
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class PlayerController : MonoBehaviour
{
  WalkMovement walkMovement;

  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
  }

  protected void FixedUpdate()
  {
    walkMovement.desiredWalkDirection
      = Input.GetAxis("Horizontal");
  }
}
```

 - Add **PlayerController** to the Character.

</details><br>
<details><summary>What did that do?</summary>

The character should walk around (use arrow keys or WASD), but there is clearly work to be done:

<img src="http://i.imgur.com/xOpivgJ.gif" />

Note the character will always be looking right, even while walking left.  He can also walk off the screen and push the balls around.  This will all be addressed later in the tutorial.

<hr></details>
<details><summary>What is an Input 'Axis' and how are they configured?</summary>

Unity offers several ways of detecting keyboard/mouse/controller input.  'Axis' is the recommended approach.  Each input Axis may be configured in the inspector:

 - Edit -> Project Settings -> Input.
 - In the 'Inspector', you will find a list of supported input types.

<img src="http://i.imgur.com/T2BJvBm.png" width=100px />

You can add, remove, rename, and configure the inputs for your game.  Inputs may also be reconfigured by the player at runtime.  For more information about the various options, see [Unity's description of the InputManager](https://docs.unity3d.com/Manual/class-InputManager.html).  We will be using the defaults for this tutorial.

To read / detect Input, Unity offers a few APIs including:

 - GetAxis: Gets the current state as a float.  E.g. horizontal may return 1 for right, -1 for left.
 - GetButtonDown/GetButtonUp: Determines if a button was pressed or released this frame.
 - GetMouseButtonDown/GetMouseButtonUp: Same as above, but for mouse buttons.

There are a ton of options, check out the [complete list of Input APIs](https://docs.unity3d.com/ScriptReference/Input.html).

</details>
<details><summary>Why not use a bool or Enum for Left/Right instead of a float?</summary>

You could for the game we are making at the moment.  When playing with a keyboard, a button is down or it isn't.

The nice thing about using a float here is it could be leveraged to allow players even more control over movement.  When playing with a controller, left and right are not simply on and off - the amount you move the joystick  by scales how quickly the character should walk.

The WalkMovement desiredWalkDirection should be set to something in the range of -1 to 1, where 1 represents the desire to walk at full speed towards the right.  From there the WalkMovement component will apply the walkSpeed, representing the fastest speed the entity should walk, and then update the rigidbody.

</details>


## 2.7) Jump

Add a script to the character to be able to jump and update the player controller to match.  Play a sound effect when an entity jumps.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**JumpMovement**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class JumpMovement : MonoBehaviour
{
  [SerializeField]
  AudioClip jumpSound;

  [SerializeField]
  float jumpSpeed = 7f;

  Rigidbody2D myBody;

  AudioSource audioSource;

  bool wasJumpRequestedSinceLastFixedUpdate;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    audioSource = GetComponent<AudioSource>();
  }

  public void Jump()
  {
    wasJumpRequestedSinceLastFixedUpdate = true;
  }

  protected void FixedUpdate()
  {
    if(wasJumpRequestedSinceLastFixedUpdate)
    {
      myBody.AddForce(
          new Vector2(0, jumpSpeed),
          ForceMode2D.Impulse);

      audioSource.PlayOneShot(jumpSound);
    }

    wasJumpRequestedSinceLastFixedUpdate = false;
  }
}
```

 - Add **JumpMovement** to the Character (this will automatically add an **AudioSource**):
   - Select the Jump Sound.  We are using **Jump**.

<img src="http://i.imgur.com/I5JWg9s.gif" width=300px />

 - Update Code/Components/Controllers/**PlayerController**. by adding the code below:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
```

</details>

```csharp
[RequireComponent(typeof(JumpMovement))]
```

<details><summary>Existing code</summary>

```csharp
public class PlayerController : MonoBehaviour
{
  WalkMovement walkMovement;
```

</details>

```csharp
  JumpMovement jumpMovement;
```

<details><summary>Existing code</summary>

```csharp
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
```

</details>

```csharp
    jumpMovement = GetComponent<JumpMovement>();
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void FixedUpdate()
  {
    walkMovement.desiredWalkDirection
      = Input.GetAxis("Horizontal");
  }
```

</details>

```csharp
  protected void Update()
  {
    if(Input.GetButtonDown("Jump"))
    {
      jumpMovement.Jump();
    }
  }
```

<details><summary>Existing code</summary>

```csharp
}
```

</details>


<hr></details><br>
<details><summary>What did that do?</summary>

When you press space, JumpMovement adds force to the entity causing it to jump up.  But you can spam the space bar to fly away.

A sound should play every time you jump, you can adjust the volume in the AudioSource component.

Like with walking, we use two separate components for this mechanic. JumpMovement enables the actual jump itself, allowing it to be used on another entity if we choose, and the PlayerController reads input in order to initiate jumps.

<hr></details>
<details><summary>Why AddForce here instead of velocity and what's 'Impulse'?</summary>

As discussed above when creating the WalkMovement component, you could always create mechanics using either AddForce or by modifying the velocity.

We are using AddForce to jump in this component.  Using velocity here instead would have actually created the same basic jump experience we are looking for.

Using AddForce for the jump may provide a better experience for some corner cases or future mechanics.  For example, if we wanted to support double jump in this game, initiating the second jump while in the air would feel much different.

What is ForceMode2D.Impulse and how is it different from ForceMode2D.Force?

These options have very similar effects on objects, the biggest difference is the scale (i.e. how much motion X creates when Impulse vs Force).   The unit for Impulse is defined as force per FixedUpdate.  The unit for Force is defined as force per second.

</details>
<details><summary>How do you know when to use Update vs FixedUpdate for Input and rigidbodies?</summary>

Unity recommends always using FixedUpdate when interacting with a rigidbody as physics is processed in FixedUpdate.

There is nothing blocking you from changing the rigidbody in an Update loop.  You could, for example, AddForce every Update.  This is not recommended and may lead to inconsistent experiences.

For Input:

 - When reading the current Input state (e.g. using Input.GetAxis), either FixedUpdate or Update is fine.  For example if you are checking the current position of the joystick, you'll get the same information in FixedUpdate and Update.
  - If you need to modify a rigidbody based on current Input state, I recommend reading Input in FixedUpdate to keep it simple.
 - When checking for an Input event (e.g. using Input.GetButtonDown), you must use Update.  Input is polled in the Update loop.  Since it's possible for two Updates to happen before a FixedUpdate, some events may be missed when only checking in FixedUpdate.
   - Always read events in Update.  Unity will not block or warn you when checking for an event in FixedUpdate, and most of the time it will work - but occasional bugs will arise.

<hr></details>
<details><summary>Why is AudioSource on a GameObject vs just playing clips?</summary>

Audio playback in Unity is built to support 3D audio.  3D audio refers to the feature where the closer an object making noise is to your ear, the louder it is.  Additionally 3D sound is directional, so sounds to the players left would be loudest in the left speaker.

TODO 2D is left and right channels.  3D adds distance.

Your 'ear' is typically the camera itself.  This is managed by the AudioListener component which was placed on the Main Camera by default when the scene was created.  You could choose to move this component to the character instead, if appropriate.

To enable 3D audio, sounds need to originate at a position in the world.  We use the AudioSource component to play clips.  As a component, it must live an a GameObject which in turn must have a Transform -- the position we are looking for.

For consistency, 2D audio is played the same way.  2D means we don't have the features above, the clip sounds the same regardless of where it the world it was initiated from.  Note that audio is 2D by default.

Alternatively you could use the Unity API to play a clip as shown below.  This API will create an empty GameObject at the location provided, add an AudioSource component to it, configure that source to use the clip specified and have the AudioSource start playing.  After the clip completes, the GameObject will be destroyed automatically.

```csharp
[SerializeField]
AudioClip clip;

protected void Start()
{
  AudioSource.PlayClipAtPoint(clip, new Vector3(5, 1, 2));
}
```

</details>
<details><summary>Would two separate player controllers be a better component based solution?</summary>

Maybe, but it feels like overkill.  The value of separating components is to allow us to mix and match to create new experiences.  In this tutorial, we have no use case for using one or the other player controller mechanic (i.e. just support walking or just support jumping).

<hr></details>


## 4.1) Flip entities when they walk the other way

Flip the entity when they switch between walking left and right.

<details><summary>How</summary>

 - Create script Components/Movement/**FlipFacingDirection**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlipFacingDirection : MonoBehaviour
{
  Rigidbody2D myBody;

  SpriteRenderer sprite;
  
  bool _isGoingLeft;

  public bool isGoingLeft
  {
    get
    {
      return _isGoingLeft;
    }
    private set
    {
      if(isGoingLeft == value)
      {
        return;
      }

      _isGoingLeft = value;
      sprite.flipX = isGoingLeft;
    }
  }

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    sprite = GetComponentInChildren<SpriteRenderer>();
  }

  protected void FixedUpdate()
  {
    float xVelocity = myBody.velocity.x;
    if(Mathf.Abs(xVelocity) > 0.1)
    {
      isGoingLeft = xVelocity < 0;
    }
  }
}
```

 - Add **FlipFacingDirection** to the character prefab.

<hr></details><br>
<details><summary>What did that do?</summary>

Each FixedUpdate, we determine which direction the entity is walking by its X velocity.  When the direction changes, we flip the sprite so that the character appears to be facing the other way.

<hr></details>
<details><summary>What's a C# smart property?</summary>

In C#, data may be exposed as either a Field or a Property.  Fields are simply data as one would expect.  Properties are accessed in code like a field is, but they are capable of more.

In this example, when isGoingRight changes between true and false, the GameObject's transform is rotated so that the sprite faces the correct direction.  Leveraging the property changing to trigger the rotation change is an example of logic in the property making it 'smart'.

There are pros and cons to smart properties.  For example, one may argue that including the transform change when isGoingRight is modified hides the mechanic and makes the code harder to follow.  There are always alternatives if you prefer to not use smart properties.  For example:

```csharp
bool isGoingLeftNow = xVelocity <> 0;
if(isGoingLeft != isGoingLeftNow) 
{
  sprite.flipX = isGoingLeft;
  isGoingLeft = isGoingLeftNow;
}
```

</details>
<details><summary>Why not compare to 0 when checking if there is no movement?</summary>

In Unity, numbers are represented with the float data type.  Float is a way of representing decimal numbers but is a not precise representation like you may expect.  When you set a float to some value, internally it may be rounded ever so slightly.

The rounding that happens with floats allows operations on floats to be executed very quickly.  However it means we should never look for exact values when comparing floats, as a tiny rounding issue may lead to the numbers not being equal.

In the example above, as the velocity approaches zero, the significance of if the value is positive or negative, is lost.  It's possible that if we were to compare to 0 that at times the float may oscillate between a tiny negative value and a tiny positive value causing the sprite to flip back and forth.

</details>

## 3.10) Fade in

Add a script to entities so they fade in before moving.

<details><summary>How</summary>

 - Create script Components/Life/**FadeInThenEnable**:

```csharp
using System.Collections;
using UnityEngine;

public class FadeInThenEnable : MonoBehaviour
{
  [SerializeField]
  float timeTillEnabled = 3;

  [SerializeField]
  MonoBehaviour[] componentsToEnable;

  protected void OnEnable()
  {
    StartCoroutine(FadeIn());
  }

  protected void OnDisable()
  {
    StopAllCoroutines();
  }

  IEnumerator FadeIn()
  {
    SpriteRenderer[] spriteList
      = gameObject.GetComponentsInChildren<SpriteRenderer>();

    float timePassed = 0;
    while(timePassed < timeTillEnabled)
    {
      float percentComplete = timePassed / timeTillEnabled;
      SetAlpha(spriteList, percentComplete);

      yield return null;

      timePassed += Time.deltaTime;
    }

    SetAlpha(spriteList, 1);

    for(int i = 0; i < componentsToEnable.Length; i++)
    {
      MonoBehaviour component = componentsToEnable[i];
      component.enabled = true;
    }
  }

  void SetAlpha(
    SpriteRenderer[] spriteList,
    float alpha)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      Color originalColor = sprite.color;
      sprite.color = new Color(
        originalColor.r, 
        originalColor.g, 
        originalColor.b, 
        alpha);
    }
  }
}
```

 - Disable the character's PlayerController component.

<img src="http://i.imgur.com/5WtzPmv.png" width=300px />

 - Select the Character and add **FadeInThenEnable**:
   - Expand 'Components to Enable'.
   - Set 'Size' to 1 and then hit tab for the list to update.
   - Drag/drop the PlayerController into the list as 'Element 0'.

<img src="http://i.imgur.com/hrXMt1f.gif" width=300px />

 - Select the HoverGuy prefab:
   - Disable the WanderWalkController.
   - Add **FadeInThenEnable**:
     - Assign WanderWalkController to the Components to Enable list.
 - Select the Hammer prefab:
   - Add **FadeInThenEnable** (nothing needed in the to enable list).

<hr></details><br>
<details><summary>What does this do?</summary>

The FadeInThenEnable script smoothly transitions the alpha for all the sprites in that GameObject from 0 (hidden) to 1 (visible) and then enables the list of components configured.

FadeInThenEnable is added to the Character and we disable the PlayerController to prevent any input such as walk or jump until complete.

On the HoverGuy we disable wander movement until complete.

For the Hammer, we could disable the Hammer component (preventing pickup) but it is unnecessary since the character can't move.

<hr></details>
<details><summary>What does StopAllCoroutines do?</summary>

StopAllCoroutines will stop any coroutines which were started by this script.  Coroutines in Unity are not running on a different thread, so nothing will be interrupted in that sense - however any coroutine which has yield returned and is expecting to be resumed will not be.

Coroutines are automatically stopped when a GameObject is Destroyed or SetActive(false) is called.  However disabling a component (and not the entire GameObject) does not automatically stop coroutines - which is why we do it explicitly OnDisable here.

</details>



## 2.11) Create a pattern for death effects

Create a pattern to use instead of destroying GameObjects directly, allowing an opportunity for objects to animate on death.

<details><summary>How</summary>

 - Create script Code/Components/Death/**DeathEffect**:

```csharp
using UnityEngine;

[RequireComponent(typeof(DeathEffectManager))]
public abstract class DeathEffect : MonoBehaviour
{
  public abstract float timeUntilObjectMayBeDestroyed
  {
    get;
  }

  public abstract void PlayDeathEffects();
}
```

 - Create script Code/Components/Death/**DeathEffectManager**:

```csharp
using UnityEngine;

public class DeathEffectManager : MonoBehaviour
{
  bool isInProcessOfDieing;

  public static void PlayDeathEffectsThenDestroy(
    GameObject gameObject)
  {
    DeathEffectManager deathEffectManager
      = gameObject.GetComponent<DeathEffectManager>();

    if(deathEffectManager == null)
    {
      Destroy(gameObject);
      return;
    }

    deathEffectManager.PlayDeathEffectsThenDestroy();
  }

  void PlayDeathEffectsThenDestroy()
  {
    if(isInProcessOfDieing)
    {
      return;
    }
    isInProcessOfDieing = true;

    DeathEffect[] deathEffectList
      = gameObject.GetComponentsInChildren<DeathEffect>();

    float maxTimeUntilObjectMayBeDestroyed = 0;
    for(int i = 0; i < deathEffectList.Length; i++)
    {
      DeathEffect deathEffect = deathEffectList[i];
      maxTimeUntilObjectMayBeDestroyed = Mathf.Max(
        maxTimeUntilObjectMayBeDestroyed,
        deathEffect.timeUntilObjectMayBeDestroyed);

      deathEffect.PlayDeathEffects();
    }

    Destroy(gameObject, maxTimeUntilObjectMayBeDestroyed);
  }
}
```

<hr></details><br>
<details><summary>What did that do?</summary>

Nothing yet.

This is a pattern we will leverage a few times in this tutorial, starting with the next section.

When an entity dies in the game, we call DeathEffectManager.PlayDeathEffectsThenDestroy instead of the usual Unity Destroy method.

This allows us to defer the actual Destroy call, and to spawn an explosion or play an animation on the sprite as it dies.  Also it allows us to differentiate between a request to immediately destroy a GameObject (e.g. for a scene change) vs a death that should maybe animate and spawn an explosion.

<hr></details>
<details><summary>Why not just play effects OnDestroy()?</summary>

OnDestroy is called anytime the object is destroyed, but we only want the death effects to trigger in certain circumstances.  For example, when we quit back to the main menu, we do not want explosions spawning for character being destroyed while closing level 1.

This pattern was selected because:

 - It gives us easy control over when DeathEffects should be considered, vs promptly destroying the object.
 - It gracefully falls back to Destroy when there are no DeathEffects to play.
 - It allows for several separate DeathEffects to be combined, creating a new kind of effect.

As always, there are probably a thousand different ways you could achieve similar results.

</details>
<details><summary>Why is there a public method that 'should not be called directly'?</summary>

PlayDeathEffects() in the DeathEffect class has a public method with a comment saying it 'should not be called directly'.  So why is it public?

In order to support multiple DeathEffects and to be able to fallback gracefully when an object does not have one, we always start effects by calling the public static method in DeathEffectManager, PlayDeathEffectsThenDestroy.

Since DeathEffectManager is a class of its own, we would not be able to call a private or protected method in DeathEffect.

'internal' could be an option to consider, but typically when working in Unity you are working in a single project - therefore internal is effectively the same as public.

You might also consider using nested classes.  For simplicity in the tutorial, we're not using nested classes as they can be a bit confusing.  If you are familiar with this topic, briefly you could make DeathEffectsManager a class nested inside DeathEffect and then make PlayDeathEffects() private, and the rest pretty much works the same.

</details>
<details><summary>Why are you using Mathf and not System.Math?</summary>

Unity offers the UnityEngine.Mathf class to try and make some things a little easier.  Basically it's the same APIs which are offered from the standard System.Math class (which is also still available to use if you prefer).  The main difference is all of the APIs in Mathf are focused on the float data type, where the System.Math class often prefers double.  Most of the data you interact with in Unity is float.

</details>

<details><summary>What does GetComponentsInChildren do?</summary>

GetComponent returns a reference to the component (or script) which is the type specified or inherits from the type specified.

GetComponents returns an array with every matching component.

GetComponentInChildren returns one match, from this GameObject or one of its child GameObjects.

GetComponentsInChildren returns an array with every matching component from this GameObject and all of its children (and their children).

</details>

<details><summary>What's C# abstract do and how's it different from an interface?</summary>

In C#, abstract refers to a class which is incomplete and may not be instantiated directly.  In order to create an object, a sub class inherits from the abstract class and you can then instantiate the sub class.

The sub class has access to everything created in the parent class, similar to if you had copy pasted everything from the parent into the child.

```csharp
public abstract class MyParentClass
{
  public int points;
}

public class MySubClass : MyParentClass
{
  public void PrintPoints()
  {
    print(points);
  }
}
```

An abstract class may include an abstract method when the parent knows a method should exist, but not how it should be implemented.

```csharp
public abstract class MyParentClass
{
  public int points;

  public abstract void PrintPoints();
}

public class MySubClass : MyParentClass
{
  public override void PrintPoints()
  {
    print(points);
  }
}
```

This allows you to create an API that works with all sub classes of the parent.

```csharp
public void Print(MyParentClass a)
{
  a.PrintPoints();
}
```

Methods may also be virtual, meaning the parent has an implementation but the child my optionally extend or replace it.


```csharp
public abstract class MyParentClass
{
  public int points;

  public virtual void PrintPoints()
  {
    print(points);
  }
}

public class MySubClass : MyParentClass
{
  public override void PrintPoints()
  {
    print("You have... ");
    base.PrintPoints();
  }
}
```

In C#, an interface is similar to an abstract class that has no data or non-abstract methods (including virtual).  Interfaces are a way of defining a common API for classes to leverage.  The name of an interface always starts with "I", by convention.

```csharp
public interface IMyInterface
{
  void PrintPoints();
}

public class MyClass : IMyInterface
{
  public int points;

  public void PrintPoints()
  {
    print(points);
  }
}
```

Other methods can leverage an interface without knowing the class that implemented the method like we did with the abstract class.

```csharp
public void Print(IMyInterface a)
{
  a.PrintPoints();
}
```

<hr></details>


## 2.12) Kill the player when he hits a ball

When the player comes in contact with a spiked ball, kill him!

<details><summary>How</summary>

 - Create script Code/Utils/**LayerMaskExtensions**:

```csharp
using UnityEngine;

public static class LayerMaskExtensions
{
  public static bool Includes(
    this LayerMask mask,
    int layer)
  {
    return (mask.value & (1 << layer)) > 0;
  }
}
```

 - Create script Code/Components/Death/**KillOnContactWith**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillOnContactWith : MonoBehaviour
{
  [SerializeField]
  LayerMask layersToKill;

  protected void Start() {}

  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    ConsiderKilling(collision.gameObject);
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    ConsiderKilling(collision.gameObject);
  }

  void ConsiderKilling(
    GameObject gameObjectWeJustHit)
  {
    if(enabled == false)
    {
      return;
    }

    if(layersToKill.Includes(gameObjectWeJustHit.layer) == false)
    {
      return;
    }

    DeathEffectManager.PlayDeathEffectsThenDestroy(gameObjectWeJustHit);
  }
}
```

 - Under Project Settings -> Tags and Layers:
   - Add a layer for 'Player'.
 - Select the Character GameObject:
   - Change its Layer to Player.
     - When prompted, 'No, this object only'.
 - Select the SpikeBall prefab in Assets/Prefabs:
    - Add **KillOnContactWith**:
      - Update 'Layers To Kill' to Player.

<img src="http://i.imgur.com/wrkb3eJ.png" width=300px />

<hr></details><br>
<details><summary>What did that do?</summary>

We created a component to use on enemies which will initiate the death sequence for the Character anytime he touches one of the enemy colliders (both collisions and triggers).

Hit play to watch the player die:

<img src="http://i.imgur.com/gKEl8wE.gif" width=300px />

For now, to test again stop and hit play again.  We'll respawn the player later in the tutorial.

<hr></details>
<details><summary>Why check the layer instead of using the Collision Matrix?</summary>

Layers are defined per GameObject.  The GameObject we will be adding this script to, already have a layer defined to support other use cases.  This means that the KillOnContactWith component will get event calls for collisions with other objects such as the platforms.

In order to do this with a Collision Matrix, a child GameObject with its own Layer could be added to hold this component.

<hr></details>
<details><summary>What is this '& 1 <<' black magic?</summary>

Bitwise operations... which are beyond the scope of this tutorial.  More specifically, this is 'bitwise and' and 'bit shifting' if you would like to read more about this.  Here is a [Stackoverflow post on the topic](http://answers.unity3d.com/questions/8715/how-do-i-use-layermasks.html).

<hr></details>
<details><summary>What is a C# extension method?</summary>

Extension methods are a way of adding additional methods to a class or struct you don't own.  In this example, Unity has a struct 'LayerMask'.  That struct does not offer an easy way to determine if a layer is part of that LayerMask.  Using extensions, we are able to create an 'Includes' method that then can be used as if Unity had written it for us.

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
<details><summary>Why is there an empty Start method and why check if enabled?</summary>

We will need the ability to disable this component later in the tutorial.

A disabled component will not get called for events such as Update.  However it does still receive some calls while disabled, including OnTriggerEnter. This is why we check if enabled vs depending on Unity to do that for us.

Unity only allows you to use the enable / disable feature if it detects that there is a method in the script which would be impacted.  We added an empty Start method to get the enable / disable feature since Unity does not enable enable by checking 'if(enabled)' in code.

<hr></details>

## 2.13) Create an explosion prefab with sound effect

Create an explosion particle system and save it as a prefab.

<details><summary>How</summary>

 - Create an empty GameObject:
   - Name it "Explosion".
   - Add a **ParticleSystem**:
     - Set 'Renderer' Material: Default-Particle
     - Set 'Renderer' Max Particle Size: 1000

<img src="http://i.imgur.com/xkv8CJd.png" width=300px />

 - Back at the top of the Particle System component, set:
   - Looping: Off
   - Start Lifetime: 0.5
   - Start Size: 30
   - Scaling Mode: Local

<img src="http://i.imgur.com/qlmzCMy.png" width=300px />

 - Update the Transform scale to about (.05, .05, .05)
 - Enable Color over Lifetime, and then:
   - Click the color to open the Gradient editor.
   - Click above the color bar, about 1/5th in from the left - creating a keyframe.
   - Click on the top left keyframe, change Alpha to 0.  Do the same for the top right.
   - Click on the bottom left keyframe, change the color to Hex 'FFF3DD'.

<img src="http://i.imgur.com/x7tYdUE.gif" width=300px />

 - Under 'Emission':
   - Rate over Time: 0
   - Click the '+' under 'Bursts' to create an entry, change
     - Min: 3
     - Max: 3

<img src="http://i.imgur.com/TPWUZjE.png" width=300px />


 - Add **AudioSource** to the GameObject:
   - Change the AudioClip.  We are using **Death**.
 - Drag the Explosion GameObject into Assets/Prefabs.
 - Delete the Explosion GameObject.


 - Create script Components/Death/**SuicideIn**:

```csharp
using UnityEngine;
using System.Collections;

public class SuicideIn : MonoBehaviour
{
  [SerializeField]
  float timeTillDeath = 5;

  protected void Start()
  {
    StartCoroutine(CountdownToDeath());
  }

  IEnumerator CountdownToDeath()
  {
    yield return new WaitForSeconds(timeTillDeath);
    DeathEffectManager.PlayDeathEffectsThenDestroy(gameObject);
  }
}
```

 - Add **SuicideIn** to the Explosion prefab.

<hr></details><br>
<details><summary>What did that do?</summary>

We created a particle system to represent a simple explosion and added a sound effect to match.  The 'system' is just 3 particles which look a bit like clouds.  They scale and color overtime to create the effect.

Briefly, the rational for each change recommended above:

 - Set the material: the default may be broken due to a Unity Bug, we are simply selecting what should have been the default.
 - Particle Size: this limits the size of the effect you may see on the screen.  We crank it up so that while previewing the in Scene window we can zoom in.
 - Looping: just one explosion.
 - Start Lifetime: Defines how long until each particle should be destroyed.
 - Start Size: How large each particle is.
 - Scaling Mode: Enables us to scale the size of the explosion using Transform scale.
 - Color over Lifetime: Changes the coloring to add to the effect.
 - Emission: Defines when and how many particles to create.  We are using exactly 3 particles for each explosion.



<hr></details>
<details><summary>What's a particle / particle system?</summary>

A particle is a small 2D image managed by a particle system.  It's optimized to display a large number of similar particles at the same time, possible with different colors, sizes, etc.

A Particle System component animates a number of particles to create effects such as fluid, smoke, and fire. Read more about [Particle Systems from Unity](https://docs.unity3d.com/Manual/class-ParticleSystem.html).

</details>
<details><summary>Could you RNG select the audio clip to play?</summary>

Anything is possible.  Here's a little code sample that may help you get started.

On a related note, you could also randomize the pitch to get some variation between each clip played.  e.g. this could be a nice addition to a rapidly firing gun.

```csharp
[SerializeField]
AudioClip clip1;
[SerializeField]
AudioClip clip2;

protected void OnEnable()
{
  AudioSource audioSource = GetComponent<AudioSource>();
  switch(UnityEngine.Random.Range(0, 2))
  {
    case 0:
    audioSource.clip = clip1;
    break;
    case 1:
    audioSource.clip = clip2;
    break;
  }
  audioSource.Play();
}
```

</details>

## 2.14) Spawn and destroy explosion 

Add a script which spawns the explosion prefab when the character dies.  The explosion should Destroy after a few the effect completes.

<details><summary>How</summary>

 - Create script Components/Death/**DeathEffectSpawn**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathEffectSpawn : DeathEffect
{
  [SerializeField]
  GameObject gameObjectToSpawnOnDeath;

  public override float timeUntilObjectMayBeDestroyed
  {
    get
    {
      return 0;
    }
  }

  public override void PlayDeathEffects()
  {
    Collider2D collider = GetComponent<Collider2D>();

    Instantiate(
      gameObjectToSpawnOnDeath,
      collider.bounds.center,
      Quaternion.identity);
  }
}
```

 - Add **DeathEffectSpawn** to the character (this will automatically add a **DeathEffectManager** as well).
   - Assign the Explosion prefab to 'Game Object To Spawn'.


<hr></details><br>
<details><summary>What did that do?</summary>

DeathEffectSpawn will spawn in another GameObject when the entity it's on dies.  In this case, we spawn the explosion when the character dies:

<img src="http://i.imgur.com/XhhkRpC.gif" width=150px />

This component starts DeathEffects for a GameObject, which in turn will destroy the GameObject, after a period of time.  We use it on the explosion to prevent a memory leak by deleting it's GameObject after the explosion itself is no longer visible.

<hr></details>
<details><summary>What's bounds represent?</summary>

The Unity Bounds struct represents the axis aligned bounding box for the collider.  This means if you were to contain the collider in a cube which cannot be rotated - what is the position and size of the smallest possible surrounding cube.

For 2D, the Bounds struct still has a z but it will be 0 and everything else will work as expected.

Unity has a number of APIs available for bounds.  Here we are using .center, which represents the center of the collider which may differ from the transform position - particularly for the character since the pivot point is Bottom.

</details>
<details><summary>Why not spawn the explosion at transform.position instead of bounds.center?</summary>

The character sprite was configured with Pivot 'Bottom'.  The transform.position refers to the location of this pivot point.  If we were to target transform.position instead, the explosion would center around the character's feet.

This component could be reused on other GameObjects which may have a different pivot point. It will work correctly so long as the object has a collider.

We use the collider's bounds to determine where to spawn the explosion.  The [bounds struct](https://docs.unity3d.com/ScriptReference/Bounds.html) has a number of convenient methods for things like determining the center point of an object.

</details>
<details><summary>Why bother destroying, the explosion is not visible after a few seconds?</summary>

Similar to how we destroyed balls which rolled off the bottom of the screen in chapter 1, we need to ensure the explosion GameObjects are destroyed at some point.

The explosion effect on screen only lasts for a few seconds, but Unity does not realize this on its own.  Destroying the GameObject prevents Unity from wasting resources on the old GameObjects which are never going to be visible again.

In other words, this script ensures that our explosions do not result in a memory leak.

</details>


## 2.17) Animate characters death

Add a scaling effect for the character dieing, in addition to the explosion.

<details><summary>How</summary>

 - Create script Components/Death/**DeathEffectThrob**:

```csharp
using UnityEngine;
using System.Collections;

public class DeathEffectThrob : DeathEffect
{
  [SerializeField]
  float lengthOfEffectInSeconds = 1;

  [SerializeField]
  int numberOfPulses = 5;

  public override float timeUntilObjectMayBeDestroyed
  {
    get
    {
      return lengthOfEffectInSeconds;
    }
  }

  public override void PlayDeathEffects()
  {
    StartCoroutine(ThrobToDeath());
  }

  IEnumerator ThrobToDeath()
  {
    float timePerPulse
      = lengthOfEffectInSeconds / numberOfPulses;
    float timeRun = 0;
    while(timeRun < lengthOfEffectInSeconds)
    {
      float percentComplete
        = timeRun / lengthOfEffectInSeconds;
      float sinValue
        = Mathf.Sin(Mathf.PI * timeRun / timePerPulse);
      float pulse = .5f + Mathf.Abs(sinValue);
      float scale = (1 - percentComplete) * pulse;
      gameObject.transform.localScale
        = Vector3.one * scale;

      yield return null;
      timeRun += Time.deltaTime;
    }

    Destroy(gameObject);
  }
}
```

 - Add **DeathEffectThrob** to the Character.

<hr></details><br>
<details><summary>What did that do?</summary>

When the DeathEffects for an entity are initiated, this component will scale the sprite up and down while shrinking it overall until it's gone.

<img src="http://i.imgur.com/gSJtJRd.gif" width=300px />

<hr></details>

<details><summary>What does yield return null do?</summary>

Enumerators are methods which can 'yield return' and then later be resumed from where they left off.  Coroutines in Unity are enumerators.

With Coroutines, "yield return null" is shorthand for wait for one frame.

Each of these accomplishes the same, the coroutine resumes on the next Update:

```csharp
yield return null; // Preferred
yield return new WaitForSeconds(0); // Same, but longer
yield return 0; // Less efficient
```

<hr></details>
<details><summary>Why not use an animation instead?</summary>

You could.  There are numerous ways to create animations and effects - in this tutorial we cover a few different approaches just for the experience.

We will be introducing Unity 'animations' later in this tutorial.

<hr></details>
<details><summary>Why use Mathf.Sin?</summary>

Sin is used frequently in game dev because of the nice curve it creates:

<img src="https://upload.wikimedia.org/wikipedia/commons/d/d2/Sine_one_period.svg" width=300px />

We will be taking the absolute value, so the curve from 0 to Pi repeats over and over.  The result oscillates smoothly between 0 and 1.

We add .5 to the result, giving us .5 -> 1.5.  That's used as a multiple when scaling, creating the throb effect.

More about how you can use [Sin and Cos to create nice curves from OSU.edu](https://accad.osu.edu/~aprice/courses/694/Sin_fun.htm).

<hr></details>
<details><summary>How might we disable movement when the character is dying?</summary>

After the character dies and the throb animation begins, you can still walk around.  This could be addressed, but we are leaving it like this for the tutorial for simplicity and because it's kind of funny looking.

To stop movement, you could disable the PlayerController or the Rigidbody.  You might also want to stop the current animation as well.

<hr></details>

## 3.4) Spawn in a hovering enemy

Create a GameObject for the HoverGuy, reusing components from the SpikeBall and character.  

<details><summary>How</summary>

Create the HoverGuy:

 - Select **spritesheet_jumper_30**, **84**, and **90** and drag them into the Hierarchy, creating Assets/Animations/**HoverGuyWalk**.
   - Order in Layer: 1
 - Add the sprite to a parent GameObject named "HoverGuy":
   - Layer: Enemy
   - Add a **Rigidbody2D**:
     - Freeze the Z rotation.
   - Add a **CapsuleCollider2D**:
     - Adjust the size to fit the sprite's body.

<img src="http://i.imgur.com/d1lxoEj.png" width=150px />

<br>Make the HoverGuy float above the ground:

 - Create a Layer for "Feet".
   - Update the Physics 2D collision matrix to:
     - Disable Feet / Player.
     - Disable Feet / Enemy.
     - Disable Feet / Feet.
 - Add an empty GameObject as a child under the HoverGuy.  
   - Name it "Feet".
   - Assign its Layer to Feet.
   - Add a **CircleCollider2D** 
     - Set the radius to .1
     - Position it a little below the sprite.

<img src="http://i.imgur.com/BPohw5V.png" width=150px />

<br>Add a spawner for HoverGuys:

 - Select HoverGuy:
   - Add **DeathEffectSpawn**:
     - GameObject to Spawn: the Explosion prefab
   - Add **KillOnContactWith**:
     - Layers to kill: Player
- Drag in **spritesheet_tiles_43** and then drag in **47**.
   - Order in Layer: -2
 - Add them to a parent named "Door":
   - Scale up the size of the Door to (1.5, 1.5, 1.5).
   - Move the door to the bottom left of the level.
     - Position its Y so that the midpoint of the Door approximately aligns with the midpoint of the HoverGuy (at the height we would want it to spawn).

<img src="http://i.imgur.com/EjVJkZ4.gif" width=300px />

 - Move the sprite for the top into position, then vertex snap the bottom.

<img src="http://i.imgur.com/SF57oFs.gif" width=150px />

 - Create a prefab for 'HoverGuy' and delete the GameObject.
 - Select the Door and add **Spawner**:
   - Thing to spawn: HoverGuy
   - Initial wait time: 10


<hr></details><br>
<details><summary>What did that do?</summary>

Create the HoverGuy:

The HoverGuy animation we created simply kicks its feet around.  We are not going to do anything more with this animation in this tutorial.  But you could use some of the same techniques we did for the character if you want to improve the experience.

The rigidbody and collider enables physics and allows them to stay on platforms.  We freeze the z rotation so the HoverGuy does not fall over.

The collider, layer, and KillOnContactWith replicates the configuration we used for the SpikeBall to kill the character.

DeathEffectSpawn creates an explosion when the HoverGuy is hit by a hammer.

<br>Make the HoverGuy float above the ground:

The second collider we added is configured to collide with platforms but not with the character or other entities. This allows it to prop up the HoverGuy, making it hover above the ground.

We don't want the 'feet' to collide with the character because later in the tutorial we will be adding ladders. While the HoverGuy is on a ladder, the character can walk underneath. If the feet could hit the character he may die unexpectedly.

<br>Add a spawner for HoverGuys:

We added a sprite representing the area where HoverGuys will spawn from.

For simplicity in the Spawner component, the position enemies appear at is the center of the Spawner's GameObject. We attempt to position this for the HoverGuy, and then adjust the door sprites' positions to fit the visible space.

The Spawner added should start to spawn HoverGuys periodically after about 10 seconds into the level.

Note that if the character stands still at the level start, a HoverGuy will spawn and kill him. This will be corrected later.

<hr></details>

<details><summary>How do you know what size to make the second collider?</summary>

It does not matter much.  This second collider's only purpose is to ensure that the HoverGuy hovers above the ground.  So in a sense, we only need a single pixel to represent the correct Y position for Unity physics to use -- represented by the bottom of this circle collider.

Unity physics by default uses discrete collisions instead of continuous. 

 - Discrete means that each FixedUpdate, collisions are considered for the object's current position.
 - Continues means that each FixedUpdate, collisions consider the entire path the object has taken since the last FixedUpdate.

Discrete is is the default because it is more performant.  However Discrete is also less accurate. 

When a collider is too small, collisions may be missed entirely as the object changes from a little above to a little below an obstacle. e.g. this is a common problem when shooting, bullets may start to travel through walls instead of hitting them.

The collider may also be too large, causing our HoverGuy to continue standing on a platform when they should have fallen off the edge.

<hr></details>
<details><summary>Why use a child GameObject instead of two colliders on the parent?</summary>

You could opt to do this using just one GameObject instead.

We are using a child GameObject for the HoverGuy's feet in order to simplify future components.  Specifically we will be created a FloorDetector which will need to know which collider represents the bottom of the object. 

<hr></details>

## 3.5) Make the HoverGuy walk

Add a script to the HoverGuy to drive random walk movement.

<details><summary>How</summary>

 - Create script Components/Movement/**WanderWalkController**:

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class WanderWalkController : MonoBehaviour
{
  [SerializeField]
  float timeBeforeFirstWander = 10;

  [SerializeField]
  float minTimeBetweenReconsideringDirection = 1;

  [SerializeField]
  float maxTimeBetweenReconsideringDirection = 10;

  WalkMovement walkMovement;

  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
  }

  protected void Start()
  {
    StartCoroutine(Wander());
  }

  IEnumerator Wander()
  {
    walkMovement.desiredWalkDirection = 1;
    if(timeBeforeFirstWander > 0) 
    {
      yield return new WaitForSeconds(timeBeforeFirstWander);
    } 

    while(true)
    {
      SelectARandomWalkDirection();
      float timeToSleep = UnityEngine.Random.Range(
        minTimeBetweenReconsideringDirection,
        maxTimeBetweenReconsideringDirection);
      yield return new WaitForSeconds(timeToSleep);
    }
  }

  void SelectARandomWalkDirection()
  {
    walkMovement.desiredWalkDirection
      = UnityEngine.Random.value <= .5f ? 1 : -1;
  }
}
```

 - Add **WanderWalkController** to the HoverGuy (it should automatically add WalkMovement as well).

<hr></details><br>
<details><summary>What did that do?</summary>

WanderWalkController is a controller to drive the WalkMovement component, similar to how the PlayerController does.  

The PlayerController reads input from the keyboard (or controller) and feeds that to WalkMovement.  WanderWalkController uses RNG to effectively do the same, simulating holding the right or left button.

WanderWalkController will always request movement either left or right.  It starts by going right for a period of time and then chooses directions randomly.  You could extend this logic to have the HoverGuy occasionally stand in the same place for a moment before continuing on.

You can configure the walk speed by modifying the WalkMovement component's 'Walk Speed'.

Note that at the moment HoverGuys will walk right off the screen.  This will be addressed soon.

<hr></details>
<details><summary>Why use timeBeforeFirstWander instead of RNG from the start?</summary>

When the HoverGuy first spawns in the bottom left of the world, we always want those enemies to walk to the right.  It would look strange for the enemies to go left and promptly hit the side of the screen before turning around.

When the coroutine starts, we tell WalkMovement to go right and then wait a period of time.  The time we wait before entering the while loop should be configured to be long enough for HoverGuys to reach the first ladder -- maybe even longer.

<hr></details>
<details><summary>Why not set desiredWalkDirection to a random value instead of 1 or -1?</summary>

You could, if it creates the experience you want in the game.  For example:

```csharp
walkMovement.desiredWalkDirection 
  = UnityEngine.Random.Range(-1, 1);
```

This call would achieve the goal of getting the HoverGuy to walk randomly in one direction or the other. desiredWalkDirection is a percent - so 1 means walk at full speed to the right and -1 is full speed to the left.  Using Random.Range will often give you a much smaller value (e.g. .1) and therefore the walk speed in game may appear too slow.

</details>







## 2.18) Test!

Chapter 2, complete!  Your game should now look a lot like the gif at the top.  You can compare to our  [demo build](https://hardlydifficult.com/PlatformerTutorialPart2/index.html) and review the [Unity Project / Source Code for Chapter 2](https://github.com/hardlydifficult/Unity2DPlatformerTutorial/tree/Part2). TODO links

Additionally to review, you may want to:
 - Consider sorting components on the character GameObject, as it's starting to look a little cluttered.
 - Try adjusting the jump speed for the character.
 - Maybe try different particle systems for the explosion death effect.
 - Cut a test build and try it outside of the Unity editor environment.

<details><summary>How do you sort components on a GameObject?</summary>

The order does not impact anything.  So why bother?  Just tidiness really.   As the number of components grows it may be nice to have them presented in an order you find more intuitive.

Start by collapsing everything.
To sort, select the GameObject and in the Inspector

Transform has to be first. Then Unity stuff.  Then scripts.

Unity grouping logically similar components, eg. Rigidbody near Collider

Scripts in order where possible, like DeathEffectManager before any DeathEffects.

<img src="http://i.imgur.com/ElAr8xt.gif" width=150px />

On a related note, order does matter when for some scripts in terms of which component executes before another.  To ma... Script execution order

TODO

</details>








TODO talk about disabling the spawner for debugging




# Next chapter

[Chapter 3](https://github.com/hardlydifficult/Platformer/blob/master/Chapter3.md).