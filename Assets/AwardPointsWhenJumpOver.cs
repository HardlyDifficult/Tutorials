using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardPointsWhenJumpOver : MonoBehaviour
{
  [SerializeField]
  int numberOfPointsToAward;

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 10, ~LayerMask.GetMask(new[] { "Enemy", "Ladder" }));
    if(hit.collider != collision)
    { // There is something between me and the player
      print("Blocked by " + hit.collider);
      return;
    }
    print("GG " + collision.gameObject.name);
    GameController.instance.points += numberOfPointsToAward;
  }
}
