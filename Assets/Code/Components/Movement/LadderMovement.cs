using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
  #region Data
  [NonSerialized]
  public float climbDirection;

  public event Action onGettingOffLadder;
  Rigidbody2D myBody;
  Feet feet;
  [SerializeField]
  float climbSpeed = 1;
  [SerializeField]
  bool canClimbBrokenLadders;
  public Ladder currentLadder
  {
    get; private set;
  }
  bool _isOnLadder;
  #endregion

  #region Properties
  public bool isOnLadder
  {
    get
    {
      return _isOnLadder;
    }
    set
    {
      Debug.Assert(value == false || currentLadder != null);
      if(isOnLadder == value)
      {
        return;
      }

      _isOnLadder = value;

      if(isOnLadder)
      {
        myBody.GetComponent<Collider2D>().isTrigger = true;
        myBody.gravityScale = 0;
      }
      else
      {
        climbDirection = 0;
        myBody.GetComponent<Collider2D>().isTrigger = false;
        myBody.gravityScale = 1;
      }

      if(isOnLadder == false
        && onGettingOffLadder != null)
      {
        onGettingOffLadder();
      }
    }
  }
  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponent<Feet>();
  }
  #endregion

  #region Events
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    {
      return;
    }
    if(canClimbBrokenLadders || ladder.isBroken == false)
    {
      currentLadder = ladder;
    }
  }

  void OnTriggerExit2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    {
      return;
    }
    if(currentLadder != ladder)
    { // Exiting a different trigger than the one we are tracking ATM
      return;
    }
    currentLadder = null;
    isOnLadder = false;
  }

  void FixedUpdate()
  {
    FixedUpdate_ConsiderGettingOnLadderGivenInput();
  }
  #endregion

  #region Helpers
  void FixedUpdate_ConsiderGettingOnLadderGivenInput()
  {
    Ladder ladder = currentLadder;
    if(ladder == null)
    {
      return;
    }
    if(isOnLadder == false
          && Mathf.Abs(climbDirection) > 0.01
          && ((climbDirection > 0 && myBody.position.y < ladder.bounds.center.y)
            || (climbDirection < 0 && myBody.position.y > ladder.bounds.center.y)))
    { // Get on if moving up and on lower half or moving down and on upper half
      isOnLadder = true;
    }

    if(isOnLadder)
    {
      float currentVerticalVelocity = myBody.velocity.y;
      if(feet.distanceToGround > .1f && feet.distanceToGround < .3f
        && ((currentVerticalVelocity > 0 && myBody.position.y > ladder.bounds.center.y)
          || (currentVerticalVelocity < 0 && myBody.position.y < ladder.bounds.center.y)
        ))
      { // Get off if feet near ground and moving towards end of ladder
        isOnLadder = false;
      }
      else
      { // Else move up/down ladder or hold current location
        myBody.velocity = new Vector2(myBody.velocity.x, climbDirection * climbSpeed * Time.fixedDeltaTime);
      }
    }
  }
  #endregion
}
