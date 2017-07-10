using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClimbLadder))]
//[RequireComponent(typeof(Feet))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
  public static int bombCount;

  public float startingAngularSpeed;
  public float startingSpeed;
  public int climbLadderOddsBiggerIsLessLikely;
  Rigidbody2D myBody;
  ClimbLadder climbLadder;
  Feet feet;
  float previousAngularVelocity, previousXVelocity;

  void Start()
  {
    myBody = GetComponent<Rigidbody2D>();
    myBody.angularVelocity = startingAngularSpeed;
    myBody.velocity = new Vector2(startingSpeed, 0);
    climbLadder = GetComponent<ClimbLadder>();
    feet = GetComponent<Feet>();
    bombCount++;
  }

  void OnDestroy()
  {
    bombCount--;
  }

  void Update()
  {
    Update_ClimbLadder();
  }

  private void FixedUpdate()
  {
    // TODO this for all
    if(transform.position.y < -12)
    { // Fell out of bounds
      Destroy(gameObject);
      return;
    }

    FixedUpdate_GetOnLadder();
  }

  void FixedUpdate_GetOnLadder()
  {
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(climbLadder.isOnLadder == false) // TODO move to fixed update
    { // If not climbing, roll the dice to see if we should start
      if(feet.isGrounded
        && (myBody.position.y > ladder.bounds.center.y)
        && Mathf.Abs(ladder.transform.position.x - transform.position.x) < .1f
        && UnityEngine.Random.Range(0, climbLadderOddsBiggerIsLessLikely) == 0)
      {
        print("Get on at " + myBody.position.y);
        previousAngularVelocity = myBody.angularVelocity;
        previousXVelocity = myBody.velocity.x;
        climbLadder.isOnLadder = true;
        myBody.velocity = Vector2.zero; // Stop all current momentum
      }
    }
  }

  void Update_ClimbLadder()
  { // TODO separate player controls from movement (for bad guys)
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }

    if(climbLadder.isOnLadder)
    {
      if(ladder.isBroken && myBody.position.y < ladder.bounds.min.y
        || ladder.isBroken == false && myBody.position.y < ladder.bounds.center.y)
      {
        print("Get off at " + myBody.position.y);
        climbLadder.isOnLadder = false;
        myBody.angularVelocity = -previousAngularVelocity;
        myBody.velocity = new Vector2(-previousXVelocity, myBody.velocity.y);
      } else
      {
        climbLadder.Climb(-1);
      }
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    if(collision.gameObject.CompareTag("Bumper")
      && PlayerController.instance != null)
    {
      if(transform.position.y < PlayerController.instance.transform.position.y)
      {
        Destroy(gameObject);
      }
    }
  }
}
