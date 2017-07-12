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
  
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 10, maskOfPlayerPlusObstaclesWhichMayBlockPoints);
    if(hit.collider != collision)
    { // There is something between me and the player
      return;
    }
    GameController.instance.points += numberOfPointsToAward;
  }
}
