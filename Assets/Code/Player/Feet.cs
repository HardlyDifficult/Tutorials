using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Feet : MonoBehaviour
{
  #region Data
  LayerMask layerMaskFloor;
  Collider2D myCollider;
  Rigidbody2D myBody;
  #endregion

  #region Properties
  public bool isGrounded
  {
    get
    {
      return currentFloor != null;
    }
  }

  Collider2D currentFloor
  {
    get
    {
      if(myBody.gravityScale < 0.001)
      { // Not grounded when climbing a ladder
        return null;
      }
      ContactFilter2D filter = new ContactFilter2D()
      {
        layerMask = layerMaskFloor,
        useLayerMask = true
      };

      Collider2D[] results = new Collider2D[1];
      if(Physics2D.OverlapCollider(myCollider, filter, results) == 0)
      {
        return null;
      }
      if(Vector2.Dot(Vector2.up, results[0].Distance(myCollider).normal) < 0)
      {
        return null;
      }
      return results[0];
    }
  }

  public Quaternion floorRotation
  {
    get
    {
      Collider2D floor = currentFloor;
      if(floor == null)
      {
        return Quaternion.identity;
      }
      return floor.transform.rotation;
    }
  }

  public float? distanceToGround
  {
    get
    {
      RaycastHit2D hit = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, 1, layerMaskFloor);
      if(hit.collider == null)
      {
        return null;
      }
      return hit.distance;
    }
  }
  #endregion

  #region Events
  void Awake()
  {
    layerMaskFloor = LayerMask.GetMask(new[] { "Floor" });
    myBody = GetComponentInParent<Rigidbody2D>();
    myCollider = GetComponent<Collider2D>();
  }
  #endregion
}
