using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOvertimeFromIdentityToOriginalRotation : MonoBehaviour
{
  #region Data
  [SerializeField]
  float initialWaitTime;
  [SerializeField]
  float rotationTimeFactor;
  [SerializeField]
  float maxTimeBetweenRotations = .25f;
  #endregion

  #region Init
  void Start()
  {
    StartCoroutine(AnimateRotation());
  }
  #endregion

  #region Private helpers
  IEnumerator AnimateRotation()
  {
    // Start with ro rotation
    Quaternion targetRotation = transform.rotation;
    transform.rotation = Quaternion.identity;
    yield return new WaitForSeconds(initialWaitTime);

    // Jitter towards the original rotation
    float percentComplete = 0;
    float sleepTimeLastFrame = 0;
    while(true)
    {
      sleepTimeLastFrame = UnityEngine.Random.Range(0, maxTimeBetweenRotations);
      yield return new WaitForSeconds(sleepTimeLastFrame);

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
