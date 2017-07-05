using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Feet : MonoBehaviour
{
  public bool isGrounded
  {
    get
    {
      return collisionCount > 0;
    }
  }
  public Collider2D lastFloorHit
  {
    get; private set;
  }
  int collisionCount;

  void OnTriggerEnter2D(Collider2D collision)
  {
    collisionCount++;
    lastFloorHit = collision;
  }

  void OnTriggerExit2D(Collider2D collision)
  {
    collisionCount--;
  }
}
