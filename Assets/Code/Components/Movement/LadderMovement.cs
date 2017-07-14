using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
  #region Data
  [NonSerialized]
  public float climbDirection;

  public event Action onGettingOnLadder;
  public event Action onGettingOffLadder;
  Rigidbody2D myBody;
  Feet feet;
  [SerializeField]
  float climbSpeed = 1;
  [SerializeField]
  bool canClimbBrokenLadders;

  bool _isOnLadder;

  SetTriggerCommand triggerCommand;
  List<Ladder> currentLadderList;
  #endregion

  #region Properties
  public Ladder currentLadder
  {
    get
    {
      if(currentLadderList.Count == 0)
      {
        return null;
      }

      Ladder closestLadder = null;
      for(int i = 0; i < currentLadderList.Count; i++)
      {
        Ladder ladder = currentLadderList[i];
        if(closestLadder == null ||
          (ladder.transform.position - transform.position).sqrMagnitude < (closestLadder.transform.position - transform.position).sqrMagnitude)
        {
          closestLadder = ladder;
        }
      }

      return closestLadder;
    }
  }

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
        Debug.Assert(triggerCommand == null);

        triggerCommand = new SetTriggerCommand(gameObject);
        myBody.gravityScale = 0;
      }
      else
      {
        triggerCommand.Undo();
        triggerCommand = null;
        climbDirection = 0;
        myBody.GetComponent<Collider2D>().isTrigger = false;
        myBody.gravityScale = 1;
      }

      if(isOnLadder == false
        && onGettingOffLadder != null)
      {
        onGettingOffLadder();
      }
      else if(isOnLadder && onGettingOnLadder != null)
      {
        onGettingOnLadder();
      }
    }
  }
  #endregion

  #region Init
  void Awake()
  {
    currentLadderList = new List<Ladder>();
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponentInChildren<Feet>();
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
      currentLadderList.Add(ladder);
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
    if(ladder == currentLadder)
    {
      isOnLadder = false;
    }
    currentLadderList.Remove(ladder);
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
        myBody.velocity = new Vector2(0, climbDirection * climbSpeed * Time.fixedDeltaTime);
      }
    }
  }
  #endregion
}
