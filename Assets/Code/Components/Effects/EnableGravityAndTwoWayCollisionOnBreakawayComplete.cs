using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class EnableGravityAndTwoWayCollisionOnBreakawayComplete  : MonoBehaviour
{
  void Start()
  {
    BreakawayOnTrigger.onBreakaway += BreakawayOnTrigger_onBreakaway;
  }

  void OnDestroy()
  {
    BreakawayOnTrigger.onBreakaway -= BreakawayOnTrigger_onBreakaway;
  }

  void BreakawayOnTrigger_onBreakaway()
  {
    if(BreakawayOnTrigger.numberOfInTactBlocks > 0)
    {
      return;
    }

    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.useOneWay = false;
    }
  }
}
