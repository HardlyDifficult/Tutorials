# 3) Animations and stuff

TODO intro

## Use velocity as a speed multipler

Update the walk animation speed so that the animation plays faster when the character moves quickly.

Note that the character will walk when jumping too, we'll address this later in the tutorial.

<details><summary>How</summary>

 - Select the character's GameObject.
 - Double click the box next to 'Controller' to open the 'Animator' tab for the character's animation controller.

<img src="http://i.imgur.com/F7AkEaH.gif" width=200px />

 - Switch to the 'Parameters' tab on the left.
 - Click the '+' button and select 'Float'.

<img src="http://i.imgur.com/p6F4gHG.png" width=100px />

 - Name the parameter "Speed".
 - Select the 'CharacterWalk' state (the orange box).
 - In the Inspector, under speed check the box near 'Multiplier' to enable a 'Parameter'.
 - Confirm Speed is selected (should be the default).

Hit play and you'll see that the walk animation has stopped completely.

 - Create a C# script "PlayerAnimator" under Assets/Code/Components/Animations.
 - Select the Character GameObject and add the PlayerAnimator component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Updates the Animator each frame with variables of interest, such as speed.
/// 
/// These directly impact the animations playing, either triggering state transitions or changing playback speed.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
  /// <summary>
  /// Used to set variables leveraged by animations such as the current speed.
  /// </summary>
  Animator animator;

  /// <summary>
  /// Used to get the current velocity.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    animator = GetComponentInChildren<Animator>();
    myBody = GetComponent<Rigidbody2D>();
    Debug.Assert(animator != null);
    Debug.Assert(myBody != null);
  }

  /// <summary>
  /// A Unity event, called each frame.
  /// 
  /// Update the animator with variables such as current speed.
  /// </summary>
  protected void Update()
  {
    animator.SetFloat("Speed", myBody.velocity.magnitude);
  }
}
```

Hit play to see the character playing the walk animation only while moving.

<img src="http://i.imgur.com/KZYjZf2.gif" width=150px />

</details>

## Adjust the walk speed

Change the walk animator state's speed to about .4.


<details><summary>How</summary>

 - Select the 'CharacterWalk' state in the Animator tab.
 - Adjust the 'Speed' to about '.4'

<img src="http://i.imgur.com/dhFASHe.png" width=150px />

Now the character's walk animation should align with the moment a little better.  Adjust the value to something you think looks good. However the walk animation also plays while jumping:

<img src="http://i.imgur.com/2dfN2RE.gif" width=150px />

</details>

## Create an idle animation

Create an animation for the character to set the sprite to an idle stance.  We are using adventurer_tilesheet_0.

<details><summary>How</summary>

 - Open the 'Animation' tab by selecting menu Window -> Animation.
 - Select the character GameObject.
 - Click the dropdown (it should say 'CharacterWalk') and 'Create New Clip'.

<img src="http://i.imgur.com/uJ0VeOp.png" width=150px />

 - Save the clip as Assets/Animations/CharacterIdle.
 - Click the red record button.

<img src="http://i.imgur.com/XxrsDpf.png" width=200px />

 - Change the 'Sprite' under the character's Sprite Renderer component to an idle stance.  We are using adventurer_tilesheet_0.

<img src="http://i.imgur.com/yvWU5vP.gif" width=150px />
 
 - Click the record button again to stop recording.

The Animation should now look like this, note the preview of the character's idle sprite and there is no timeline, it is just a single keyframe.

<img src="http://i.imgur.com/j2S25Ex.png" width=200px />

The Animator tab should now have a new state for CharacterIdle (a grey box).

</details>

## Transition to and from idle

Have the character switch to an idle stance when he is not moving. 

<details><summary>How</summary>

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

</details>

## Add scale effect to the idle animation

As there character stands there, animate the scale to make the character look like he is breathing.

<details><summary>How</summary>

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

</details>


## Create a breakdance animation

Create an animation for the character dancing.  We are using adventurer_tilesheet 11 - 21 (10 sprites).

<details><summary>How</summary>

 - Select the character and in the Animation tab create a new clip, save it as Assets/Animations/CharacterDance.
 - Select all the sprites for this animation. We are using adventurer_tilesheet 11 - 21 (10 sprites).
 - Drag and drop them into the animation timeline.

<img src="http://i.imgur.com/JSsHfeU.gif" width=400px />

Click play in the animation tab to see a preview of the dance, but it may be a little fast:

<img src="http://i.imgur.com/thjyiMM.gif" width=200px />

</details>

## Transition to and from dancing

Have the character start dancing after idling for a few seconds.

<details><summary>How</summary>

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

</details>



## Wire up an animation parameter for isGrounded


## Add a Jump animation

Add an animation for jumping to play when isGrounded.  We are using adverturer_spritesheet_7 and 8.

<details><summary>How</summary>

 - Select the character and in the Animation tab, create a new clip Assets/Animations/CharacterJump.
 - Select the sprites for the jump animation We are using adverturer_spritesheet_7 and 8.
 - Drag and drop the sprites onto the Animation timeline.

TODO transitions.

</details>



## Climb animation





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



## Hammer

## Hammer animation

## Hammer killing



## Intro

Try Unity Timeline.

Character and fly guy fades in via AppearInSecondsAndFade

+ other intro effects
 - Cloud and animation


# Next chapter

[Chapter 4](https://github.com/hardlydifficult/Platformer/blob/master/Chapter4.md).