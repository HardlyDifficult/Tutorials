using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  [SerializeField]
  LayerMask layerToTriggerOn;

  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    if(layerToTriggerOn.Includes(collision.gameObject.layer))
    {
      GameObject.FindObjectOfType<LevelManager>().YouWin();
    }
  }
}
