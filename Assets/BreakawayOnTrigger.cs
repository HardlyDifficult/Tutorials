using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BreakawayOnTrigger : MonoBehaviour
{
  public static int numberOfInTactBlocks;
  public static event Action onBreakaway;
  Rigidbody2D myBody;

  void OnEnable()
  {
    myBody = GetComponent<Rigidbody2D>();
    numberOfInTactBlocks++;
  }

  void OnDisable()
  {
    numberOfInTactBlocks--;
    if(onBreakaway != null)
    {
      onBreakaway();
    }
  }

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(enabled == false)
    {
      return;
    }
    myBody.constraints = RigidbodyConstraints2D.None;
    PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.useOneWay = false;
    }
    enabled = false;
  }
}
