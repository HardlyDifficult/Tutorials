using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnActive : MonoBehaviour
{
  [SerializeField]
  GameObject gameObjectToDestroy;

  void OnEnable()
  {
    Destroy(gameObjectToDestroy);
  }
}
