# 5) Animations 

TODO intro


## Hammer animation 

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

## Stop swinging by default

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


## Start swinging hammer on equipt

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

## Character animation parameters

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

## Adjust the walk speed

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

## Jump animation

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
 - Create a transition from CharacterJump to CharacterWalk.
 - Select the transition and set the condition to 'isTouchingFloor true'.
 - Uncheck 'Has Exit Time'.
 - Under 'Settings' change the 'Transition Duration' to 0.

</details>


## Climb animation

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
   - Add a condition 'isClimbing false'.
 - Select the transition from Any State to CharacterJump
   - Add a condition 'isClimbing false'.


<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## Add an idle animation

Create an animation for the character to set the sprite to an idle stance.  

As there character stands there, animate the scale to make the character look like he is breathing.

<details><summary>How</summary>

 - Open menu Window -> Animation.
 - Drag the character prefab into the scene and select the child sprite GameObject.
 - Click the dropdown (it should say 'CharacterWalk') and 'Create New Clip'.

<img src="http://i.imgur.com/uJ0VeOp.png" width=300px />

 - Save the clip as Assets/Animations/CharacterIdle.
 - Click the red record button.
 - Change the 'Sprite' under the character's Sprite Renderer component to an idle stance.  We are using adventurer_tilesheet_0. 
 - Click the record button again to stop recording.


 - Open menu Window -> Animator.
 - Right click on the CharacterWalk state and select 'Make Transition'.
 - An arrow will follow your mouse, click on the CharacterIdle state to create the transition.

<img src="http://i.imgur.com/4X3rXti.gif" width=300px />

 - Click on the transition arrow you just created, then in the Inspector
   - Uncheck 'Has Exit Time'.
   - Under 'Conditions' click the '+' button.
   - Update the condition to be when 'Speed' is 'Less' than '.1'

<img src="http://i.imgur.com/kPE2Iup.png" width=150px />

Hit play, the character should 'walk' as he falls... but once he comes to a complete stop he never starts the walk animation again.

<img src="http://i.imgur.com/O7XQUeP.gif" width=150px />

 - Right click on the CharacterIdle state and Make a Transition to CharacterWalk.
 - Click on the transition just created:
   - Uncheck Has Exit Time.
   - Add a condition for 'Speed' is 'Greater' than '.1'.

The character's animator controller should look something like this now:

<img src="http://i.imgur.com/VotmF1k.png" width=200px />

Hit play so see the character switch between walking and standing:

<img src="http://i.imgur.com/YjZ1zrE.gif" width=200px />

You can adjust the 'Transition Duration' if you want the character to switch sprites faster or slower.

 - Select one of the transition arrows.
 - Under 'Settings', change the 'Transition Duration' value.
 - Do the same for the other transition arrow.

Hit play and note the difference, to help demonstrate what is happening we are using a transition duration of 1 here for both transitions:

<img src="http://i.imgur.com/QV38yfS.gif" width=200px />


 - In the Animation tab, select the CharacterIdle animation and hit record.
 - Select the character's GameObject, in the Inspector change scale to 0 and then back to one.

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



<hr></details><br>
<details><summary>TODO</summary>

The Animation should now look like this, note the preview of the character's idle sprite and there is no timeline, it is just a single keyframe.

<img src="http://i.imgur.com/j2S25Ex.png" width=300px />

The Animator tab should now have a new state for CharacterIdle (a grey box).

<hr></details>





## Add a breakdance animation

Create an animation for the character dancing.  We are using adventurer_tilesheet 11 - 21 (10 sprites).

<details><summary>How</summary>

 - Select the character and in the Animation tab create a new clip, save it as Assets/Animations/CharacterDance.
 - Select all the sprites for this animation. We are using adventurer_tilesheet 11 - 21 (10 sprites).
 - Drag and drop them into the animation timeline.



 - Select the character and in the Animator tab, create a transition from CharacterIdle to CharacterDance.
 - Select the transition you just created, in the Inspector under 'Settings' change the 'Exit Time' to about '3'.

Click play and wait a few seconds for the dance to begin.

 - Select the CharacterDance state and adjust the speed to about .1
 - Create a transition from CharacterDance to CharacterIdle (using the default settings).

Play to preview the dance:

<img src="http://i.imgur.com/pE6tUfe.gif" width=150px />

However if you start to walk during the dance, it doesn't look quite right:

<img src="http://i.imgur.com/d9wCdad.gif" width=250px />

 - Create a transition from CharacterDance to CharacterWalk.
 - Select the transition you just created and:
   - Uncheck 'Has Exit Time'.
   - Change the 'Transition Duration' to '0'
   - Add a Condition for 'Speed' is 'Greater' than '.1'

Now we resume walking as desired:

<img src="http://i.imgur.com/t7cUVPI.gif" width=250px />



<hr></details><br>
<details><summary>TODO</summary>

Click play in the animation tab to see a preview of the dance, but it may be a little fast:

<img src="http://i.imgur.com/thjyiMM.gif" width=200px />

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





## Intro

Try Unity Timeline.

Character and fly guy fades in via AppearInSecondsAndFade

+ other intro effects
 - Cloud and animation


# Next chapter

[Chapter 6](https://github.com/hardlydifficult/Platformer/blob/master/Chapter6.md).



TODO something for bomb? but that doesn't make sense till the hammer. 
other death effects?