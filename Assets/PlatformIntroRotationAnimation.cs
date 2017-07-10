using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformIntroRotationAnimation : MonoBehaviour
{
  [SerializeField]
  float initialWaitTime;
  [SerializeField]
  float rotationTimeFactor;

  Quaternion targetRotation;

  void Start()
  {
    targetRotation = transform.rotation;
    transform.rotation = Quaternion.identity;
    StartCoroutine(AnimateRotation());
  }

  IEnumerator AnimateRotation()
  {
    yield return new WaitForSeconds(initialWaitTime);
    float percentComplete = 0;
    float sleepTimeLastFrame = 0;
    while(true)
    {
      sleepTimeLastFrame = UnityEngine.Random.Range(0, .25f);
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
}
