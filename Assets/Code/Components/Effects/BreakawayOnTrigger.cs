using UnityEngine;
using System;

/// <summary>
/// When a collider is triggered, enable physics (gravity/bouncing) on this object.
/// </summary>
public class BreakawayOnTrigger : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The total number of boxes still in tact on the screen.
  /// </summary>
  public static int numberOfInTactBlocks;
  
  /// <summary>
  /// Called anytime a block is broken.
  /// </summary>
  public static event Action onBreakaway;

  /// <summary>
  /// The parent's rigidbody, used to enable gravity.
  /// </summary>
  Rigidbody2D myParentBody;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate variables.
  /// </summary>
  protected void Awake()
  {
    Debug.Assert(transform.parent != null);

    myParentBody = transform.parent.GetComponent<Rigidbody2D>();

    Debug.Assert(myParentBody != null);
  }

  /// <summary>
  /// On start, add this to the number of in-tact blocks.
  /// </summary>
  protected void Start()
  {
    numberOfInTactBlocks++;
  }

  /// <summary>
  /// On disable or destroy, remove this from the number of in-tact blocks.  And fire breakaway event.
  /// </summary>
  protected void OnDisable()
  {
    numberOfInTactBlocks--;
    if(onBreakaway != null)
    {
      onBreakaway();
    }

    Debug.Assert(numberOfInTactBlocks >= 0);
  }
  #endregion

  #region Events
  /// <summary>
  /// On trigger, break away.
  /// Only called for the player c/o of the Physics matrix.
  /// </summary>
  /// <param name="collision">The thing which hit us (the player).</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    // Enable physics
    myParentBody.constraints = RigidbodyConstraints2D.None;

    // Disable one-way platformer effect
    PlatformEffector2D effector = GetComponentInParent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.useOneWay = false;
    }
    enabled = false; // Disable this script, triggering the breakaway event
  }
  #endregion
}
