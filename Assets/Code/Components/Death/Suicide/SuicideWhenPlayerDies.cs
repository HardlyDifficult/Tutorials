using UnityEngine;

/// <summary>
/// When the player dies, this object dies as well.
/// </summary>
public class SuicideWhenPlayerDies : MonoBehaviour, ICareWhenPlayerDies
{
  #region Events
  /// <summary>
  /// The player died, Destroy the gameObject.
  /// </summary>
  void ICareWhenPlayerDies.OnPlayerDeath()
  { // TODO move to a separate component
    Destroy(gameObject);
  }
  #endregion
}
