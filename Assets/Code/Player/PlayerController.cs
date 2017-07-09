using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ClimbLadder))]
public class PlayerController : MonoBehaviour
{
  #region Data
  public static PlayerController instance;

  public float maxSpeed = 1;
  public float speed = 1;
  public float jumpSpeed = 1;
  public float characterHeight = 1;

  public Rigidbody2D myBody;
  Feet feet;
  [NonSerialized]
  public float lastJumpTime;
  ClimbLadder climbLadder;
  #endregion

  #region Init
  void Awake()
  {
    instance = this;
    myBody = GetComponent<Rigidbody2D>();
    climbLadder = GetComponent<ClimbLadder>();
    feet = GetComponentInChildren<Feet>();
  }

  void OnDestroy()
  {
    instance = null;
  }
  #endregion

  void Update()
  {
    if(feet.isGrounded)
    {
      Update_Jump();
      Update_RotateCharacterToMatchPlatform();
    }
    Update_ClimbLadder();
  }

  void Update_ClimbLadder()
  { // TODO separate player controls from movement (for bad guys)
    Ladder ladder = climbLadder.currentLadder;
    if(ladder == null)
    {
      return;
    }
    float vertical = Input.GetAxis("Vertical");
    if(climbLadder.isOnLadder == false
      && Mathf.Abs(vertical) > 0.01
      && ((vertical < 0 && myBody.position.y > ladder.bounds.center.y)
        || (vertical > 0 && myBody.position.y < ladder.bounds.center.y)))
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
        climbLadder.Climb(vertical);
      }
    }
  }

  void Update_Jump()
  {
    if(Input.GetButtonDown("Jump")
            && Time.timeSinceLevelLoad - lastJumpTime > .1f)
    {
      myBody.AddForce((Vector2)transform.up * jumpSpeed, ForceMode2D.Impulse);
      lastJumpTime = Time.timeSinceLevelLoad;
    }
  }

  void Update_RotateCharacterToMatchPlatform()
  {
    Quaternion targetRotation = feet.floorRotation; //floor.transform.rotation;
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .4f);
  }
}
