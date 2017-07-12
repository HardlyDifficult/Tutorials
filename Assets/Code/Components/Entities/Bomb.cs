using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LadderMovement))]
[RequireComponent(typeof(Feet))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour , ICareWhenPlayerDies
{
  #region Data
  public static int bombCount;

  [SerializeField]
  float startingAngularSpeed;
  [SerializeField]
  float startingSpeed;
  [SerializeField]
  float oddsOfClimbingLadder;

  Rigidbody2D myBody;
  LadderMovement climbLadder;
  Feet feet;

  float previousAngularVelocity, previousXVelocity; 
  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    climbLadder = GetComponent<LadderMovement>();
    feet = GetComponent<Feet>();
    climbLadder.onGettingOffLadder += ClimbLadder_onGettingOffLadder;
  }

  void Start()
  {
    bombCount++;
    myBody.angularVelocity = startingAngularSpeed;
    myBody.velocity = new Vector2(startingSpeed, 0);
  }

  void OnDestroy()
  {
    bombCount--;
  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    FixedUpdate_GetOnLadder();
    FixedUpdate_ClimbLadder();
  }

  void ClimbLadder_onGettingOffLadder()
  {
    // Resume momentum
    myBody.angularVelocity = -previousAngularVelocity;
    myBody.velocity = new Vector2(-previousXVelocity, myBody.velocity.y);
  }

  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    Destroy(gameObject);
  }
  #endregion

  #region Private helpers
  void FixedUpdate_GetOnLadder()
  {
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(climbLadder.isOnLadder == false)
    { // If not climbing, roll the dice to see if we should start
      if(feet.isGrounded // Not in air / bouncing
        && (myBody.position.y > ladder.bounds.center.y) // At top of ladder
        && Mathf.Abs(ladder.transform.position.x - transform.position.x) < .1f // Near center of ladder
        && UnityEngine.Random.value <= oddsOfClimbingLadder) // Rng
      {
        // Get on
        climbLadder.climbDirection = -1;

        // Store momentum 
        previousAngularVelocity = myBody.angularVelocity;
        previousXVelocity = myBody.velocity.x;

        // Stop movement
        myBody.velocity = Vector2.zero; 
      }
    }
  }

  void FixedUpdate_ClimbLadder()
  { 
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }

    if(climbLadder.isOnLadder)
    {
      if(myBody.position.y < ladder.bounds.min.y)
      {
        // Get off
        climbLadder.isOnLadder = false;
      }
    }
  }
  #endregion
}
