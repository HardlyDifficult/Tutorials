using UnityEngine;

/// <summary>
/// Wires up user input, allowing the user to control the player in game with a keyboard.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The main player component.
  /// </summary>
  Player player;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate variables and disable this component until the entity fades in.
  /// </summary>
  protected void Awake()
  {
    player = GetComponent<Player>();

    AppearInSecondsAndFadeInSprite.DisableMeTillComplete(this);

    Debug.Assert(player != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// On fixed update, consider moving left/right up/down.
  /// </summary>
  /// <remarks>
  /// Moving uses an input state, and therefore may be captured on Update or FixedUpdate, we use
  /// FixedUpdate since physics also runs on FixedUpdate, 
  /// so trying to do this on update would require an extra cache (w/o benefit).
  /// </remarks>
  protected void FixedUpdate()
  {
    FixedUpdate_MoveLeftRight();
    FixedUpdate_ClimbLadder();
  }

  /// <summary>
  /// On update, consider jumping.
  /// </summary>
  /// <remarks>
  /// Jumping uses an input event, and therefore must be captured on Update (vs fixed).
  /// </remarks>
  protected void Update()
  {
    Update_Jump();
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Consider moving left/right.
  /// </summary>
  void FixedUpdate_MoveLeftRight()
  {
    player.walkMovement.inputWalkDirection = Input.GetAxis("Horizontal");
  }

  /// <summary>
  /// Consider climbing ladder, if we don't have a weapon ATM.
  /// </summary>
  void FixedUpdate_ClimbLadder()
  {
    if(player.currentWeapon != null)
    { // You can't climb while swinging a hammer
      player.ladderMovement.climbDirection = 0;
    }
    else
    {
      float vertical = Input.GetAxis("Vertical");
      player.ladderMovement.climbDirection = vertical;
    }
  }

  /// <summary>
  /// Consider jumping, and cache the jump request to be processed on FixedUpdate.
  /// </summary>
  void Update_Jump()
  {
    if(Input.GetButtonDown("Jump"))
    {
      player.jumpMovement.Jump();
    }
  }
  #endregion
}
