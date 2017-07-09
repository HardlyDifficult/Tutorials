using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTo0 : MonoBehaviour
{
  void Update()
  {
    transform.rotation = Quaternion.identity;
  }
}
