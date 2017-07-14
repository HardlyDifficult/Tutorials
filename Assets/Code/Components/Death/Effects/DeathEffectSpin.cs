using System.Collections;
using UnityEngine;

/// <summary>
/// Spins at an accelerating rate until the object is Destroyed.
/// </summary>
public class DeathEffectSpin : MonoBehaviour, IHaveDeathEffect
{
  #region Data
  /// <summary>
  /// How long to spin before dieing.
  /// </summary>
  [SerializeField]
  float timeTillDeath = 3;

  /// <summary>
  /// How fast to start the spin (speeds up exponentially over-time)
  /// </summary>
  [SerializeField]
  float rotationSpeed = 200;
  #endregion

  #region API
  /// <summary>
  /// Called when the object should die.  Begins the spin effect before Destroying the object.
  /// </summary>
  void IHaveDeathEffect.Die()
  {
    StartCoroutine(SpinToDeath());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the spin animation until it's time to die.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator SpinToDeath()
  {
    float currentRotation = 0;
    float timeRun = 0;
    while(timeRun < timeTillDeath)
    { // Spin overtime
      currentRotation = rotationSpeed * timeRun * timeRun; // Scale up spin speed exponetially over-time
      transform.rotation = Quaternion.Euler(0, 0, currentRotation);

      // Wait for the next Update
      yield return 0;
      timeRun += Time.deltaTime;
    }

    // Destroy object
    Destroy(gameObject);
  }
  #endregion
}
