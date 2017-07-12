using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DieOutOfBounds : MonoBehaviour
{
  void FixedUpdate()
  {
    if(transform.position.y < -12)
    { // Fell out of bounds
      Destroy(gameObject);
    }
  }
}
