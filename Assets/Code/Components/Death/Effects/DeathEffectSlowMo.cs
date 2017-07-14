using UnityEngine;

/// <summary>
/// Causes the game's timeScale to slow to a pause briefly when this object dies.
/// </summary>
public class DeathEffectSlowMo : MonoBehaviour, IHaveDeathEffect
{
  #region Data
  /// <summary>
  /// How long to lerp timeScale down to 0.
  /// </summary>
  [SerializeField]
  float timeTillPaused = .1f;

  /// <summary>
  /// How long to remain paused before speedup begins.
  /// </summary>
  [SerializeField]
  float timePausedTillSpeedUp = .5f;

  /// <summary>
  /// How long to lerp timeScale up to 1.
  /// </summary>
  [SerializeField]
  float timeForSpeedUp = .5f;

  /// <summary>
  /// How many points to award the player when this object dies.
  /// </summary>
  [SerializeField]
  int pointsPerDeath = 100;
  #endregion

  #region API
  /// <summary>
  /// Called when the object is killed.  Triggers time effect, awards points, and destroys the object.
  /// </summary>
  void IHaveDeathEffect.Die()
  {
    Debug.Assert(timeTillPaused >= 0);
    Debug.Assert(timePausedTillSpeedUp >= 0);
    Debug.Assert(timeForSpeedUp >= 0);
    Debug.Assert(timeTillPaused + timePausedTillSpeedUp + timeForSpeedUp > 0);
    Debug.Assert(pointsPerDeath >= 0);

    GameController.instance.timeController.SlowDownAndSpeedUp(timeTillPaused, timePausedTillSpeedUp, timeForSpeedUp);
    GameController.instance.points += pointsPerDeath;
    Destroy(gameObject);
  }
  #endregion
}
