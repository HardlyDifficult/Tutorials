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
<details><summary>TODO</summary>

TODO
 - Click play to preview the hammer swinging, adjust the middle keyframe's position until the hammer has a nice swing, about 0:10.
Hit play and the hammer is swinging in the air.

<hr></details>

## 5.2) Stop swinging by default

Update the hammer animator to not play any animation by default.


<details><summary>How</summary>

 - Open menu Window -> Animator.
 - Select a hammer.
 - Right click -> Create State -> Empty.  
 - Select the box which appeared and in the Inspector name it "Idle".
 - Create new Empty State, name it "Idle".
 - Right click and 'Set as Layer Default State'.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 5.3) Start swinging hammer on equipt

Add a script to the hammer to start the swing animation when it's equipt.

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

 - Add it to a hammer prefab and set the animation to play to "HammerSwing".
 - Disable the PlayAnimationOnEnable component and add it under the hammer component's to enable list.

<hr></details><br>
<details><summary>TODO</summary>

TODO

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

 - Add it to the character.

<hr></details><br>
<details><summary>TODO</summary>

Hit play to see the character playing the walk animation only while moving.

<img src="http://i.imgur.com/KZYjZf2.gif" width=150px />


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
<details><summary>TODO</summary>

Now the character's walk animation should align with the moment a little better.  Adjust the value to something you think looks good. However the walk animation also plays while jumping:

<img src="http://i.imgur.com/2dfN2RE.gif" width=150px />

<hr></details>

## 5.7) Jump animation

Add an animation to the character for jumping. 

<details><summary>How</summary>

 - Select the character's sprite and in the Animation window, create a new clip Assets/Animations/**CharacterJump**.
 - Select the sprites for the jump animation. We are using **adverturer_spritesheet_7** and **8**.
 - Drag and drop the sprites onto the Animation timeline.

<img src="http://i.imgur.com/0rHCGDm.gif" width=300px />

 - In the Animator window, select the CharacterJump state and use the Speed paramater times about .05
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
 - Select the transition and set the condition to 'isTouchingFloor true'.
 - Uncheck 'Has Exit Time'.
 - Change the 'Transition Duration' to 0.

</details>


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
<details><summary>TODO</summary>

TODO

<hr></details>

## 5.9) Idle animation

Create an animation for the character to set the sprite to an idle stance.  As there character stands there, animate the scale to make the character look like he is breathing.

<details><summary>How</summary>

 - Create a new animation for the character Assets/Animations/**CharacterIdle**.
 - Click record
   - Change the 'Sprite' under the character's Sprite Renderer component to an idle stance. We are using **adventurer_tilesheet_0**.
   - Double click to create a keyframe at 1:00.
   - Switch the current time position to 0:30 and set the Transform scale to (1, .95, 1).
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




<hr></details><br>
<details><summary>TODO</summary>

The character's animator controller should look something like this now:

<img src="http://i.imgur.com/VotmF1k.png" width=200px />

Hit play so see the character switch between walking and standing:

<img src="http://i.imgur.com/YjZ1zrE.gif" width=200px />

You can adjust the 'Transition Duration' if you want the character to switch sprites faster or slower.
Hit play, the character should 'walk' as he falls... but once he comes to a complete stop he never starts the walk animation again.


Hit play and note the difference, to help demonstrate what is happening we are using a transition duration of 1 here for both transitions:

<img src="http://i.imgur.com/QV38yfS.gif" width=200px />
<img src="http://i.imgur.com/O7XQUeP.gif" width=150px />

The Animation should now look like this, note the preview of the character's idle sprite and there is no timeline, it is just a single keyframe.

<img src="http://i.imgur.com/j2S25Ex.png" width=300px />

The Animator tab should now have a new state for CharacterIdle (a grey box).


When we make a change to scale while in record mode, a keyframe is added.  So by changing the scale and then changing it back to the default of 1, we simply added a keyframe for scale 1 at the start of the animation. 

<img src="http://i.imgur.com/qVndjho.png" width=200px />

 - Click on 0:02 in the timeline.

This will move the white line, indicating where in the timeline modifications will be made:

<img src="http://i.imgur.com/1pwa5EU.gif" width=200px />

 - In the Inspector, change the scale to 0 and the back to one.

This updated the timeline, creating a second keyframe.

 - Click on 0:01 in the timeline.
 - Change the the scale to (1, .95, 1).
 - Hit record to stop recording.

Your animation should look like this:

<img src="http://i.imgur.com/ebuSIxb.png" width=200px />

Hit play to see the character breathing, but maybe a little fast:

<img src="http://i.imgur.com/81bajQP.gif" width=100px />

 - Change the CharacterIdle animator state's 'Speed' to about .01

The breath rate should be more reasonable now:

<img src="http://i.imgur.com/bfYKFkC.gif" width=100px />

<hr></details>


## 5.10) Add a breakdance animation

Add an animation for the character dancing after standing still for a bit.  

<details><summary>How</summary>

 - Create a new animation for the character Assets/Animations/**CharacterDance**.
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
<details><summary>TODO</summary>

Click play in the animation tab to see a preview of the dance, but it may be a little fast:

<img src="http://i.imgur.com/thjyiMM.gif" width=200px />


Play to preview the dance:

<img src="http://i.imgur.com/pE6tUfe.gif" width=150px />

However if you start to walk during the dance, it doesn't look quite right:

<img src="http://i.imgur.com/d9wCdad.gif" width=250px />


Now we resume walking as desired:

<img src="http://i.imgur.com/t7cUVPI.gif" width=250px />


<hr></details>





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


## 5.11) Add an intro animation for the cloud

Create an animation for the cloud entrance at the start of the level.

<details><summary>How</summary>

 - Create an animation for the evil cloud sprite Assets/Animations/**CloudLevel1Entrance**.
 - Click record:
   - Start by moving the cloud off screen.
   - Then over time, modify its position to create a dramatic entrance.
 - Select Assets/Animations/CloudLevel1Entrance and in the Inspector uncheck 'Loop Time'.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 5.12) Add an intro timeline

Create a timeline which enables LevelManager and hammers after the intro is complete.

<details><summary>How</summary>

 - Open menu Window -> Timeline.
 - Select the evil cloud sprite and click 'Create'.  Save as Assets/Animations/**Level1Entrance**.
 - Select 'Add from Animation Clip' and select CloudLevel1Entrance.

<img src="http://i.imgur.com/7HXZs7Z.gif" width=300px />

 - Drag the parent 'Hammers' GameObject (which holds all the hammers) onto the timeline and select 'Activation Track'.
 - Move the box for the script so that it starts after the cloud animation completes.  The size of the box itself does not matter, the start represents when it will be enabled and the end must align with the end of the time timeline to prevent it from being disabled.

<img src="http://i.imgur.com/6XyJZlh.gif" width=300px />

 - Do the same for the LevelManager and the ladders.

<hr></details><br>
<details><summary>TODO</summary>

TODO

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

 - Drag drop the script into the timeline.  Set the time like we did for the hammers.
 - In the Inspector, change the 'Event Type' to 'Complete'.

<img src="http://i.imgur.com/ynW3z5a.png" width=300px />

 - Drag the script in a second time and set the time to fire a bit before the animation ends.

<img src="http://i.imgur.com/AYkG3Jc.png" width=300px />

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## 5.14) Rotate platforms during intro

<details><summary>How</summary>

Platforms should start out straight and then when the intro animation is nearly complete, shake down into position.

<hr></details><br>
<details><summary>TODO</summary>

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
    // Start with no rotation
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

 - Add it to the camera and disable the component.
 - Add EnableComponentsOnLevelLoad, add screenshake to the list to enable on almost complete.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## 5.16) Test

TODO

# Next chapter

[Chapter 6](https://github.com/hardlydifficult/Platformer/blob/master/Chapter6.md).
