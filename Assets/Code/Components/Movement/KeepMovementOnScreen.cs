using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class KeepMovementOnScreen : MonoBehaviour
{
  WalkMovement walkMovement;

  void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
  }

  void Update()
  {
    if(GameController.instance.screenBounds.Contains(transform.position) == false)
    { // Player is out of bounds
      transform.position = GameController.instance.screenBounds.ClosestPoint(transform.position);
      walkMovement.inputWalkDirection = -walkMovement.inputWalkDirection;
    }
  }
}
