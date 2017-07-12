using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Explosion
{
  static GameObject _explosion;
  static GameObject explosion
  {
    get
    {
      if(_explosion == null)
      {
        _explosion = Resources.Load<GameObject>("Explosion");
        Debug.Assert(_explosion != null);
      }

      return _explosion;
    }
  }

  public static void ExplodeAt(
    Vector2 position)
  {
    MonoBehaviour.Instantiate(explosion, position, Quaternion.identity);
  }
}
