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
  /// A cache of the rigidbody, for performance.
  /// </summary>
  Rigidbody2D myBody;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate variables.
  /// </summary>
  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
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
  void OnDisable()
  {
    numberOfInTactBlocks--;
    if(onBreakaway != null)
    {
      onBreakaway();
    }
  }
  #endregion

  #region Events
  /// <summary>
  /// On trigger, break away.
  /// Only called for the player c/o of the Physics matrix.
  /// </summary>
  /// <param name="collision">The thing which hit us (the player).</param>
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    // Enable physics
    myBody.constraints = RigidbodyConstraints2D.None;

    // Disable one-way platformer effect
    PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.useOneWay = false;
    }
  }
  #endregion
}
