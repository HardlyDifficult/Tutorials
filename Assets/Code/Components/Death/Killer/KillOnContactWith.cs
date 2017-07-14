using UnityEngine;
using System;

/// <summary>
/// Kills anything on contact which matches the provided LayerMask.
/// </summary>
public class KillOnContactWith : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Defines which layers will be killed on contact.
  /// </summary>
  [SerializeField]
  LayerMask layersToKill;
  #endregion

  #region Init
  /// <summary>
  /// Unity Hack to allow enable/disable (used by the collision/trigger events).
  /// </summary>
  void Start() { }
  #endregion

  #region Events
  /// <summary>
  /// On collision, consider killing the thing we touched.
  /// </summary>
  /// <param name="collision">The thing we touched.</param>
  void OnCollisionEnter2D(
    Collision2D collision)
  {
    TryKilling(collision.gameObject);
  }

  /// <summary>
  /// On trigger, consider killing the thing we touched.
  /// </summary>
  /// <param name="collision">The thing we touched.</param>
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    TryKilling(collision.gameObject);
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Checks if we should kill the object just touched, if so trigger death effects and Destroy it.
  /// </summary>
  /// <param name="gameObjectWeJustHit">The gameObject just touched</param>
  void TryKilling(
    GameObject gameObjectWeJustHit)
  {
    if(enabled == false
      || layersToKill.Includes(gameObjectWeJustHit.layer) == false)
    { // This object gets to live.
      return;
    }

    // Explode and trigger death effects.
    Explosion.ExplodeAt(gameObjectWeJustHit.transform.position);
    gameObjectWeJustHit.CallIDie();
  }
  #endregion
}