using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Provides a method to manipulate timeScale, ensuring that multiple timeScale changes are not happening in parallel.
/// Place on the same GameObject as the GameController.
/// </summary>
[RequireComponent(typeof(GameController))]
public class TimeController : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The currently running time manipulation routine, if any.
  /// </summary>
  Coroutine routine;
  #endregion

  #region API
  /// <summary>
  /// Manipulates time, for a brief pause effect in-game.
  /// If there is already a time manipulation in progress then the call for another is ignored.
  /// </summary>
  /// <param name="timeTillPaused">How long to lerp timeScale down to 0.</param>
  /// <param name="timePausedTillSpeedUp">How long to remain paused before speedup begins.</param>
  /// <param name="timeForSpeedUp">How long to lerp timeScale up to 1.</param>
  public void SlowDownAndSpeedUp(
    float timeTillPaused,
    float timePausedTillSpeedUp,
    float timeForSpeedUp)
  {
    if(routine != null)
    { // Only one time effect at a time
      return;
    }

    routine = StartCoroutine(AnimateTime(timeTillPaused, timePausedTillSpeedUp, timeForSpeedUp));
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the time manipulation.
  /// </summary>
  /// <param name="timeTillPaused">How long to lerp timeScale down to 0.</param>
  /// <param name="timePausedTillSpeedUp">How long to remain paused before speedup begins.</param>
  /// <param name="timeForSpeedUp">How long to lerp timeScale up to 1.</param>
  /// <returns>Used to manage time for the coroutine.</returns>
  IEnumerator AnimateTime(
    float timeTillPaused,
    float timePausedTillSpeedUp,
    float timeForSpeedUp)
  {
    // Slowly pause game
    float timePassed = 0;
    while(timePassed < timeTillPaused)
    {
      float percentComplete = 1 - timePassed / timeTillPaused;
      Time.timeScale = percentComplete;

      yield return 0;
      timePassed += Time.unscaledDeltaTime;
    }

    // Hold pause
    yield return new WaitForSecondsRealtime(timePausedTillSpeedUp);

    // Slowly resume game
    timePassed = 0;
    while(timePassed < timeForSpeedUp)
    {
      float percentComplete = timePassed / timeForSpeedUp;
      Time.timeScale = percentComplete;

      yield return 0;
      timePassed += Time.unscaledDeltaTime;
    }

    // Cleanup
    Time.timeScale = 1;
    routine = null;
  }
  #endregion
}
