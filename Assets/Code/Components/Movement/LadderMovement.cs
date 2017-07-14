using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the motion up/down ladders.
/// Another components uses climbDirection and isOnLadder to control when to use a ladder.
/// When on a ladder, this component overwrites the rigidbody velocity.y (not the .x)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class LadderMovement : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Set by another component to attempt climbing a ladder up/down.
  /// </summary>
  [NonSerialized]
  public float climbDirection;

  /// <summary>
  /// Called when the entity first gets on a ladder.
  /// </summary>
  public event Action onGettingOnLadder;

  /// <summary>
  /// Called when the entity gets off a ladder it was previously climbing.
  /// </summary>
  public event Action onGettingOffLadder;

  /// <summary>
  /// Used to turn off gravity while we are climbing.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Used to determine the distance to the ground, for determining when to get off a ladder.
  /// </summary>
  Feet feet;

  /// <summary>
  /// How quickly the entity moves up/down ladders.
  /// </summary>
  [SerializeField]
  float climbSpeed = 60;

  /// <summary>
  /// True if the entity can climb broken ladders.
  /// </summary>
  [SerializeField]
  bool canClimbBrokenLadders;

  /// <summary>
  /// True if the entity is currently on a ladder.
  /// This data backs a property of the same name below.
  /// </summary>
  bool _isOnLadder;

  /// <summary>
  /// Used to turn off colliders when we get on a ladder, and then turn them back on when we get off a ladder.
  /// This allows us to walk through floors while climbing.
  /// </summary>
  SetTriggerCommand triggerCommand;

  /// <summary>
  /// Via trigger enter/exit we maintain a list of all the ladders the entity is currently standing on.
  /// </summary>
  List<Ladder> currentLadderList;
  #endregion

  #region Properties
  /// <summary>
  /// The best fit ladder the entity is standing on/near, if any.
  /// </summary>
  public Ladder currentLadder
  {
    get
    {
      if(currentLadderList.Count == 0)
      { // We are not near any ladder ATM
        return null;
      }

      Ladder closestLadder = null;
      for(int i = 0; i < currentLadderList.Count; i++)
      {
        Ladder ladder = currentLadderList[i];
        if(closestLadder == null ||
          (ladder.transform.position - transform.position).sqrMagnitude < (closestLadder.transform.position - transform.position).sqrMagnitude)
        { // Select the closest ladder, if we are standing near several
          closestLadder = ladder;
        }
      }

      return closestLadder;
    }
  }

  /// <summary>
  /// True if the entity is actively climbing a ladder.
  /// On set, events are fired and physics updated on the entity (to allow moving through the floor).
  /// </summary>
  public bool isOnLadder
  {
    get
    {
      return _isOnLadder;
    }
    private set
    {
      Debug.Assert(value == false || currentLadder != null);
      if(isOnLadder == value)
      { // The value is not changing
        return;
      }

      _isOnLadder = value;
      
      if(isOnLadder)
      { // When we first get on a ladder, disable physics to allow climbing through the floor
        Debug.Assert(triggerCommand == null);

        if(onGettingOnLadder != null)
        { // Fire event
          onGettingOnLadder();
        }

        triggerCommand = new SetTriggerCommand(gameObject);
        myBody.gravityScale = 0;
        myBody.velocity = Vector2.zero;
      }
      else
      { // When we get off a ladder, re-enable physics
        if(onGettingOffLadder != null)
        { // Fire event
          onGettingOffLadder();
        }

        triggerCommand.Undo();
        triggerCommand = null;
        climbDirection = 0;
        myBody.GetComponent<Collider2D>().isTrigger = false;
        myBody.gravityScale = 1;
      }     
    }
  }
  #endregion

  #region Init
  /// <summary>
  /// On awake, initialize variables.
  /// </summary>
  protected void Awake()
  {
    currentLadderList = new List<Ladder>();
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponentInChildren<Feet>();

    Debug.Assert(myBody != null);
    Debug.Assert(feet != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// When we encounter a new ladder, add it to the list.
  /// </summary>
  /// <param name="collision">The gameObject we just encountered.</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    { // The thing we hit is not a ladder, ignore.
      return;
    }

    if(canClimbBrokenLadders || ladder.isBroken == false)
    { // Ignore any broken ladders if we can't use them
      currentLadderList.Add(ladder);
    }
  }

  /// <summary>
  /// When we walk away from a ladder, remove it from the currentLadderList.
  /// </summary>
  /// <param name="collision">The gameObject we are walking away from.</param>
  protected void OnTriggerExit2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    { // This is not a ladder.
      return;
    }
    if(ladder == currentLadder)
    { // If the ladder being removed is the currentLadder, force getting off.
      isOnLadder = false;
    }

    // Remove the ladder from the list
    currentLadderList.Remove(ladder);
  }

  /// <summary>
  /// Consider getting on/off a ladder given climbDirection. When on a ladder, control the entity's y movement.
  /// </summary>
  protected void FixedUpdate()
  {
    Ladder ladder = currentLadder;
    if(ladder == null)
    { // If we are not near a ladder, there's nothing to do
      return;
    }

    if(Mathf.Abs(climbDirection) > 0.01
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
          || (currentVerticalVelocity < 0 && myBody.position.y < ladder.bounds.center.y)))
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

  #region API
  /// <summary>
  /// Allows other components to initiate getting off the ladder.
  /// </summary>
  /// <remarks>
  /// We don't use a public property for this because we don't want other scripts to set isOnLadder to true directly.
  /// </remarks>
  public void GetOffLadder()
  {
    isOnLadder = false;
  }
  #endregion
}
