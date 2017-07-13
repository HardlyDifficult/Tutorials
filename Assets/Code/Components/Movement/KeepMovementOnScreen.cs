using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class KeepMovementOnScreen : MonoBehaviour
{
  WalkMovement walkMovement;
  Rigidbody2D myBody;

  void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    myBody = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    if(GameController.instance.screenBounds.Contains(transform.position) == false)
    { // Enstity is out of bounds
      transform.position = GameController.instance.screenBounds.ClosestPoint(transform.position);
      walkMovement.inputWalkDirection = -walkMovement.inputWalkDirection;
    }
    else if(myBody.velocity.sqrMagnitude < .001)
    { // Entity is stuck
      walkMovement.inputWalkDirection = -walkMovement.inputWalkDirection;
    }
  }
}
