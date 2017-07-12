using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToIdentity : MonoBehaviour
{
  void Update()
  {
    transform.rotation = Quaternion.identity;
  }
}
