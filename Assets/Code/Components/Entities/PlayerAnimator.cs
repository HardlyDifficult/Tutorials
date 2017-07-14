using UnityEngine;

/// <summary>
/// Updates the Animator each frame with variables of interest, such as speed.
/// These directly impact the animations playing, either triggering state transitions or changing playback speed.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerAnimator : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Used to set variables leveraged by animations such as the current speed.
  /// </summary>
  Animator animator;

  /// <summary>
  /// Reference to the main player entity for easy access to the data the animation is looking for.
  /// </summary>
  Player player;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate variables.
  /// </summary>
  void Awake()
  {
    player = GetComponent<Player>();
    animator = GetComponentInChildren<Animator>();
  }

  /// <summary>
  /// On disable, tell the animator to freeze movement.  Used to freeze the player at the end of the level.
  /// </summary>
  void OnDisable()
  { 
    Update_Animation(shouldFreeze: true);
  }
  #endregion

  #region Events
  /// <summary>
  /// On update, update the animator with variables such as current speed.
  /// </summary>
  void Update()
  {
    Update_Animation();
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Update the animator with variables such as speed.
  /// </summary>
  /// <param name="shouldFreeze">True if all movement should stop.</param>
  void Update_Animation(
    bool shouldFreeze = false)
  {
    if(animator.isActiveAndEnabled == false)
    {
      return;
    }

    animator.SetFloat("Speed", shouldFreeze ? 0 : player.myBody.velocity.magnitude);
    animator.SetBool("isGrounded", shouldFreeze ? false : player.feet.isGrounded);
    animator.SetBool("isClimbing", shouldFreeze ? false : player.ladderMovement.isOnLadder);
    animator.SetBool("hasWeapon", shouldFreeze ? false : player.currentWeapon != null);
  }
  #endregion
}
