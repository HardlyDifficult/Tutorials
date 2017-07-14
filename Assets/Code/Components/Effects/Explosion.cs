using UnityEngine;

/// <summary>
/// A static class to make creating explosions easy.
/// </summary>
public static class Explosion
{
  #region Data
  /// <summary>
  /// A cache of the prefab, for performance.
  /// </summary>
  static GameObject _explosion;

  /// <summary>
  /// The prefab of the explosion to instatiate.  If it's not cached, load it and cache now.
  /// </summary>
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
  #endregion

  #region API
  /// <summary>
  /// Spawn an explosion.
  /// </summary>
  /// <param name="position">The position where the explosion should appear.</param>
  public static void ExplodeAt(
    Vector2 position)
  {
    MonoBehaviour.Instantiate(explosion, position, Quaternion.identity);
  }
  #endregion
}
