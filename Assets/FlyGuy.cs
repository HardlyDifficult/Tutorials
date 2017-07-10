using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyGuy : MonoBehaviour
{
  public static int flyGuyCount;
  MoveStandard moveController;
  SpriteRenderer sprite;
  ClimbLadder climbLadder;
  [SerializeField]
  float oddsOfClimbingLadderUp, oddsOfClimbingLadderDown;
  float timeOfLastClimb;
  Feet feet;

  void Start()
  {
    flyGuyCount++;
    moveController = GetComponent<MoveStandard>();
    sprite = GetComponent<SpriteRenderer>();
    climbLadder = GetComponent<ClimbLadder>();
    feet = GetComponentInChildren<Feet>();
    StartCoroutine(Wander());
  }

  void Update()
  {
    Update_KeepInBounds();
  }

  void FixedUpdate()
  {
    FixedUpdate_GetOnLadder();
  }


  void Update_KeepInBounds()
  {
    if(GameController.instance.screenBounds.Contains(transform.position) == false)
    { // Player is out of bounds
      transform.position = GameController.instance.screenBounds.ClosestPoint(transform.position);
      moveController.walkSpeed = -moveController.walkSpeed;
    }
  }
  void FixedUpdate_GetOnLadder()
  {
    if(climbLadder.isOnLadder == false
      && Mathf.Abs(moveController.climbDirection) >= 0.001f)
    {
      timeOfLastClimb = Time.timeSinceLevelLoad;
      moveController.climbDirection = 0;
      SelectARandomWalkDirection();
    }

    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(climbLadder.isOnLadder == false)
    { // If not climbing, roll the dice to see if we should start
      if(Mathf.Abs(ladder.bounds.center.x - transform.position.x) < .1f
        && Time.timeSinceLevelLoad - timeOfLastClimb > 3)
      {
        if(transform.position.y < ladder.bounds.center.y && UnityEngine.Random.value < oddsOfClimbingLadderUp)
        {
          moveController.climbDirection = 1;
          moveController.walkSpeed = 0;
        }
        else if(transform.position.y > ladder.bounds.center.y && UnityEngine.Random.value < oddsOfClimbingLadderDown)
        {
          moveController.climbDirection = -1;
          moveController.walkSpeed = 0;
        }
      }
    }
  }

  IEnumerator Wander()
  {
    moveController.walkSpeed = -1;
    yield return new WaitForSeconds(5);
    sprite.sortingOrder = 0;

    while(true)
    {
      if(climbLadder.isOnLadder == false)
      {
        SelectARandomWalkDirection();
      }
      yield return new WaitForSeconds(UnityEngine.Random.Range(1, 10f));
    }
  }

  void SelectARandomWalkDirection()
  {
    float dot = Vector2.Dot(feet.floorUp, Vector2.right);
    if(dot < 0)
    {
      moveController.walkSpeed = 1;
    }
    else if(dot > 0)
    {
      moveController.walkSpeed = -1;
    }
    else if(UnityEngine.Random.Range(0, 2) == 0)
    {
      moveController.walkSpeed = 1;
    }
    else
    {
      moveController.walkSpeed = -1;
    }
  }
}
