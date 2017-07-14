using UnityEngine;

/// <summary>
/// Used to determine if the entity is on the ground.  Also provides properties about the ground we are standing on.
/// This component may be placed on the main entity gameObject or a child gameObject.  A child may be used to offset the feet
/// from the collider used for other things (e.g. the fly guys invisible feet should not kill the character).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Feet : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The collider on this gameObject, used to determine if we are currently on the ground (vs jumping).
  /// </summary>
  Collider2D myCollider;

  /// <summary>
  /// The ladder movement on this gameObject (or the parent gameObject), used to determine when we are climbing.
  /// </summary>
  LadderMovement ladderMovement;

  /// <summary>
  /// Sets a LayerMask to floor for use when calling Physics to check if we are on ground.
  /// </summary>
  ContactFilter2D floorFilter;
  #endregion

  #region Properties
  /// <summary>
  /// True if the entity is currently standing on the ground.
  /// </summary>
  public bool isGrounded
  {
    get
    {
      return currentFloor != null;
    }
  }

  /// <summary>
  /// The collider of the floor we are standing on, if any.
  /// </summary>
  Collider2D currentFloor
  {
    get
    {
      if(ladderMovement.isOnLadder)
      { // Not grounded when climbing a ladder
        return null;
      }

      Collider2D[] results = new Collider2D[10];
      if(Physics2D.OverlapCollider(myCollider, floorFilter, results) == 0)
      { // Not on any floor
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
        ColliderDistance2D distance = collider.Distance(myCollider);
        if(distance.distance >= -.1f // The feet collider is on or above the floor (vs jumping up through a floor)
          && Vector2.Dot(Vector2.up, distance.normal) > 0) // Only accept collisions with the top of the floor (b/c we use one-way colliders / platformer effect)
        {
          foundFloor = collider;
          break;
        }
      }

      return foundFloor;
    }
  }

  /// <summary>
  /// Returns the normal for the floor we are standing on.
  /// </summary>
  public Vector2 floorUp
  {
    get
    {
      Collider2D floor = firstFloorUnderUs;
      if(floor == null)
      {// If we can't find a floor, lie...
        return Vector2.up;
      }

      // The transform up represents the platform's normal because any rotation in the platform sprite 
      // is part of it's gameObject (vs drawn with rotation or rotated in a child object).
      return floor.transform.up;
    }
  }

  /// <summary>
  /// Returns the currentFloor or if in air, raycast to find the first floor under us if we can.
  /// </summary>
  Collider2D firstFloorUnderUs
  {
    get
    {
      Collider2D floor = currentFloor;
      if(floor == null)
      { // Raycast to find any floor under us if we can.
        RaycastHit2D[] result = new RaycastHit2D[1];
        if(Physics2D.Raycast(myCollider.transform.position, Vector2.down, floorFilter, result) == 0)
        { // Can't find any floor
          return null;
        }
        floor = result[0].collider;
      }

      return floor;
    }
  }

  /// <summary>
  /// Returns the rotation for the floor we are standing on.
  /// </summary>
  public Quaternion floorRotation
  {
    get
    {
      Collider2D floor = firstFloorUnderUs;
      if(floor == null)
      { // if we can't find a floor, lie...
        return Quaternion.identity;
      }
      Quaternion rotation = floor.transform.rotation;

      if(Quaternion.Dot(rotation, Quaternion.identity) <= 0)
      { // Ignore floor blocks which are upside down ATM
        return Quaternion.identity;
      }

      return rotation;
    }
  }

  /// <summary>
  /// How far above the ground we are ATM.  Null if there is no floor under us ATM.
  /// </summary>
  public float? distanceToGround
  {
    get
    {
      RaycastHit2D[] result = new RaycastHit2D[1];
      if(Physics2D.Raycast(myCollider.bounds.center, Vector2.down, floorFilter, result) == 0)
      { // Can't find any floor..
        return null;
      }

      return result[0].distance;
    }
  }
  #endregion

  #region Init
  /// <summary>
  /// On awake, initial variables.
  /// </summary>
  protected void Awake()
  {
    ladderMovement = GetComponentInParent<LadderMovement>();
    myCollider = GetComponent<Collider2D>();

    floorFilter = new ContactFilter2D()
    {
      layerMask = LayerMask.GetMask(new[] { "Floor" }),
      useLayerMask = true
    };

    Debug.Assert(ladderMovement != null);
    Debug.Assert(myCollider != null);
  }
  #endregion
}
