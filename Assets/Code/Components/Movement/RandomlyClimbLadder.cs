using UnityEngine;

/// <summary>
/// Causes an entity to roll the dice to consider getting on a ladder everytime it walks near one.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))]
public class RandomlyClimbLadder : MonoBehaviour
{
  #region Data
  /// <summary>
  /// When near a ladder going up, this is the odds that we start climbing.
  /// </summary>
  /// <remarks>You can set this to zero to disable going up ladders (used for the bombs).</remarks>
  [SerializeField]
  float oddsOfClimbingLadderUp = .5f;

  /// <summary>
  /// When near a ladder going down, this is the odds that we start climbing.
  /// </summary>
  [SerializeField]
  float oddsOfClimbingLadderDown = .1f;

  /// <summary>
  /// Used to stop walking when we get on a ladder.  This is not required and may be null.
  /// </summary>
  WalkMovement walkMovement;
  
  /// <summary>
  /// Used to get on/off ladders.
  /// </summary>
  LadderMovement ladderMovement;
  #endregion

  #region Init
  /// <summary>
  /// On awake, initialize varaibles.
  /// </summary>
  protected void Awake()
  {
    Debug.Assert(oddsOfClimbingLadderUp >= 0);
    Debug.Assert(oddsOfClimbingLadderDown >= 0);

    walkMovement = GetComponent<WalkMovement>(); 
    ladderMovement = GetComponent<LadderMovement>();

    Debug.Assert(ladderMovement != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// Consider getting on a ladder based on RNG and proximity.
  /// </summary>
  protected void FixedUpdate()
  {
    Ladder ladder = ladderMovement.currentLadder;
    if(ladder == null
      || Mathf.Abs(ladder.bounds.center.x - transform.position.x) > .1f
      || ladderMovement.isOnLadder)
    { // Nothing to do if not near a ladder's center or already on one.
      return;
    }

    if(transform.position.y < ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderUp)
    { // Climb up and stop walking
      ladderMovement.climbDirection = 1;
      if(walkMovement != null)
      {
        walkMovement.inputWalkDirection = 0;
      }
    }
    else if(transform.position.y > ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderDown)
    { // Climb down and stop walking
      ladderMovement.climbDirection = -1;
      if(walkMovement != null)
      {
        walkMovement.inputWalkDirection = 0;
      }
    }
  }
  #endregion
}
