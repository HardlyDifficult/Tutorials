using UnityEngine;

/// <summary>
/// Used on gameObjects representing life points remaining (one gameObject per life point).
/// This component Destroys gameObjects when a player dies if the lifeCount is less than what this gameObject represents.
/// </summary>
public class LifeLine : MonoBehaviour, ICareWhenPlayerDies
{
  #region Data
  /// <summary>
  /// This gameObject should be displayed if the player has this many lives remaining, or more.
  /// </summary>
  [SerializeField]
  int lifeCount = 1;
  #endregion

  #region Events
  /// <summary>
  /// When the player dies, consider destoying this gameObject as well.
  /// </summary>
  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    Debug.Assert(lifeCount >= 0);

    if(GameController.instance.lifeCounter < lifeCount)
    {
      gameObject.CallIDie();
    }
  }
  #endregion
}
