using UnityEngine;

/// <summary>
/// Destroy's the gameObject as soon as this component activates.
/// You can place this on a gameObject and then activate when needed (e.g. by an animation).
/// </summary>
public class KillOnActive : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The object to kill (may point to anything in the scene).
  /// </summary>
  [SerializeField]
  GameObject gameObjectToDestroy;
  #endregion

  #region Events
  /// <summary>
  /// On enable, kill the game object.
  /// </summary>
  void OnEnable()
  {
    Destroy(gameObjectToDestroy);
  }
  #endregion
}
