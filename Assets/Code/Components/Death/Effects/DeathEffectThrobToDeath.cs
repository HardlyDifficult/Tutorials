using UnityEngine;
using System.Collections;

/// <summary>
/// Causes the object to scale up and down before Destroy.
/// </summary>
public class DeathEffectThrobToDeath : MonoBehaviour, IHaveDeathEffect
{
  #region Data
  /// <summary>
  /// How long to throb before Destroy.
  /// </summary>
  [SerializeField]
  float lengthOfEffectInSeconds = 1;

  /// <summary>
  /// The number of throbs before Destroy.
  /// </summary>
  [SerializeField]
  int numberOfPulses = 5;
  #endregion

  #region API
  /// <summary>
  /// Called when this object should die.  Triggers the throb animation and then Destroy.
  /// </summary>
  void IHaveDeathEffect.Die()
  {
    StartCoroutine(ThrobToDeath());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the throb animation and then Destroys the gameObject.
  /// </summary>
  /// <returns>Used by coroutine to manage time.</returns>
  IEnumerator ThrobToDeath()
  {
    Debug.Assert(lengthOfEffectInSeconds > 0);
    Debug.Assert(numberOfPulses > 0);

    // Throb over time
    float timePerPulse = lengthOfEffectInSeconds / numberOfPulses;
    float timeRun = 0;
    while(timeRun < lengthOfEffectInSeconds)
    {
      float percentComplete = timeRun / lengthOfEffectInSeconds;
      float pulse = (.5f + Mathf.Abs(Mathf.Sin(Mathf.PI * timeRun / timePerPulse)));
      float scale = (1 - percentComplete) * pulse;
      gameObject.transform.localScale = Vector3.one * scale;

      // Wait till the next update
      yield return 0; 
      timeRun += Time.deltaTime;
    }

    // Cleanup
    Destroy(gameObject);
  }
  #endregion
}
