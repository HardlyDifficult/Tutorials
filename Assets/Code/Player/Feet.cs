using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Feet : MonoBehaviour
{
  #region Data
  Collider2D playerFeetCollider;
  Rigidbody2D myBody;
  ContactFilter2D filter;
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

      Collider2D[] results = new Collider2D[10];
      if(Physics2D.OverlapCollider(playerFeetCollider, filter, results) == 0)
      {
        return null;
      }
      Collider2D foundFloor = null;
      for(int i = 0; i < results.Length; i++)
      {
        Collider2D collider = results[i];
        if(collider == null)
        {
          continue;
        }
        ColliderDistance2D distance = collider.Distance(playerFeetCollider);
        if(distance.distance >= -.1f && Vector2.Dot(Vector2.up, distance.normal) > 0)
        {
          foundFloor = collider;
          break;
        } else {
          print(distance.distance);
        }
      }

      return foundFloor;
    }
  }

  public Vector2 floorUp
  {
    get
    {
      Collider2D floor = currentFloor;
      if(floor == null)
      {
        RaycastHit2D[] result = new RaycastHit2D[1];
        if(Physics2D.Raycast(myBody.transform.position, Vector2.down, filter, result) == 0)
        {
          print("No floor");
          return Vector2.up;
        }
        floor = result[0].collider;
      }
      return floor.transform.up;
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
      RaycastHit2D[] result = new RaycastHit2D[1];
      if(Physics2D.Raycast(playerFeetCollider.bounds.center, Vector2.down, filter, result) == 0) 
      {
        return null;
      }
      return result[0].distance;
    }
  }
  #endregion

  #region Events
  void Awake()
  {
    filter = new ContactFilter2D()
    {
      layerMask = LayerMask.GetMask(new[] { "Floor" }),
      useLayerMask = true
    };
    myBody = GetComponentInParent<Rigidbody2D>();
    playerFeetCollider = GetComponent<Collider2D>();
  }
  #endregion
}
