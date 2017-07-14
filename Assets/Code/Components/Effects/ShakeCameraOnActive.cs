using System.Collections;
using UnityEngine;

/// <summary>
/// When activated (e.g. maybe via an animation), the camera shakes (via transform position changes).
/// </summary>
public class ShakeCameraOnActive : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How long to play the shake animation for.
  /// </summary>
  [SerializeField]
  float timeToShakeFor = 1;

  /// <summary>
  /// The max sleep time between camera position movements.  
  /// This scales such that the mid point of the animation shakes fastest.
  /// </summary>
  [SerializeField]
  float maxTimeBetweenShakes = .2f;

  /// <summary>
  /// How much to move the camera around by.
  /// </summary>
  [SerializeField]
  float shakeMagnitude = 1;
  #endregion

  #region Init
  /// <summary>
  /// On start, begine the shake animation.
  /// </summary>
  void Start()
  {
    StartCoroutine(ShakeCamera());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the camera shake animation.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator ShakeCamera()
  {
    Camera camera = Camera.main;
    Vector3 startingPosition = camera.transform.position;

    float timePassed = 0;
    while(timePassed < timeToShakeFor)
    {
      // Shake camera
      float percentComplete = timePassed / timeToShakeFor;
      percentComplete *= 2;
      if(percentComplete > 1)
      {
        percentComplete = 2 - percentComplete;
      }
      camera.transform.position = startingPosition + (Vector3)UnityEngine.Random.insideUnitCircle * shakeMagnitude * percentComplete;

      // Wait for the next frame
      float sleepTime = UnityEngine.Random.Range(0, maxTimeBetweenShakes * (1 - percentComplete));
      yield return new WaitForSeconds(sleepTime);
      sleepTime = Mathf.Max(Time.deltaTime, sleepTime);
      timePassed += sleepTime;
    }

    // Ensure we end at the exact position we started
    camera.transform.position = startingPosition;
  }
  #endregion
}
