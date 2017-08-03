# 5) Animations 

TODO intro


## 5.1) Hammer animation 

Create an animation for the hammer swinging.

<details><summary>How</summary>

 - Open menu Window -> Animation.
 - Select a hammer.
 - Click create, save as Assets/Animations/**HammerSwing**.

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

We created an animation for the Hammer, which automatically created the Animation Controller and a default state to play that animation.

If you hit play now, the hammer will be swinging in place.  In the next couple sections we will change this to trigger the animation at the right time.

<hr></details>
<details><summary>Why use a 1:00, what if I want to speed up the animation?</summary>

Unity offers a few different ways you could speed up an animation.  They are all valid, use what you are comfortable with. 

I prefer to get the sequence and relative timing for animation correct using the Animation timeline, and then using the Animation controller state to modify the playback speed for that animation.  As animations get more complex, making updates to the animation timeline is more tedious which is why I prefer using the 'speed' field.

<hr></details>

## 5.2) Stop swinging by default

Update the hammer animator to not play any animation by default.

<details><summary>How</summary>

 - Select a Hammer.
 - Open menu Window -> Animator.
   - Right click -> Create State -> Empty.  
   - Select the box which appeared and in the Inspector name it "Idle".
   - Right click "Idle" and 'Set as Layer Default State'.

<hr></details><br>
<details><summary>What did that do?</summary>

An Animation controller always requires at least one state be active. We created a state which does nothing and made that the default so the hammer does not move until another component starts the animation.

<hr></details>


## 5.3) Start swinging hammer on equip

Add a script to the hammer to start the swing animation when it's equip.

<details><summary>How</summary>

 - Create script Code/Components/Effects/**PlayAnimationOnEnable**:

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
     - Set the animation to play to "HammerSwing".
     - Disable the PlayAnimationOnEnable component
   - Add the PlayAnimationOnEnable component to the Hammer component's 'To Enable' list.

<hr></details><br>
<details><summary>What did that do?</summary>

When the Hammer component is touched by the Character, it will enable the PlayAnimationOnEnable component which starts the swing animation.  

<hr></details>

## 5.4) Character animation parameters

Create parameters to use in the character's animation controller and a script to feed the data.

<details><summary>How</summary>

 - Open menu Window -> Animator.
   - Select the character's child sprite GameObject.
   - Switch to the 'Parameters' tab on the left.
   - Click the '+' button and select 'Float'.

<img src="http://i.imgur.com/p6F4gHG.png" width=150px />

 - Name the parameter "Speed".
 - Repeat to create:
   - A bool named 'isTouchingFloor'.
   - A bool named 'isClimbing'.
   - A bool named 'hasWeapon'.
 - Create script Code/Components/Animations/**PlayerAnimator**:

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

  protected void Update()
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

Nothing yet.

The parameters we are creating will be used to cause the Animation controller to transtition from one state to another.  This approach is an alternative to playing the animation state directly like we had done for the Hammer above.  

The speed parameter will also be used to scale the animation playback speed based off how quickly the entity is moving at the time.

The PlayerAnimatorController is simply forwarding information from various components to the Animation controller.

<hr></details>
<details><summary>When do you use animation parameters vs Play(state)?</summary>

It's up to you.  Both approaches have the same capabilities, but by using animation parameters you can let the Animation controller own much of the logic - simplifying your code and debugging.

I prefer to use Play for simple objects like the Hammer, and use animation parameters for more complex ones like entities.

<hr></details>

## 5.6) Adjust the walk speed

Update the walk speed to leverage the speed parameter created.

<details><summary>How</summary>

 - In the Animator for the character, select the 'CharacterWalk' state (the orange box).
   - In the Inspector, under speed check the box near 'Multiplier' to enable a 'Parameter'.
   - Confirm Speed is selected (should be the default).
   - Adjust the 'Speed' to about '.4'

<img src="http://i.imgur.com/9A6mp98.png" width=300px />

<hr></details><br>
<details><summary>What did that do?</summary>

This slows the character's walk animation and gradually turns it on and off as the character starts and stops moving.

Now the character's walk animation should align with the moment a little better.  Adjust the value to something you think looks good. However the walk animation also plays while jumping, we'll address this next.

<img src="http://i.imgur.com/2dfN2RE.gif" width=150px />

<hr></details>
<details><summary>What unit/scale is speed defined in?</summary>

Percent.  1 represents the speed as it was defined in the animation itself.  Going to 2 would double the playback speed, going to .5 would cut the playback speed in half.

<hr></details>

## 5.7) Jump animation

Add an animation to the character for jumping. 

<details><summary>How</summary>

 - Select the character's sprite and in the Animation window, create a new clip Assets/Animations/**CharacterJump**.
 - Select the sprites for the jump animation. We are using **adverturer_spritesheet_7** and **8**.
 - Drag and drop the sprites onto the Animation timeline.

<img src="http://i.imgur.com/0rHCGDm.gif" width=300px />

 - In the Animator window:
   - Select the CharacterJump state and use the Speed paramater times about .05
   - Right click on the 'Any State' box and select 'Make Transition'.
     - An arrow will follow your mouse, click on the CharacterJump state to create the transition.

<img src="http://i.imgur.com/Fl0WTPO.gif" width=300px />

 - Select the transition arrow just created, in the Inspector click the plus to create a new condition.

<img src="http://i.imgur.com/WgOfzQY.png" width=150px />

 - Change the condition to read 'isTouchingFloor false'.
 - Under 'Settings':
   - Change the 'Transition Duration' to 0.
   - Uncheck 'Can Transition to Self'.
 - Create a transition from CharacterJump to CharacterWalk.
 - Select the transition:
   - Set the condition to 'isTouchingFloor true'.
   - Uncheck 'Has Exit Time'.
   - Change the 'Transition Duration' to 0.

<hr></details><br>
<details><summary>What did that do?</summary>

A jump animation for the character was added.  As you jump, the character should kick his feet a bit and then resume walking when he lands.

<hr></details>
<details><summary>Why transition from Any State instead of from CharacterWalk?</summary>

Any State is a special 'state' in the Animation Controller, allowing you to define transitions which could happen at any time.

You could create this transiton from the CharacterWalk state instead.  However I am using Any State because as we add more animations for the character, we won't need to define as many total transitions.

<hr></details>
<details><summary>How do animation conditions work?</summary>

For transitions with one or more conditions, we change states when all conditions are met.  This could be a single parameter such as the bool we are using here, or it could be a combination of factors.

<hr></details>
<details><summary>What does Has Exit Time do and how does it relate to conditions?</summary>

Has Exit Time is an additional way of triggering a transition.  So if a transition has both Has Exit Time and Conditions defined, the transition occurs when **either** the time has passed **or** the conditions are true.

<hr></details>
<details><summary>What does the Transition Duration impact?</summary>

Once the conditions are met, the transition from one state to the other completes in the 'Transition Duration' time.  This is a great feature for 3D models as the Unity animator will smooth the transition from one stance to another.  However for sprites, there is no smoothing so we typically want a transition duration of 0.

<hr></details>


## 5.8) Climb animation

Add an animation for when climbing ladders.

<details><summary>How</summary>

 - Create a new animation for the character Assets/Animations/**CharacterClimb**.
 - Drag in the sprites for the climb animation.  We are using **adverturer_spritesheet_5** and **6**.
 - Select the CharacterClimb state and use the Speed paramater times about .1
 - Create a transition from Any State to CharacterClimb.
   - Add a condition 'isClimbing true'.
 - Create a transition from CharacterClimb to CharacterWalk.
   - Uncheck Has Exit Time.
   - Set Transition Duration to 0.
   - Uncheck Can Transition to Self.
   - Add a condition 'isClimbing false'.
 - Select the transition from Any State to CharacterJump
   - Add a condition 'isClimbing false'.

<hr></details><br>
<details><summary>What did that do?</summary>

A climb animation for the character was added. As you climb up or down a ladder, the character should move his arms and then resume walking when he gets off.

<hr></details>

## 5.9) Idle animation

Create an animation for the character to set the sprite to an idle stance.  As there character stands there, animate the scale to make the character look like he is breathing.

<details><summary>How</summary>

 - Create a new animation for the character Assets/Animations/**CharacterIdle**.
 - Click record
   - Change the 'Sprite' under the character's Sprite Renderer component to an idle stance. We are using **adventurer_tilesheet_0**.
   - Double click to create a keyframe at 1:00.
   - Switch the current time position to 0:30.
     - This will move the white line, indicating where in the timeline modifications will be made. 
   - Set the Transform scale to (1, .95, 1).
   - Switch the time to 1:00 and set the Transform scale to (1, 1, 1).
   - Then stop recording.   
 - In the Animator, create a transition from CharacterWalk to CharacterIdle.
   - Uncheck Has Exit Time.
   - Set transition duration to 0.
   - Add a condition when 'Speed' is 'Less' than '.1'
 - Make a transition from CharacterIdle to CharacterWalk.
   - Uncheck Has Exit Time.
   - Set transition duration to 0.
   - Add a condition for 'Speed' is 'Greater' than '.1'

The character's animator controller should look something like this now:

<img src="http://i.imgur.com/VotmF1k.png" width=200px />

<hr></details><br>
<details><summary>What did that do?</summary>

When the character is not moving, he will switch to the idle stance.  We also modify the scale, from 1 to .95 on the Y and then back to 1 to create the breathing effect.  

Hit play so see the character switch between walking and standing:

<img src="http://i.imgur.com/YjZ1zrE.gif" width=200px />

<hr></details>


## 5.10) Add a breakdance animation

Add an animation for the character dancing after standing still for a bit.  

<details><summary>How</summary>

 - Create a new animation for the character Assets/Animations/**CharacterDance**.anim
 - Select all the sprites for this animation and drag them into the timeline. We are using **adventurer_tilesheet_11** **- 21** (10 sprites).
 - Change the CharacterDance speed to '.1'
 - Create a transition from CharacterIdle to CharacterDance.
   - Change the 'Exit Time' to about '3'
   - Set Transition Duration to 0.
 - Create a transition from CharacterDance to CharacterIdle.
   - Set Transition Duration to 0.
 - Create a transition from CharacterDance to CharacterWalk.
   - Uncheck 'Has Exit Time'.
   - Set Transition Duration to 0.
   - Add a Condition for 'Speed' is 'Greater' than '.1'

<hr></details><br>
<details><summary>What did that do?</summary>

The character will dance after standing still for a few seconds.

We use 'Has Exit Time' to define how long the character should be in the CharacterIdle state before dancing.  If you start to walk during the dance, he will switch to the walk animation right away.

<img src="http://i.imgur.com/t7cUVPI.gif" width=250px />

<hr></details>


## 5.11) Add an intro animation for the cloud

Create an animation for the cloud entrance at the start of the level.

<details><summary>How</summary>

 - Create an animation for the evil cloud sprite Assets/Animations/**CloudLevel1Entrance**.anim
 - Click record:
   - Start by moving the cloud off screen.
   - Then over time, modify its position to create a dramatic entrance.
 - Select Assets/Animations/CloudLevel1Entrance and in the Inspector uncheck 'Loop Time'.

<hr></details><br>
<details><summary>What did that do?</summary>

We created an entrance for the cloud to play at the start of the level.  This is just for design, so do whatever you like here.  We'll get the spawners to hold until the animation completes in a bit.  

Our animation looks like this at the moment:

<img src="http://i.imgur.com/o40dfEx.gif" width=300px>

<hr></details>


## 5.12) Add an intro timeline

Create a timeline which enables the LevelManager and Hammers after the intro is complete.

<details><summary>How</summary>

 - Select the evil cloud sprite.
 - Open menu Window -> Timeline.
   - Click 'Create'.  Save as Assets/Animations/**Level1Entrance**.
   - Select 'Add from Animation Clip' and select CloudLevel1Entrance.

<img src="http://i.imgur.com/7HXZs7Z.gif" width=300px />

 - Drag the parent Hammers GameObject (which holds all the hammers) onto the timeline and select 'Activation Track'.
   - Move the box for the script so that it starts after the cloud animation completes.  The size of the box itself does not matter, the start represents when it will be enabled and the end must align with the end of the time timeline to prevent it from being disabled.

<img src="http://i.imgur.com/6XyJZlh.gif" width=300px />

 - Repeat, creating activation tracks for the LevelManager and the Ladders.

<hr></details><br>
<details><summary>What did that do?</summary>

A Timeline on the evil cloud is used to coordinate the intro animation across objects.  

 - The Hammers and Ladders are hidden until we start their FadeInThenEnable script with an Activation Track.
 - The character is spawned via an Activation Track for the LevelManager.

<hr></details>
<details><summary>What is a Unity Timeline / Activation Track?</summary>

Timeline is a new feature released with Unity 2017.  It's a higher level component than the Animator Controller, used to coordinate animations and trigger events across several objects in the scene with an interface that resembles the Animation timeline.

Previously, achieving similiar results would have required a script.  Now you can manage the sequence visually if you prefer.

'Add Animation From Clip' plays an animation during the timeframe specified, overriding what the Animator controller for that object would have done.

Activation Tracks are one of several ways that you trigger behaviour with the Timeline.  An activation track will enable a GameObject where the track begins in the timeline, and disable it again where it ends.  If the activation track ends at the very end of the entire timeline then it will remain active after the timeline completes.

<hr></details>


## 5.13) Disable spawners till the intro is complete

Create a script to enable components when the level intro completes.

<details><summary>How</summary>

 - Create script Code/Components/Life/**EnableComponentsOnLevelLoad**:

```csharp
using UnityEngine;

public class EnableComponentsOnLevelLoad : MonoBehaviour
{
  [SerializeField]
  MonoBehaviour[] componentToEnableOnAlmostLoaded;
  
  [SerializeField]
  MonoBehaviour[] componentToEnableOnComplete;

  public void OnLevelAlmostLoaded()
  {
    for(int i = 0; i < componentToEnableOnAlmostLoaded.Length; i++)
    {
      MonoBehaviour component = componentToEnableOnAlmostLoaded[i];
      component.enabled = true;
    }
  }

  public void OnLevelLoaded()
  {
    for(int i = 0; i < componentToEnableOnComplete.Length; i++)
    {
      MonoBehaviour component = componentToEnableOnComplete[i];
      component.enabled = true;
    }
  }
}
```

 - For both the cloud and door:
   - Disable the Spawner component.
   - Add EnableComponentsOnLevelLoad.
   - Set the Components To Enable On Complete to the Spawner component.
 - Create script Code/Components/Animations/**EndOfLevelPlayable**:

```csharp
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EndOfLevelPlayable : BasicPlayableBehaviour
{
  public enum EndOfLevelEventType
  {
    AlmostComplete, Complete
  }

  [SerializeField]
  EndOfLevelEventType eventType;

  public override void OnBehaviourPlay(
    Playable playable,
    FrameData info)
  {
    base.OnBehaviourPlay(playable, info);

    EnableComponentsOnLevelLoad[] endOfLevelList
      = GameObject.FindObjectsOfType<EnableComponentsOnLevelLoad>();

    for(int i = 0; i < endOfLevelList.Length; i++)
    {
      EnableComponentsOnLevelLoad endOfLevel = endOfLevelList[i];
      switch(eventType)
      {
        case EndOfLevelEventType.AlmostComplete:
          endOfLevel.OnLevelAlmostLoaded();
          break;
        case EndOfLevelEventType.Complete:
          endOfLevel.OnLevelLoaded();
          break;
      }
    }
  }
}
```

 - Drag drop the script into the timeline.  Set the time like we did for the Hammers.
   - In the Inspector, change the 'Event Type' to 'Complete'.

<img src="http://i.imgur.com/ynW3z5a.png" width=300px />

 - Drag the script in a second time and set the time to fire a bit before the animation ends.

<img src="http://i.imgur.com/AYkG3Jc.png" width=300px />

<hr></details><br>
<details><summary>What did that do?</summary>

EnableComponentsOnLevelLoad is used to enable specific components during the intro sequence (as opposed to the entire GameObject).  There are two event types supported:

 - Almost Loaded: fired a few moments before the end of the intro.
 - On Complete: fired once the intro sequence is complete.
 
EndofLevelPlayable is the component which appears in the Timeline to call each of the EnableComponentsOnLevelLoad in the scene.  

We add this to evil cloud and the door so that their sprites are visible but the spawners are not enabled until the intro animations completes.

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

<hr></details>


## 5.14) Rotate platforms during intro

Platforms start out straight and then when the intro animation is nearly complete, shake down into position.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**RotateOvertimeToOriginal**:

```csharp
using System.Collections;
using UnityEngine;

public class RotateOvertimeToOriginal : MonoBehaviour
{
  [SerializeField]
  float rotationTimeFactor = 1;

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
      sleepTimeLastFrame = UnityEngine.Random.Range(0, maxTimeBetweenRotations);
      yield return new WaitForSeconds(sleepTimeLastFrame);

      float percentCompleteThisFrame = sleepTimeLastFrame * rotationTimeFactor;
      percentCompleteThisFrame *= UnityEngine.Random.Range(0, 10);
      percentComplete += percentCompleteThisFrame;
      if(percentComplete >= 1)
      {
        transform.rotation = targetRotation;
        yield break;
      }
      transform.rotation = Quaternion.Lerp(Quaternion.identity, targetRotation, percentComplete);
    }
  }
}
```

 - For each Platform:
   - Add RotateOvertimeToOriginal and disable the component.
   - Add EnableComponentsOnLevelLoad and add RotateOvertimeToOriginal to the 'Components to enable on almost loaded'.

<hr></details><br>
<details><summary>What did that do?</summary>

When the level begins, RotateOvertimeToOriginal stores the object's original rotation (as it was placed in the scene).  We then change the rotation to identity, or the default rotation for the sprite before the first render on screen.

A coroutine periodically lerps rotation back to the original.  We use RNG, both for a random sleep time between rotation changes and to randomize how much the rotation changes by.  Our goal here is to make it not smooth, as if it were falling / shaking into place.

<hr></details>

## 5.15) Add screen shake

Shake the screen when the platforms fall into place.

<details><summary>How</summary>

 - Create script Code/Components/Animations/**ScreenShake**.

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
      camera.transform.position = startingPosition + (Vector3)UnityEngine.Random.insideUnitCircle * shakeMagnitude * percentComplete;

      float sleepTime = UnityEngine.Random.Range(0, maxTimeBetweenShakes * (1 - percentComplete));
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
     - Add ScreenShake to the list to enable on 'almost complete'.

<hr></details><br>
<details><summary>What did that do?</summary>

ScreenShake moves the camera up/down/left/right to create a shaking effect.  This component is enabled when the intro sequence is 'almost complete', and that event aligns with the cloud bouncing - making it look like the cloud is shaking the platforms into place.

<hr></details>

## 5.16) Test

TODO

# Next chapter

[Chapter 6](https://github.com/hardlydifficult/Platformer/blob/master/Chapter6.md).
