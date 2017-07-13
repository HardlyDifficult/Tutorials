using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Requires a child object with it's own Kinematic Rigidbody2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AwardPointsWhenJumpOver : MonoBehaviour
{
  [SerializeField]
  int numberOfPointsToAward = 100;
  [SerializeField]
  LayerMask maskOfPlayerPlusObstaclesWhichMayBlockPoints;

  BoxCollider2D collider;

  void Awake()
  {
    collider = GetComponent<BoxCollider2D>();
    Debug.Assert(collider != null);
  }

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    const float boxCastHeight = .01f;
    Vector2 rayStart = new Vector2(collider.bounds.center.x, collider.bounds.max.y + collider.edgeRadius + boxCastHeight * 2);
    RaycastHit2D hit = Physics2D.BoxCast(rayStart, new Vector2((collider.bounds.extents.x + collider.edgeRadius) * 2, boxCastHeight), 0, Vector2.up, 10, maskOfPlayerPlusObstaclesWhichMayBlockPoints);
    if(hit.collider != collision)
    { // There is something between me and the player
      return;
    }
    GameController.instance.points += numberOfPointsToAward;
    Destroy(this); // Single use
  }
}
