using UnityEngine;

/// <summary>
/// The main bomb enemy entity component.  This tracks total bombCount in the world and assists with spawn configuration.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))]
public class Bomb : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The total number of bombs still alive in the game.
  /// </summary>
  public static int bombCount;

  /// <summary>
  /// When spawned, add this angular speed to the object.  This causes the bomb to spin.
  /// </summary>
  [SerializeField]
  float startingAngularSpeed = -500;

  /// <summary>
  /// When spawned, apply this to the velocity.x
  /// </summary>
  [SerializeField]
  float startingSpeed = 3;

  /// <summary>
  /// Cache of the rigidBody, for performance.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Cache of the angular velocity when getting on a ladder, applied to the object again when getting off.
  /// </summary>
  float previousAngularVelocity;

  /// <summary>
  /// Cache of the x velocity when getting on a ladder, applied to the object again when getting off.
  /// </summary>
  float previousXVelocity;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate vars.
  /// </summary>
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    LadderMovement ladderMovement = GetComponent<LadderMovement>();
    ladderMovement.onGettingOffLadder += ClimbLadder_onGettingOffLadder;
    ladderMovement.onGettingOnLadder += LadderMovement_onGettingOnLadder;
  }

  /// <summary>
  /// On start, increment the total active bomb count and start movement.
  /// </summary>
  void Start()
  {
    bombCount++;

    myBody.angularVelocity = startingAngularSpeed;
    myBody.velocity = new Vector2(startingSpeed, 0);
  }

  /// <summary>
  /// On destroy, decrement the total active bomb count.
  /// </summary>
  void OnDestroy()
  {
    bombCount--;
  }
  #endregion

  #region Events
  /// <summary>
  /// When getting on a ladder, store momentum.
  /// </summary>
  void LadderMovement_onGettingOnLadder()
  {
    previousAngularVelocity = myBody.angularVelocity;
    previousXVelocity = myBody.velocity.x;
  }

  /// <summary>
  /// When getting off a ladder, resume moving in the opposite direction you were when getting on the ladder.
  /// </summary>
  void ClimbLadder_onGettingOffLadder()
  { 
    myBody.angularVelocity = -previousAngularVelocity;
    myBody.velocity = new Vector2(-previousXVelocity, myBody.velocity.y);
  }
  #endregion
}
