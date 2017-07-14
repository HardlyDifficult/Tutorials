using System.Collections;
using UnityEngine;

/// <summary>
/// Stores object's rotation as defined in the scene or prefab, then sets it to identity.
/// Overtime, the object jitters back to the original rotation.
/// </summary>
public class RotateOvertimeFromIdentityToOriginalRotation : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How long before the rotation effect begins.
  /// </summary>
  [SerializeField]
  float initialWaitTime;

  /// <summary>
  /// How to rotate each jitter (a randomish amount of time).
  /// </summary>
  [SerializeField]
  float rotationTimeFactor;

  /// <summary>
  /// Select a random time 0->this value to sleep between each jiffer.
  /// </summary>
  [SerializeField]
  float maxTimeBetweenRotations = .25f;
  #endregion

  #region Init
  /// <summary>
  /// On start, kick off the rotation animation.
  /// </summary>
  void Start()
  {
    StartCoroutine(AnimateRotation());
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Executes the rotation animation.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator AnimateRotation()
  {
    // Start with no rotation
    Quaternion targetRotation = transform.rotation;
    transform.rotation = Quaternion.identity;
    yield return new WaitForSeconds(initialWaitTime);

    // Jitter towards the original rotation
    float percentComplete = 0;
    float sleepTimeLastFrame = 0;
    while(true)
    {
      // Select a random sleep time
      sleepTimeLastFrame = UnityEngine.Random.Range(0, maxTimeBetweenRotations);
      yield return new WaitForSeconds(sleepTimeLastFrame);

      // Update rotation
      float percentCompleteThisFrame = sleepTimeLastFrame * rotationTimeFactor;
      percentCompleteThisFrame *= UnityEngine.Random.Range(0, 10);
      percentComplete += percentCompleteThisFrame;
      if(percentComplete >= 1)
      {
        transform.rotation = targetRotation;
        yield break;
      }
      transform.rotation = Quaternion.Lerp(Quaternion.identity, targetRotation, percentComplete);
    }
  }
  #endregion
}
