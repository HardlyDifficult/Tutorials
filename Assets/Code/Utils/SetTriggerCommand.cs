using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class SetTriggerCommand
{
  List<Collider2D> impactedColliderList;

  public SetTriggerCommand(
    GameObject gameObject)
  {
    impactedColliderList = new List<Collider2D>();
    Collider2D[] colliderList = gameObject.GetComponentsInChildren<Collider2D>();
    for(int i = 0; i < colliderList.Length; i++)
    {
      Collider2D collider = colliderList[i];
      if(collider.isTrigger == false)
      {
        impactedColliderList.Add(collider);
        collider.isTrigger = true;
      }
    }
  }

  public void Undo()
  {
    for(int i = 0; i < impactedColliderList.Count; i++)
    {
      Collider2D collider = impactedColliderList[i];
      collider.isTrigger = false;
    }
  }
}
