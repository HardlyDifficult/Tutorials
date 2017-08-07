# 5) Animations 

TODO intro

## 5.1) Hammer animation 

Create an animation for the hammer swinging.

<details><summary>How</summary>

 - Open menu Window -> Animation.
 - Select a Hammer.
 - Click create, save as Animations/**HammerSwing**.anim

<img src="http://i.imgur.com/Kokz29S.png" width=300px />

 - Click the red record button.

<img src="http://i.imgur.com/bha8EJC.png" width=150px />

 - Modify the rotation, then set it back to 0, creating a keyframe for the default rotation.
 - Double click under 1:00 to create another keyframe.

<img src="http://i.imgur.com/ZVNovlp.png" width=300px />

 - Switch the current time position (the white line) to 0:10.
 - Change rotation to (0, 0, -90).
 - Click record to stop recording.

<hr></details><br>
<details><summary>What did that do?</summary>

We created an animation for the Hammer, which automatically created the Animator Controller and a default state to play that animation.

If you hit play now, the hammer will be swinging in place.  In the next couple sections we will change this to trigger the animation at the right time.

<hr></details>
<details><summary>Why use a 1:00, what if I want to speed up the animation?</summary>

Unity offers a few different ways you could speed up an animation.  They are all valid, use what you are comfortable with. 

I prefer to get the sequence and relative timing for animation correct using the Animation timeline, and then using the Animator Controller state to modify the playback speed for that animation.  As animations get more complex, making updates to the animation timeline is more tedious which is why I prefer using the 'speed' field.

<hr></details>
<details><summary>How do keyframes work / what happens between keyframes?</summary>

A keyframe is a datapoint on the timeline.  Between each keyframe, Unity will smoothly transition from the previous keyframe to the next.  If you open the "Curves" tab you can see a graph showing how this transition occurs, and you make make modifications there directly.

<hr></details>

## 5.3) Start swinging hammer on equip

Add a script to the hammer to start the swing animation when it's equip.

<details><summary>How</summary>

Stop swinging by default:

 - Select a Hammer.
 - Open menu Window -> Animator.
   - Right click -> Create State -> Empty.  
   - Select the box which appeared and in the Inspector name it "Idle".
   - Right click "Idle" and 'Set as Layer Default State'.


 - Create script Components/Effects/**PlayAnimationOnEnable**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimationOnEnable : MonoBehaviour
{
  [SerializeField]
  string animationToPlay;

  Animator animator;

  protected void Awake()
  {
    animator = GetComponent<Animator>();
  }

  protected void OnEnable()
  {
    animator.Play(animationToPlay);
  }
}
```

 - Select the Hammer prefab:
   - Add **PlayAnimationOnEnable**.
     - Animation to play: "HammerSwing"
     - Disable the PlayAnimationOnEnable component.
   - Add the PlayAnimationOnEnable component to the Hammer component's 'To Enable' list.

<hr></details><br>
<details><summary>What did that do?</summary>

An Animator Controller always requires at least one state be active. We created a state which does nothing and made that the default so the hammer does not move until another component switches the state to HammerSwing, starting the animation.

When the Hammer component is touched by the Character, it will enable the PlayAnimationOnEnable component which starts the swing animation.  

<hr></details>
<details><summary>How does animator.Play work?</summary>

Calling Play on the animator will interrupt the current animation, if there is one, and start playing the one requested.  You pass the name of the Animator State from its Animator Controller, which in turn has a reference to the animation clip to play.  Any parameters defined in the animator state apply, including Speed.

<hr></details>

## 5.4) Character animation parameters

Create parameters to use in the character's Animator Controller and a script to feed the data.

<details><summary>How</summary>

Create animation parameters:

 - Open menu Window -> Animator.
   - Select the character's child sprite GameObject.
   - Switch to the 'Parameters' tab on the left.
   - Click the '+' button and select 'Float'.

<img src="http://i.imgur.com/p6F4gHG.png" width=300px />

 - Name the parameter "Speed".
 - Repeat to create:
   - A bool named 'isTouchingFloor'.
   - A bool named 'isClimbing'.
   - A bool named 'hasWeapon'.


 - In the Animator for the character, select the 'CharacterWalk' state (the orange box).
   - In the Inspector:
     - Adjust the 'Speed' to about '.4'
     - Check the box near 'Multiplier' to enable a 'Parameter'.
       - Confirm Speed is selected (should be the default).

<img src="http://i.imgur.com/9A6mp98.png" width=300px />

<br>Have the Character sync animation parameters:

 - Create script Components/Animations/**PlayerAnimatorController**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))]
[RequireComponent(typeof(WeaponHolder))]
public class PlayerAnimatorController : MonoBehaviour
{
  Animator animator;

  Rigidbody2D myBody;

  LadderMovement ladderMovement;

  FloorDetector floorDetector;

  WeaponHolder weaponHolder;

  protected void Awake()
  {
    animator = GetComponentInChildren<Animator>();
    myBody = GetComponent<Rigidbody2D>();
    ladderMovement = GetComponent<LadderMovement>();
    floorDetector = GetComponentInChildren<FloorDetector>();
    weaponHolder = GetComponent<WeaponHolder>();
  }

  protected void Update() TODO late update
  {
    animator.SetFloat("Speed", myBody.velocity.magnitude);
    animator.SetBool("isTouchingFloor", floorDetector.isTouchingFloor);
    animator.SetBool("isClimbing", ladderMovement.isOnLadder);
    animator.SetBool("hasWeapon", weaponHolder.currentWeapon != null);
  }
}
```

 - Add **PlayerAnimatorController** to the Character.



<hr></details><br>
<details><summary>What did that do</summary>

<br>Create animation parameters:

The parameters we are creating will be used to cause the Animator controller to transition from one state to another.  This approach is an alternative to playing the animation state directly like we had done for the Hammer above.  

The speed parameter will also be used to scale the animation playback speed based off how quickly the entity is moving at the time.

<br>Walk speed

This slows the character's walk animation and gradually turns it on and off as the character starts and stops moving.

Now the character's walk animation should align with the moment a little better.  Adjust the value to something you think looks good. However the walk animation also plays while jumping, we'll address this next.

<img src="http://i.imgur.com/2dfN2RE.gif" width=300px />

<br>Have the Character sync animation parameters:

The PlayerAnimatorController is simply forwarding information from various components to the Animator controller.

<hr></details>
<details><summary>When do you use Animator Controller parameters vs Play(state) to change animations?</summary>

It's up to you.  Both approaches have the same capabilities, but by using animation parameters you can let the Animator Controller own much of the logic - simplifying your code and debugging.

I prefer to use Play for simple objects like the Hammer, and use animation parameters for more complex ones like entities.

You can also use a combination of the two approaches.  Calling Play will change the current Animator State, and from there any transitions from that state will be considered.

<hr></details>
<details><summary>Why LateUpdate instead of Update or FixedUpdate?<summary>

TODO

</details>
<details><summary>What unit/scale is speed defined in?</summary>

Percent.  1 represents the speed as it was defined in the animation itself.  Going to 2 would double the playback speed, going to .5 would cut the playback speed in half.

<hr></details>
<details><summary>How does the Multiplier Parameter work?</summary>

Various settings for the animator state may be modified with one of the parameters we define in the Animator Controller.  Here we are using speed with a default value of .4.  When the animation is playing, the animation playback speed is multiplied by the Speed parameter (which is the velocity magnitude) - so if we are not moving the animation actually pauses, and it slows down / speeds up with our movement.

<hr></details>

## 5.7) Jump animation

Add an animation to the character for jumping. 

<details><summary>How</summary>

Jump animation:

 - Select the character's sprite and in the Animation window, create a new clip Animations/**CharacterJump**:
   - Select the sprites for the jump animation. We are using **adverturer_spritesheet_7** and **8**.
   - Drag and drop the sprites onto the Animation timeline.

<img src="http://i.imgur.com/0rHCGDm.gif" width=300px />

 - In the Animator window:
   - Select the CharacterJump state:
     - Speed: .05
     - Check to use the Speed Multiplier Parameter: 'Speed'

<br>Transition to jump:

   - Right click on the 'Any State' box and select 'Make Transition'.
     - An arrow will follow your mouse, click on the CharacterJump state to create the transition.

<img src="http://i.imgur.com/Fl0WTPO.gif" width=300px />

 - Select the transition arrow just created, in the Inspector click the plus to create a new condition.

<img src="http://i.imgur.com/WgOfzQY.png" width=150px />

 - Change the condition to read 'isTouchingFloor false'.
 - Under 'Settings':
   - Transition Duration: 0
   - Uncheck 'Can Transition to Self'
 - Create a transition from CharacterJump to CharacterWalk.
 - Select the transition just created:
   - Add a condition: isTouchingFloor true
   - Uncheck 'Has Exit Time'
   - Transition Duration: 0

<hr></details><br>
<details><summary>What did that do?</summary>

Jump animation:

A jump animation for the character was added which simply kicks his feet.  By default this is way to fast, we slow it down and multiply by the entity's current speed.

<br>Transition to jump:

As you jump, the character should kick his feet a bit and then resume walking when he lands.  We do this with Transitions in the Animator Controller.  These transitions are driven by conditions, checking the parameters we have populated with the PlayerAnimatorController.

<hr></details>
<details><summary>Why transition from Any State instead of from CharacterWalk?</summary>

Any State is a special 'state' in the Animator Controller, allowing you to define transitions which could happen at any time.

You could create this transition from the CharacterWalk state instead.  However I am using Any State because as we add more animations for the character, we won't need to define as many total transitions.

<hr></details>
<details><summary>How do animation conditions work?</summary>

For transitions with one or more conditions, we change states when all conditions are met.  This could be a single parameter such as the bool we are using here, or it could be a combination of factors.

<hr></details>
<details><summary>What does the Transition Duration impact?</summary>

Once the conditions are met, the transition from one state to the other completes in the 'Transition Duration' time.  This is a great feature for 3D models as the Unity animator will smooth the transition from one stance to another.  However for sprites, there is no smoothing so we typically want a transition duration of 0.

<hr></details>
<details><summary>What does the Can Transition to Self impact?</summary>

When creating a transition from Any State, an option for Can Transition to Self is available.  

 - Checked (the default): This transition applies even when in the target state.  In this example, since the condition is just a bool check and there is no Exit Time - transition to self would cause the animation to keep starting over.
 - Unchecked: This transition effectively does not exist while in the target state.  e.g. I can't jump restart jumping while jumping.

<hr></details>
<details><summary>What does Exit Time do and how does it relate to conditions?</summary>

Has Exit Time is an additional way of triggering a transition.  So if a transition has both Has Exit Time and Conditions defined, the transition occurs when **either** the time has passed **or** the conditions are true.

<hr></details>


## 5.8) Additional character animations

Add an animation for when climbing ladders and while idle.

<details><summary>How</summary>

Climb animation:

 - Create a new animation for the character Animations/**CharacterClimb**.anim
   - Drag in the sprites for the climb animation.  We are using **adverturer_spritesheet_5** and **6**.
 - Open the character's Animator Controller:
   - Select the CharacterClimb state and use the Speed parameter times .1
   - Create a transition from Any State to CharacterClimb.
     - Condition: isClimbing true
   - Create a transition from CharacterClimb to CharacterWalk.
     - Uncheck Has Exit Time
     - Transition Duration: 0
     - Uncheck Can Transition to Self
     - Condition: isClimbing false
   - Select the transition from Any State to CharacterJump
     - Condition: isClimbing false

<br>Idle animation:

 - Create a new animation for the character Animations/**CharacterIdle**.anim
   - Click record
     - Change the 'Sprite' under the character's Sprite Renderer component to an idle stance. We are using **adventurer_tilesheet_0**.
     - Double click to create a keyframe at 1:00.
     - Switch the current time position to 0:30.
       - This will move the white line, indicating where in the timeline modifications will be made. 
     - Set the Transform scale to (1, .95, 1).
     - Switch the time to 1:00 and set the Transform scale to (1, 1, 1).
     - Then stop recording.   
 - Open the character's Animator Controller:
   - In the Animator, create a transition from CharacterWalk to CharacterIdle:
     - Uncheck Has Exit Time
     - Transition Duration: 0
     - Condition: 'Speed' is 'Less' than '.1'
   - Make a transition from CharacterIdle to CharacterWalk:
     - Uncheck Has Exit Time
     - Transition Duration: 0
     - Condition: 'Speed' is 'Greater' than '.1'

<br>Breakdance animation:

 - Create a new animation for the character Animations/**CharacterDance**.anim
   - Select all the sprites for this animation and drag them into the timeline. We are using **adventurer_tilesheet_11** **- 21** (10 sprites).
 - Open the character's Animator Controller:
   - Change the CharacterDance speed to '.1'
   - Create a transition from CharacterIdle to CharacterDance.
     - Exit Time: 3
     - Transition Duration: 0
   - Create a transition from CharacterDance to CharacterIdle.
     - Transition Duration: 0
   - Create a transition from CharacterDance to CharacterWalk.
     - Uncheck 'Has Exit Time'
     - Transition Duration: 0
     - Condition: 'Speed' is 'Greater' than '.1'

<hr></details><br>
<details><summary>What did that do?</summary>

Climb animation:

A climb animation for the character was added. As you climb up or down a ladder, the character should move his arms and then resume walking when he gets off.

The animation created works just like the original walk animation we created for the character.  By default the speed is way too fast so we turn this down in the Animator Controller.

<br>Idle animation:

When the character is not moving, he will switch to the idle stance.  We also modify the scale, from 1 to .95 on the Y and then back to 1 to create the breathing effect.  

Hit play so see the character switch between walking and standing:

<img src="http://i.imgur.com/YjZ1zrE.gif" width=300px />

<br>Breakdance animation:

The character will dance after standing still for a few seconds.

We use Exit Time to define how long the character should be in the CharacterIdle state before dancing.  If you start to walk during the dance, he will switch to the walk animation right away.

<img src="http://i.imgur.com/t7cUVPI.gif" width=300px />

<hr></details>


## 5.12) Add an intro timeline

Create a timeline which enables the LevelController and Hammers after the intro is complete.

Create an animation for the cloud entrance at the start of the level.

<details><summary>How</summary>

Create an intro animation for the cloud:

 - Create an animation for the EvilCloud sprite Animations/**CloudLevel1Entrance**.anim
   - Click record:
     - Start by moving the cloud off screen.
     - Then over time, modify its position to create a dramatic entrance.
 - Select Animations/CloudLevel1Entrance:
   - In the Inspector uncheck 'Loop Time'.

<br>Create an intro Timeline:

 - Select the EvilCloud's sprite.
 - Open menu Window -> Timeline Editor.
   - Click 'Create'.  Save as Assets/Animations/**Level1Entrance**.
   - Select 'Add from Animation Clip' and select CloudLevel1Entrance.

<img src="http://i.imgur.com/7HXZs7Z.gif" width=300px />

 - Drag the parent Hammers GameObject (which holds all the hammers) onto the timeline and select **Activation Track**.
   - Move the box for the script so that it starts after the cloud animation completes.  
     - The start of the box represents when it will be enabled.
     - The end must align with the end of the time timeline to prevent it from being disabled.

<img src="http://i.imgur.com/6XyJZlh.gif" width=300px />

 - Repeat, creating activation tracks for the LevelController and the Ladders.

<hr></details><br>
<details><summary>What did that do?</summary>

Create an intro animation for the cloud:

We created an entrance for the cloud to play at the start of the level.  This is just for design, so do whatever you like here.  We'll get the spawners to hold until the animation completes in a bit.  

Our animation looks like this at the moment:

<img src="http://i.imgur.com/o40dfEx.gif" width=300px>

<br>Create an intro Timeline:

A Timeline on the EvilCloud is used to coordinate the intro sequence across objects. 

 - It plays the intro animation on the EvilCloud.
 - The Hammers and Ladders are hidden until we start their FadeInThenEnable script with an Activation Track, after the intro animation completes.
 - The Character is spawned after the intro by the Level Manager with an Activation Track.

<hr></details>
<details><summary>What is a Unity Timeline / Activation Track?</summary>

Timeline is a new feature released with Unity 2017.  It's a higher level component than the Animator Controller, used to coordinate animations and trigger events across several objects in the scene with an interface that resembles the Animation timeline.

Previously, achieving similiar results would have required a script.  Now you can manage the sequence visually if you prefer.

'Add Animation From Clip' plays an animation during the timeframe specified, overriding what the Animator controller for that object would have done.

Activation Tracks are one of several ways that you trigger behaviour with the Timeline.  An activation track will enable a GameObject where the track begins in the timeline, and disable it again where it ends.  If the activation track ends at the very end of the entire timeline then it will remain active after the timeline completes.

<hr></details>
<details><summary>How might we do this without using the Timeline Editor?</summary>

There are always alternative ways to achieve a goal, particularly true in this case since the Timeline Editor is brand new.

An alternative solution might be something like this:

 - For the EvilCloud, simply play the intro animation with a default state in the Animator Controller.
 - Add a 'InvisibleFor' value to the FadeInThenEnable script, and time that to coordinate with the intro.
 - Add an initial sleep time to the spawner to align with the intro animation.

The advantage to using the Timeline is as you make adjustments to the sequence, you can make those changes visually and aligning the time between objects may be easier.

<hr</details>


## 5.13) Disable spawners till the intro is complete

Disable the spawners and create a script to later enable them when the level intro completes.

<details><summary>How</summary>

Enable components when a Timeline event occurs:

 - Create script Components/Animations/**EnableComponentsOnTimelineEvent**:
   - Note there will be compile issues until TimelineEventPlayable is added.

```csharp
using UnityEngine;

public class EnableComponentsOnTimelineEvent : MonoBehaviour
{
  [SerializeField]
  TimelineEventPlayable.EventType eventType;

  [SerializeField]
  MonoBehaviour[] componentList;

  public void OnEvent(
    TimelineEventPlayable.EventType currentEventType)
  {
    if(currentEventType == eventType)
    {
      EnableComponents(componentList);
    }
  }
  
  static void EnableComponents(
    MonoBehaviour[] componentList)
  {
    for(int i = 0; i < componentList.Length; i++)
    {
      MonoBehaviour component = componentList[i];
      component.enabled = true;
    }
  }
}
```

 - For both the cloud and door:
   - Disable the **Spawner** component.
   - Add **EnableComponentsOnTimelineEvent**.
     - Add the Spawner component to its list.

<br>Create a Timeline event:

 - Create script Playables/**TimelineEventPlayable**:

```csharp
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineEventPlayable : BasicPlayableBehaviour
{
  public enum EventType
  {
    AlmostAtStart, Start, End
  }

  [SerializeField]
  EventType eventType;

  public override void OnBehaviourPlay(
    Playable playable,
    FrameData info)
  {
    base.OnBehaviourPlay(playable, info);

    EnableComponentsOnTimelineEvent[] componentList
      = GameObject.FindObjectsOfType<EnableComponentsOnTimelineEvent>();

    for(int i = 0; i < componentList.Length; i++)
    {
      EnableComponentsOnTimelineEvent component = componentList[i];
      component.OnEvent(eventType);
    }
  }
}
```

 - Drag drop the script into the timeline.  Set the time like we did for the Hammers.
   - In the Inspector, change the 'Event Type' to 'Start'.
 - Drag the script in a second time and set the time to fire a bit before the animation ends.

<img src="http://i.imgur.com/AYkG3Jc.png" width=500px />

<hr></details><br>
<details><summary>What did that do?</summary>

Enable components when a Timeline event occurs:

EnableComponentsOnLevelLoad is used to enable specific components during the intro sequence (as opposed to the entire GameObject).  There are three event types supported:

 - Almost At Start: fired a few moments before the end of the intro.
 - Start: fired once the intro sequence is complete.
 - End: fired once the player has beat the level.

We add this to EvilCloud and the door so that their sprites are visible but the spawners are not enabled until the intro animations completes.

<br>Create a Timeline event:
 
TimelineEventPlayable is the component which we add to the Timeline to call each of the EnableComponentsOnLevelLoad in the scene.  We add this twice to the intro Timeline, one for the the 'Almost at start' event and another for the 'Start' event.

<hr></details>
<details><summary>What is a BasicPlayableBehaviour / when is OnBehaviourPlay called?</summary>

A BasicPlayableBehaviour is like a MonoBehaviour but for scripts to be used in the Timeline (vs on a GameObject directly).

OnBehaviourPlay is a Unity event called when the script begins on the timeline.  Note that here Unity uses override instead of the reflection pattern used with MonoBehaviour events.

<hr></details>
<details><summary>What's a C# 'enum'?</summary>

An enum is a set of named constants.  The constants are by default type int and count sequentially starting from 0.  For example:

```csharp
enum Example 
{
  A, B, C
}
```

is similiar to

```csharp
const int A = 0;
const int B = 1;
const int C = 2;
```

Enums are often used to bring a related set of constants together.  They have some additional benefits over listing the constants individually such as:

 - You can iterate all possible values using System.Enum.GetValues.
 - You can use ToString to get the named value.
 - Clarifies intent, making it easier to know what values should be accepted.

Consider using an enum if the set of values is known at compile time.

<hr></details>


## 5.14) Rotate platforms during intro

Platforms start out straight and then when the intro animation is nearly complete, shake down into position.

<details><summary>How</summary>

 - Create script Components/Movement/**RotateOvertimeToOriginal**:

```csharp
using System.Collections;
using UnityEngine;

public class RotateOvertimeToOriginal : MonoBehaviour
{
  [SerializeField]
  float rotationFactor = 1;

  [SerializeField]
  float maxTimeBetweenRotations = .25f;

  Quaternion targetRotation;

  protected void Awake()
  {
    targetRotation = transform.rotation;
    transform.rotation = Quaternion.identity;
  }

  protected void Start()
  {
    StartCoroutine(AnimateRotation());
  }

  IEnumerator AnimateRotation()
  {
    float percentComplete = 0;
    float sleepTimeLastFrame = 0;
    while(true)
    {
      sleepTimeLastFrame 
        = UnityEngine.Random.Range(0, maxTimeBetweenRotations);
      yield return new WaitForSeconds(sleepTimeLastFrame);
      sleepTimeLastFrame = Mathf.Max(Time.deltaTime, sleepTimeLastFrame);

      float percentCompleteThisFrame = sleepTimeLastFrame * rotationFactor;
      percentCompleteThisFrame *= UnityEngine.Random.Range(0, 10);
      percentComplete += percentCompleteThisFrame;
      if(percentComplete >= 1)
      {
        transform.rotation = targetRotation;
        yield break;
      }
      transform.rotation = Quaternion.Lerp(
        Quaternion.identity, 
        targetRotation, 
        percentComplete);
    }
  }
}
```

 - For each Platform:
   - Add **RotateOvertimeToOriginal**:
     - Disable the component.
   - Add **EnableComponentsOnLevelLoad**:
     - Add RotateOvertimeToOriginal to the 'Components to enable on almost loaded'.

<hr></details><br>
<details><summary>What did that do?</summary>

When the level begins, RotateOvertimeToOriginal stores the object's original rotation (as it was placed in the scene).  We then change the rotation before the first render on-screen to Quaternion.identity, or the default rotation for the sprite.

A coroutine periodically lerps rotation back to the original.  We use RNG, both for a random sleep time between rotation changes and to randomize how much the rotation changes by.  Our goal here is to make it not smooth, as if it were falling / shaking into place.

<hr></details>
<details><summary>What's C# yield break do?</summary>

Enumerators are methods which can 'yield return' and then later be resumed from where they left off.  Coroutines in Unity are enumerators.  

When working with enumerators, 'yield break' will return from the method and indicate that it's complete and cannot be resumed again.

<hr></details>

## 5.15) Add screen shake during intro

Shake the screen when the platforms fall into place.

<details><summary>How</summary>

 - Create script Components/Animations/**ScreenShake**:

```csharp
using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
  [SerializeField]
  float timeToShakeFor = 1;

  [SerializeField]
  float maxTimeBetweenShakes = .2f;

  [SerializeField]
  float shakeMagnitude = 1;

  protected void Start()
  {
    StartCoroutine(ShakeCamera());
  }

  IEnumerator ShakeCamera()
  {
    Camera camera = Camera.main;
    Vector3 startingPosition = camera.transform.position;

    float timePassed = 0;
    while(timePassed < timeToShakeFor)
    {
      float percentComplete = timePassed / timeToShakeFor;
      percentComplete *= 2;
      if(percentComplete > 1)
      {
        percentComplete = 2 - percentComplete;
      }
      Vector2 deltaPosition 
        = UnityEngine.Random.insideUnitCircle * shakeMagnitude * percentComplete;
      camera.transform.position = startingPosition + (Vector3)deltaPosition;

      float maxTime = maxTimeBetweenShakes * (1 - percentComplete);
      float sleepTime 
        = UnityEngine.Random.Range(0, maxTime);
      yield return new WaitForSeconds(sleepTime);
      sleepTime = Mathf.Max(Time.deltaTime, sleepTime);
      timePassed += sleepTime;
    }

    camera.transform.position = startingPosition;
  }
}
```

 - Select the camera:
   - Add **ScreenShake** and disable the component.
   - Add **EnableComponentsOnLevelLoad**.
     - Event Type: Almost At Start
     - Add its ScreenShake component to the component list.

<hr></details><br>
<details><summary>What did that do?</summary>

ScreenShake moves the camera up/down/left/right randomly to create a shaking effect.  The effect lasts for a limited time and scales up in magnitude (i.e. the intensity of the shake) and then back down.  This component is enabled when the intro Timeline is almost complete, and that event aligns with the cloud bouncing - making it look like the cloud is shaking the platforms into place.

<hr></details>
<details><summary>What is the script doing with percent complete?</summary>

Our goal is to smoothly transition from 0 to 1 and then back to 0.  We use this value as a multiple on how much we move the camera that frame - smoothing the start and end of the effect.

We do this by doubling the percent complete and then if greater than 1, use 2 - the value.  This gives us the desired 0 -> 1 -> 0 curve.

<hr></details>
<details><summary>What does Random.insideUnitCircle do?</summary>

Random.insideUnitCircle is a convenience method giving you a random point which falls on a circle with a radius of 1.  We take that value and then multiple it by the desired magnitude, effectively giving us a random point on a larger, or smaller, circle; and then position that the camera that far from its original position.

<hr></details>
<details><summary>What else could we add to the shake effect?</summary>

Here are a few ideas on how you might be able to make this effect even cooler:

 - Randomly change the z Rotation in addition to the position.
 - Randomly change the orthographic size, causing the camera to zoom in and out.
 - The current shake algorithm is uses a random offset from the camera's original position, you may be able to improve the effect by giving consideration to the camera position the previous frame.
 - Add a post processing effect such as blur.  Post processing effects refer to scripts you can add to your camera, modifying the display to create an effect such as blur or bloom.  Here are some [post processing effects, free from Unity](https://www.assetstore.unity3d.com/en/#!/content/83912), you can use.

<hr></details>


## Add a win condition

The goal of the game is to save the beautiful mushroom.  For level 1, that means getting close - but before you actually reach it the EvilCloud is going to carry the mushroom up to level 2.  

Here we detect the end of the game, the cloud animation will be added later in the tutorial.

<details><summary>How</summary>

Design the win area:

 - Create an empty GameObject named "WinArea".
   - Add a **BoxCollider2D** sized to cover the area that when entered will end the level.
     - Check Is Trigger.
   - Create a Layer "WinArea":
     - Configure the collision matrix to only support WinArea <-> Player collisions.
     - Assign the layer to the WinArea GameObject.
   - Add a sprite to lure the character to the win area.  We are using **spritesheet_jumper_26** with Order in Layer -3.
     - Make it a child of the WinArea. 

<img src="http://i.imgur.com/WuW9hPk.png" width=300px />

<br>Inform the LevelController when the player won:

 - Create script Components/Effects/**TouchMeToWin**:

```csharp
using System;
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  static int totalNumberActive;

  [SerializeField]
  MonoBehaviour componentToEnableOnTouch;

  int playerLayer;

  protected void Awake()
  {
    playerLayer = LayerMask.NameToLayer("Player");
  }

  protected void OnEnable()
  {
    totalNumberActive++;
  }

  protected void OnDisable()
  {
    totalNumberActive--;
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(enabled == false 
      || collision.gameObject.layer != playerLayer)
    {
      return;
    }

    if(componentToEnableOnTouch != null)
    {
      componentToEnableOnTouch.enabled = true;
    }

    enabled = false;
    if(totalNumberActive == 0)
    {
      GameObject.FindObjectOfType<LevelController>().YouWin();
    }
  }
}
```

 - Add **TouchMeToWin** to the WinArea.

<hr></details><br>
<details><summary>What did that do?</summary>

Design the win area:

We put a large trigger collider around the mushroom.  When the character enters this area, it will trigger the end the level.  The collider is configured to use a layer which only interacts with the player so enemies cannot accidentally end the level.

<br>Inform the LevelController when the player won:

TouchMeToWin counts the total number of these special zones in the world.  For level 1 we are only using one but for level 2 there will be more.  When the last one is disabled (by the character entering that area), we call YouWin on the LevelController which will own starting the end sequence / switching to level 2.

An enabled check is included to ensure we an area does not call YouWin multiple times.

<hr></details>


## Win animation

When the character reaches the win area, play a Timeline to animate the end of the level.

<details><summary>How</summary>

Create a win animation:

 - Create another animation for the EvilCloud, Animations/**CloudLevel1Exit** to play when the player wins.
   - You may not be able to record if the Timeline Editor window is open.
   - Select Animations/CloudLevel1Exit and disable Loop Time.

<br>Create a win Timeline:

 - Right click in Assets/Animations -> Create -> Timeline named **Level2Exit**.
   - Select the EvilCloud's sprite GameObject and in the Inspector change the Playable Director's 'Playable' to Level2Exit.

<img src="http://i.imgur.com/Jsah6Ll.png" width=300px />

 - In the Timeline Editor window, click 'Add' then 'Animation Track' and select the EvilCloud's child GameObject with the animator.
 - Right click in the timeline and 'Add Animation From Clip' and select the CloudLevel1Exit animation.

<img src="http://i.imgur.com/xcR7HWr.gif" width=300px />

 - Select the box which appeared for the animation, and in the Inspector modify the speed.
   - Hit play in the Timeline Editor to preview the speed.  The value is going to depend on how you created the animation.

<br>Hide the mushroom during the animation:

 - Select the mushroom GameObject and drag it into the timeline.
   - Adjust the timeframe so that it starts at the beginning of the timeline and ends when you want the mushroom to disappear.
   - Select the track's row and in the Inspector change the 'Post-playback state' to 'Inactive'.

<img src="http://i.imgur.com/W9lejAB.png" width=300px />

 - Select the EvilCloud's sprite GameObject and in the Inspector change the Playable Director's Playable back to Level1Entrance.

<br>Start the Timeline at the end of the level:

 - Update **LevelController**:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;
```

</details>

```csharp
using UnityEngine.Playables; 
```

<details><summary>Existing code</summary>

```csharp
public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;
```

</details>

```csharp
  [SerializeField]
  PlayableDirector director; 

  [SerializeField]
  PlayableAsset TimelineEventPlayable; 
```

<details><summary>Existing code</summary>

```csharp
  [SerializeField]
  int levelNumber = 1; 

  protected void OnEnable()
  {
    GameController.instance.onLifeCounterChange
      += Instance_onLifeCounterChange;

    StartLevel();
  }
  
  protected void OnDisable()
  {
    GameController.instance.onLifeCounterChange
      -= Instance_onLifeCounterChange;
  }

  void Instance_onLifeCounterChange()
  {
    if(isGameOver)
    {
      return;
    }

    BroadcastEndOfLevel();
 
    if(GameController.instance.lifeCounter <= 0)
    {
      isGameOver = true;
      YouLose();
    }
    else
    {
      StartLevel();
    }
  }

  public void YouWin()
  {
    if(isGameOver == true)
    {
      return;
    }

    isGameOver = true;
```

</details>

```csharp
    director.Play(TimelineEventPlayable); 
```

<details><summary>Existing code</summary>

```csharp
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
  }

  void StartLevel()
  {
    Instantiate(playerPrefab);
  }

  void BroadcastEndOfLevel()
  {
    PlayerDeathMonoBehaviour[] gameObjectList 
      = GameObject.FindObjectsOfType<PlayerDeathMonoBehaviour>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      PlayerDeathMonoBehaviour playerDeath = gameObjectList[i];
      playerDeath.OnPlayerDeath();
    }

  }

  void YouLose()
  {
    // TODO
  }
}
```

</details>

 - Configure the director and set the end of level playable to Level1Exit.

<hr></details><br>
<details><summary>What did that do?</summary>

Create a win animation:

Another animation was created to play when the player wins.  We leave it up to you what this looks like and how long the animation plays for.  

<br>Create a win Timeline:

A new Timeline is created for the win sequence.  We add the animation just created and adjust the speed as needed.

<br>Hide the mushroom during the animation:

An Activation Track is used to hide the mushroom when the animation is nearly complete.  Setting the post-playback state to inactive ensures that the mushroom does not return when the Timeline completes.

<br>Start the Timeline at the end of the level:

When the win condition is triggered, the LevelController changes the EvilCloud's Playable Director to play the end of level Timeline just created.

<hr></details>
<details><summary>Why switch the Playable when editing Timelines?</summary>

Unity 2017 is the first release of Timeline, it's still a work in progress.  

At the moment you cannot edit Timelines unless they are active in the scene.  You can only partially view the Timeline by selecting the file.  So anytime you want to modify the Level1Exit Timeline, you need to change the Playable Director and then when you are complete change it back.

On a related note, you can't edit an animation if the Timeline window is open.  When working with Animations and Timelines, it seems to work best if you only have one open at a time.

<hr></details>

## Stop everything when the level is over

When the level is over, stop the spawners and freeze the character and enemies while the EvilCloud animation plays.

<details><summary>How</summary>

Create a script to disable certain mechanics:

 - Create script Components/Controllers/**DisableComponentsOnEndOfLevel**:

```csharp
using UnityEngine;

public class DisableComponentsOnEndOfLevel : MonoBehaviour
{
  [SerializeField]
  Component[] componentsToDisable;

  public void OnEndOfLevel()
  {
    for(int i = 0; i < componentsToDisable.Length; i++)
    {
      Component component = componentsToDisable[i];
      if(component is Rigidbody2D)
      {
        Rigidbody2D myBody = (Rigidbody2D)component;
        myBody.simulated = false;
      }
      else if(component is Behaviour)
      {
        Behaviour behaviour = (Behaviour)component;
        behaviour.enabled = false;
        if(behaviour is MonoBehaviour)
        {
          MonoBehaviour monoBehaviour = (MonoBehaviour)behaviour;
          monoBehaviour.StopAllCoroutines();
        }
      }
      else
      {
        Destroy(component);
      }
    }
  }
}
```

<br>Configure disabling for GameObjects:

 - Select the Character prefab.
   - Add **DisableComponentsOnEndOfLevel** and to the components list, add 3 items:
     - Its Rigidbody2D.
     - Its PlayerController.
     - The character's animator (which is on the child GameObject).  You can do this by:
       - Open a second Inspector by right click on the Inspector tab and select Add Tab -> Inspector.
       - With the Character's parent GameObject selected, hit the lock symbol in one of the Inspectors.
       - Select the character's child sprite, then drag the Animator from one Inspector into the other.

<img src="http://i.imgur.com/UOEJNyx.gif" width=500px />

 - Unlock the Inspector.
 - Select the HoverGuy prefab.
   - Add **DisableComponentsOnEndOfLevel**, and add its Rigidbody2D and Animator.
 - Select the SpikeBall prefab.
   - Add **DisableComponentsOnEndOfLevel** and add its Rigidbody2D.
 - For the EvilCloud and the Door:
   - Add **DisableComponentsOnEndOfLevel** and add its Spawner.

<br>Call scripts at the end of the level:

 - Update Components/Controllers/**LevelController**:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;

public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;

  [SerializeField]
  int levelNumber = 1; 

  protected void OnEnable()
  {
    GameController.instance.onLifeCounterChange
      += Instance_onLifeCounterChange;

    StartLevel();
  }
  
  protected void OnDisable()
  {
    GameController.instance.onLifeCounterChange
      -= Instance_onLifeCounterChange;
  }

  void Instance_onLifeCounterChange()
  {
    if(isGameOver)
    {
      return;
    }

    BroadcastEndOfLevel();
 
    if(GameController.instance.lifeCounter <= 0)
    {
      isGameOver = true;
      YouLose();
    }
    else
    {
      StartLevel();
    }
  }

  public void YouWin()
  {
    if(isGameOver == true)
    { 
      return;
    }

    isGameOver = true;

    director.Play(TimelineEventPlayable);
```

</details>

```csharp
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
```

<details><summary>Existing code</summary>

```csharp
  }

  void StartLevel()
  {
    Instantiate(playerPrefab);
  }

  void BroadcastEndOfLevel()
  {
    PlayerDeathMonoBehaviour[] gameObjectList 
      = GameObject.FindObjectsOfType<PlayerDeathMonoBehaviour>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      PlayerDeathMonoBehaviour playerDeath = gameObjectList[i];
      playerDeath.OnPlayerDeath();
    }
  }

  void YouLose()
  {
    // TODO
  }
}
```

</details>

<hr></details><br>
<details><summary>What did that do?</summary>

Create a script to disable certain mechanics:

This script exposes a public method to be called when the level ends.  It will disable a list of components, typically on the same GameObject or a child GameObject.

Depending on the type of component, our approach to 'disabling' differs.

<br>Configure disabling for GameObjects:

At the end of the level, the LevelController will call each DisableComponentsOnEndOfLevel component. This component then disables other components on the GameObject to make the game freeze during our end of level animation.

 - Entities disable their rigidbody to stop gravity and the animator to stop playback.
 - The Character also disables the PlayerController so that input does not cause the sprite to flip facing direction.
 - Spawners stop the spawn coroutine so no more enemies appear.

<br>Call scripts at the end of the level:

When the LevelController detects the win condition, it's updated to call each of the DisableComponentsOnEndOfLevel components in the scene.

<hr></details>
<details><summary>Why not just set timeScale to 0?</summary>

You could, but some things would need to change a bit.

We don't want everything to pause.  The EvilCloud animation needs to progress.  If you change the timeScale, you will need to modify the Animators to use Unscaled time -- otherwise the animations would not play until time resumed.

<hr></details>
<details><summary>Why not just destroy all the components instead?</summary>

Destroying a component is an option.  Once destroyed, that component stops but the rest of the GameObject is still in-tact.

Errors occur if we attempt to destroy the components mentioned above due to other components requiring the ones we removed.  If we wanted to switch to destroying components instead, we would need to be more selective in which components are included to avoid dependency issues.  Because of this, it's simpler to disable than destroy.

<hr></details>
<details><summary>What's rigidbody simulated=false do?</summary>

Setting simulated to false on the rigidbody effectively disables the component.  The rigidbody does not support an 'enabled' flag like scripts do - 'simulated' is their equivalent.

<hr></details>
<details><summary>What's the lock symbol do?</summary>

Many of the windows in Unity have a lock symbol in the top right.  Clicking this will freeze the selection for that window.  So if you select a GameObject you can freeze the Inspector, allowing you to continue navigating other files while still having that same GameObject's properties displayed in the Inspector.

This is handy for various things such as above where we want one GameObject to reference another GameObject's component.  Open two Inspectors, select the first GameObject and lock one of the Inspector windows... now you can select the other GameObject and you have one Inspector for each.

<hr></details>


## 5.16) Test

TODO

# Next chapter

[Chapter 6](https://github.com/hardlydifficult/Platformer/blob/master/Chapter6.md).
