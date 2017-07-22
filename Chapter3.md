# 3) Animations


## Animation walk speed

Update the walk animation speed to align with the character's movement.

<details><summary>How</summary>

 - Select the character's GameObject.
 - Double click the box next to 'Controller' to open the 'Animator' tab for the character's animation controller.

<img src="http://i.imgur.com/F7AkEaH.gif" width=200px />

 - Switch to the 'Parameters' tab on the left.
 - Click the '+' button and select 'Float'.

<img src="http://i.imgur.com/p6F4gHG.png" width=100px />

 - Name the parameter "Speed".
 - Select the 'CharacterWalk' state.
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



 

# Next chapter

[Chapter 4](https://github.com/hardlydifficult/Platformer/blob/master/Chapter4.md).