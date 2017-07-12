using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TimeController : MonoBehaviour
{
  Coroutine routine;

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
}
