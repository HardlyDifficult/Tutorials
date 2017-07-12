using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For reliable results, execution order should be before movement scripts
/// </summary>
public class FlyGuy : MonoBehaviour, ICareWhenPlayerDies
{
  #region Data
  public static int flyGuyCount;

  SpriteRenderer sprite;
  [SerializeField]
  float oddsOfClimbingLadderUp, oddsOfClimbingLadderDown;
  [SerializeField]
  float oddsOfGoingUpHill = .8f;
  float timeOfLastClimb;
  [SerializeField]
  float minLengthBetweenClimbs = 3;
  [SerializeField]
  float timeBeforeFirstWander = 5;

  Feet feet;
  LadderMovement ladderMovement;
  WalkMovement moveController;
  #endregion

  #region Init
  void Awake()
  {
    moveController = GetComponent<WalkMovement>();
    sprite = GetComponent<SpriteRenderer>();
    ladderMovement = GetComponent<LadderMovement>();
    feet = GetComponentInChildren<Feet>();
    ladderMovement.onGettingOffLadder += LadderMovement_onGettingOffLadder;

    AppearInSecondsAndFadeInSprite.DisableMeTillComplete(this);
  }

  void Start()
  {
    flyGuyCount++;

    StartCoroutine(Wander());
  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    FixedUpdate_GetOnLadder();
  }

  void LadderMovement_onGettingOffLadder()
  {
    timeOfLastClimb = Time.timeSinceLevelLoad;
    SelectARandomWalkDirection();
  }

  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    Destroy(gameObject);
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
      if(Mathf.Abs(ladder.bounds.center.x - transform.position.x) < .1f
        && Time.timeSinceLevelLoad - timeOfLastClimb > minLengthBetweenClimbs)
      {
        if(transform.position.y < ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderUp)
        {
          ladderMovement.climbDirection = 1;
          moveController.inputWalkDirection = 0;
        }
        else if(transform.position.y > ladder.bounds.center.y && UnityEngine.Random.value <= oddsOfClimbingLadderDown)
        {
          ladderMovement.climbDirection = -1;
          moveController.inputWalkDirection = 0;
        }
      }
    }
  }

  IEnumerator Wander()
  {
    moveController.inputWalkDirection = 1;
    yield return new WaitForSeconds(timeBeforeFirstWander);

    while(true)
    {
      if(ladderMovement.isOnLadder == false)
      {
        SelectARandomWalkDirection();
      }
      yield return new WaitForSeconds(UnityEngine.Random.Range(1, 10f));
    }
  }

  void SelectARandomWalkDirection()
  {
    if(GameController.instance.screenBounds.SqrDistance(transform.position) <= 1)
    { // If at edge of map choices are limited
      if(transform.position.x < 0)
      {
        moveController.inputWalkDirection = 1;
      }
      else
      {
        moveController.inputWalkDirection = -1;
      }
      return;
    }

    float dot = Vector2.Dot(feet.floorUp, Vector2.right);
    if(dot < 0)
    {
      moveController.inputWalkDirection = UnityEngine.Random.value >= oddsOfGoingUpHill ? 1 : -1;
    }
    else if(dot > 0)
    {
      moveController.inputWalkDirection = UnityEngine.Random.value >= oddsOfGoingUpHill ? -1 : 1;
    }
    else if(UnityEngine.Random.Range(0, 2) == 0)
    {
      moveController.inputWalkDirection = 1;
    }
    else
    {
      moveController.inputWalkDirection = -1;
    }
  }
  #endregion
}
