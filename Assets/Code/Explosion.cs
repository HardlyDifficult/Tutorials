using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
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
    Instantiate(explosion, position, Quaternion.identity);
  }
}
