using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  public static int numberActive;

  [SerializeField]
  LayerMask layerToTriggerOn;
  bool isActive;

  protected void Start()
  {
    numberActive++;
    isActive = true;
  }

  protected void OnDisable()
  {
    if(isActive)
    {
      Deactivate();
    }
  }

  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    if(layerToTriggerOn.Includes(collision.gameObject.layer))
    {
      GameObject.FindObjectOfType<LevelManager>().YouWin();
      Deactivate();
    }
  }

  void Deactivate()
  {
    numberActive--;
    isActive = false;
  }
}
