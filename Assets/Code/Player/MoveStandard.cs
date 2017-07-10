using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveStandard : MonoBehaviour
{
  [NonSerialized]
  public float walkSpeed;
  [NonSerialized]
  public float climbDirection;

  [SerializeField]
  float speed;

  Rigidbody2D myBody;
  bool isGoingRight;
  Feet feet;
  ClimbLadder climbLadder;

  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    climbLadder = GetComponent<ClimbLadder>();
    feet = GetComponentInChildren<Feet>();
    isGoingRight = Vector2.Dot(Vector2.right, transform.right) > 0;
  }

  private void Update()
  {
    Update_RotateCharacterToMatchPlatform();
  }

  void FixedUpdate()
  {
    FixedUpdate_ConsiderGettingOnLadderGivenInput();
    if(isGoingRight == false && walkSpeed > 0.001
      || isGoingRight && walkSpeed < -0.001)
    {
      transform.rotation *= Quaternion.Euler(0, 180, 0);
      isGoingRight = walkSpeed > 0;
    }

    myBody.velocity = new Vector2(walkSpeed * speed * Time.fixedDeltaTime, myBody.velocity.y);
  }


  void Update_RotateCharacterToMatchPlatform()
  {
    Quaternion targetRotation = feet.floorRotation; //floor.transform.rotation;

    if(transform.rotation.eulerAngles.y > 0.01)
    {
      targetRotation *= Quaternion.Euler(0, 180, 0);
    }
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .4f);
  }


  void FixedUpdate_ConsiderGettingOnLadderGivenInput()
  {
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(climbLadder.isOnLadder == false
          && Mathf.Abs(climbDirection) > 0.01
          && ((climbDirection > 0 && myBody.position.y < ladder.bounds.center.y)
            || (climbDirection < 0 && myBody.position.y > ladder.bounds.center.y)))
    { // Get on if moving up and on lower half or moving down and on upper half
      climbLadder.isOnLadder = true;
    }

    if(climbLadder.isOnLadder)
    {
      float currentVerticalVelocity = myBody.velocity.y;
      if(feet.distanceToGround > .1f && feet.distanceToGround < .3f
        && ((currentVerticalVelocity > 0 && myBody.position.y > ladder.bounds.center.y)
          || (currentVerticalVelocity < 0 && myBody.position.y < ladder.bounds.center.y)
        ))
      { // Get off if feet near ground and moving towards end of ladder
        climbLadder.isOnLadder = false;
      }
      else
      { // Else move up/down ladder or hold current location
        climbLadder.Climb(climbDirection);
      }
    }
  }
}
