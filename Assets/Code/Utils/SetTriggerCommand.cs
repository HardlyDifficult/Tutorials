using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This uses the 'Command pattern' to disable colliders on a gameObject (by changing to triggers).
/// It stores the colliders modified so it may undo the change later 
/// (without this we may make a trigger a collider by mistake).
/// </summary>
public class SetTriggerCommand
{
  #region Data
  /// <summary>
  /// The colliders which were modified on construction.  Saved to enable undo later on.
  /// </summary>
  List<Collider2D> impactedColliderList;
  #endregion

  #region API
  /// <summary>
  /// Disables all colliders on the gameObject and stores them allowing undo later.
  /// </summary>
  /// <param name="gameObject">The gameObject to disable colliders for.</param>
  public SetTriggerCommand(
    GameObject gameObject)
  {
    impactedColliderList = new List<Collider2D>();
    Collider2D[] colliderList = gameObject.GetComponentsInChildren<Collider2D>();
    for(int i = 0; i < colliderList.Length; i++)
    {
      Collider2D collider = colliderList[i];
      if(collider.isTrigger == false) 
      { // Only modify colliders (vs triggers)
        impactedColliderList.Add(collider);
        collider.isTrigger = true;
      }
    }
  }

  /// <summary>
  /// Re-enable all colliders this command originally disabled.
  /// </summary>
  public void Undo()
  {
    for(int i = 0; i < impactedColliderList.Count; i++)
    {
      Collider2D collider = impactedColliderList[i];
      collider.isTrigger = false;
    }
  }
  #endregion
}
