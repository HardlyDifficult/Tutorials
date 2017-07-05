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
      return lastFloorHit != null;
    }
  }
  public Collider2D lastFloorHit
  {
    get; private set;
  }
  Collider2D currentFloorHit;

  private void OnTriggerStay2D(Collider2D collision)
  {
    currentFloorHit = collision;
  }
  
  private void FixedUpdate()
  {
    lastFloorHit = currentFloorHit;
    currentFloorHit = null;
  }
}
