using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Pace : MonoBehaviour
{
  WalkMovement walkMovement;
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    walkMovement.inputWalkDirection = 1;
  }
}
