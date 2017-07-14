using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))]
public class RandomlyClimbLadder : MonoBehaviour
{
  #region Data
  [SerializeField]
  float oddsOfClimbingLadderUp = .5f;
  [SerializeField]
  float oddsOfClimbingLadderDown = .1f;

  Rigidbody2D myBody;
  Feet feet;
  LadderMovement ladderMovement;
  WalkMovement walkMovement;

  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    walkMovement = GetComponent<WalkMovement>();
    feet = GetComponentInChildren<Feet>();
    Debug.Assert(feet != null);
    ladderMovement = GetComponent<LadderMovement>();

  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    FixedUpdate_GetOffLadder();
    FixedUpdate_GetOnLadder();
  }
  #endregion

  #region Private helpers
  void FixedUpdate_GetOnLadder()
  {
    Ladder ladder = ladderMovement.currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(ladderMovement.isOnLadder == false)
    { // If not climbing, roll the dice to see if we should start
      if(Mathf.Abs(ladder.bounds.center.x - transform.position.x) < .1f)
      {
        if(transform.position.y < ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderUp)
        {
          ladderMovement.climbDirection = 1;
          walkMovement.inputWalkDirection = 0;
        }
        else if(transform.position.y > ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderDown)
        {
          ladderMovement.climbDirection = -1;
          walkMovement.inputWalkDirection = 0;
        }
      }
    }
  }

  void FixedUpdate_GetOffLadder()
  {
    Ladder ladder = ladderMovement.currentLadder;
    if(ladder == null
      || ladderMovement.isOnLadder == false)
    {
      return;
    }

    if(myBody.position.y < ladder.bounds.min.y)
    { // Get off
      ladderMovement.isOnLadder = false;
    }
  }
  #endregion
}
