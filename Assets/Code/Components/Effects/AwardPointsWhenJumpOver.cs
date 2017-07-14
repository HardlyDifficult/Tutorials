using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives the player points when they jump over this object.
/// Requires a child object with it's own Kinematic Rigidbody2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class AwardPointsWhenJumpOver : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How many points to give the player when the jump over this object.
  /// </summary>
  [SerializeField]
  int numberOfPointsToAward = 100;

  /// <summary>
  /// This layermask is used to raycast after a collision, checking if there is something blocking 
  /// our view of the player.
  /// For this to work, this mask must include the player.
  /// </summary>
  [SerializeField]
  LayerMask maskOfPlayerPlusObstaclesWhichMayBlockPoints;

  /// <summary>
  /// The collider used to trigger awarding points.
  /// </summary>
  BoxCollider2D myTriggerCollider;

  /// <summary>
  /// The collider that defines the bounds for this entity.
  /// </summary>
  Collider2D myEntityBoundsCollider;
  #endregion

  #region Init
  /// <summary>
  /// Initialize variables.
  /// </summary>
  protected void Awake()
  {
    Debug.Assert(numberOfPointsToAward >= 0);

    myTriggerCollider = GetComponent<BoxCollider2D>();
    myEntityBoundsCollider = transform.parent.GetComponent<Collider2D>();

    Debug.Assert(myTriggerCollider != null);
    Debug.Assert(myEntityBoundsCollider != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// On trigger enter (which c/o the physics matrix will only occur for the player),
  /// check if there is anything between this object and the player... if not - award points and prevent future use.
  /// </summary>
  /// <param name="collision">The gameObject we just hit</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    const float boxCastHeight = .01f;
    Vector2 rayStart = new Vector2(myEntityBoundsCollider.bounds.center.x, myEntityBoundsCollider.bounds.max.y + boxCastHeight * 2 + .15f); // .15f is a small buffer to ensure we don't hit ourselves
    RaycastHit2D hit = Physics2D.BoxCast(rayStart, new Vector2((myTriggerCollider.bounds.extents.x + myTriggerCollider.edgeRadius + .1f) * 2, boxCastHeight), 0, Vector2.up, 10, maskOfPlayerPlusObstaclesWhichMayBlockPoints);
    if(hit.collider != collision)
    { // There is something between me and the player
      return;
    }

    GameController.instance.points += numberOfPointsToAward;

    Destroy(this); // Single use (prevents this event from being fired again but does not destroy the gameObject)
  }
  #endregion
}
