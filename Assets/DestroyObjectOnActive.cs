using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnActive : MonoBehaviour
{
  [SerializeField]
  GameObject gameObjectToDestroy;

  void OnEnable()
  {
    Destroy(gameObjectToDestroy);
  }
}
