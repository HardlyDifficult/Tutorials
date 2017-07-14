using UnityEngine;

/// <summary>
/// Cause this object to fall after all the breakaway blocks have been broken.
/// </summary>
public class EnableGravityAndTwoWayCollisionOnBreakawayComplete  : MonoBehaviour
{
  #region Init
  /// <summary>
  /// On awake, register for events.
  /// </summary>
  protected void Awake()
  {
    BreakawayOnTrigger.onBreakaway += BreakawayOnTrigger_onBreakaway;
  }

  /// <summary>
  /// On destroy, deregister events.
  /// </summary>
  protected void OnDestroy()
  {
    BreakawayOnTrigger.onBreakaway -= BreakawayOnTrigger_onBreakaway;
  }
  #endregion

  #region Events
  /// <summary>
  /// Anytime a breakaway block is broken, cause object fall if no breakable blocks remain.
  /// </summary>
  void BreakawayOnTrigger_onBreakaway()
  {
    if(BreakawayOnTrigger.numberOfInTactBlocks > 0)
    { // There are still blocks pending breaking.
      return;
    }

    // Enable physics
    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

    // Disable 1-way collisions (the platformer effect)
    PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.useOneWay = false;
    }
  }
  #endregion
}
