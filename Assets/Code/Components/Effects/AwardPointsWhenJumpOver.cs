using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives the player points when they jump over this object.
/// Requires a child object with it's own Kinematic Rigidbody2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
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
  /// A cache of this object's collider for performance.
  /// </summary>
  BoxCollider2D myCollider;
  #endregion

  #region Init 
  /// <summary>
  /// On awake, populate variables.
  /// </summary>
  void Awake()
  {
    myCollider = GetComponent<BoxCollider2D>();
    Debug.Assert(myCollider != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// On trigger enter (which c/o the physics matrix will only occur for the player),
  /// check if there is anything between this object and the player... if not - award points and prevent future use.
  /// </summary>
  /// <param name="collision"></param>
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    const float boxCastHeight = .01f;
    Vector2 rayStart = new Vector2(myCollider.bounds.center.x, myCollider.bounds.max.y + myCollider.edgeRadius + boxCastHeight * 2);
    RaycastHit2D hit = Physics2D.BoxCast(rayStart, new Vector2((myCollider.bounds.extents.x + myCollider.edgeRadius) * 2, boxCastHeight), 0, Vector2.up, 10, maskOfPlayerPlusObstaclesWhichMayBlockPoints);
    if(hit.collider != collision)
    { // There is something between me and the player
      return;
    }

    GameController.instance.points += numberOfPointsToAward;

    Destroy(this); // Single use
  }
  #endregion
}
