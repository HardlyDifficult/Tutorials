using UnityEngine;

/// <summary>
/// Ensures that the entity stays on the screen.  It also checks for an ememy which is walking into an obstacle.
/// It will flip the current walk direction automatically (which has no impact on the Player but causes enemies to bounce).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KeepMovementOnScreen : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Used to cause the entity to start walking the opposite direction when it hits the edge of the screen.
  /// This is not required and may be null;
  /// </summary>
  WalkMovement walkMovement;

  /// <summary>
  /// Used to determine if we are currently moving.
  /// </summary>
  Rigidbody2D myBody;
  #endregion

  #region Init
  /// <summary>
  /// On awake, initialize variables.
  /// </summary>
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    myBody = GetComponent<Rigidbody2D>();

    Debug.Assert(myBody != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// Every update check if the entity is still on screen or if the entity is stuck.  
  /// If off screen, pop it back and flip the walk direction.
  /// If stuck, flip the walk direction.
  /// </summary>
  protected void Update()
  {
    if(GameController.instance.screenBounds.Contains(transform.position) == false)
    { // Enstity is out of bounds
      transform.position = GameController.instance.screenBounds.ClosestPoint(transform.position);
      if(walkMovement != null)
      {
        walkMovement.inputWalkDirection = -walkMovement.inputWalkDirection;
      }
    }
    else if(myBody.velocity.sqrMagnitude < .001)
    { // Entity is stuck
      if(walkMovement != null)
      {
        walkMovement.inputWalkDirection = -walkMovement.inputWalkDirection;
      }
    }
  }
  #endregion
}
