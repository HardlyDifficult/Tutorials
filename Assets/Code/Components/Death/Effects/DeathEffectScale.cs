using System.Collections;
using UnityEngine;

/// <summary>
/// Scales the object for a period of time before destroying it.
/// </summary>
public class DeathEffectScale : MonoBehaviour, IHaveDeathEffect
{
  #region Data
  /// <summary>
  /// How long before it scales to nothing and Destroys.
  /// </summary>
  [SerializeField]
  float timeTillDeath = 3; 
  #endregion

  #region API
  /// <summary>
  /// Called when it's time to start dieing.
  /// </summary>
  void IHaveDeathEffect.Die()
  {
    StartCoroutine(ScaleToDeath());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes scale to death effect, destroying the gameObject when complete.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator ScaleToDeath()
  {
    Debug.Assert(timeTillDeath > 0);

    // Cache the original scale to manipulate in the loop
    Vector3 originalScale = transform.localScale;

    float timeRun = 0;
    while(timeRun < timeTillDeath)
    { // Slowly scale down
      float percentComplete = timeRun / timeTillDeath;
      float scaleFactor = (1 - percentComplete);
      scaleFactor *= scaleFactor;
      transform.localScale = originalScale * scaleFactor;

      // Wait till the next Update event
      yield return 0;
      timeRun += Time.deltaTime;
    }

    // Destroy object
    Destroy(gameObject);
  }
  #endregion
}
