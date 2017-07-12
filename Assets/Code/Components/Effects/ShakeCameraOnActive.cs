using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraOnActive : MonoBehaviour
{
  [SerializeField]
  float timeToShakeFor = 2;
  [SerializeField]
  float maxTimeBetweenShakes = 1;
  [SerializeField]
  float shakeMagnitude = .1f;

  void Start()
  {
    StartCoroutine(ShakeCamera());
  }

  IEnumerator ShakeCamera()
  {
    Camera camera = Camera.main;
    Vector3 startingPosition = camera.transform.position;

    float timePassed = 0;
    while(timePassed < timeToShakeFor)
    {
      float percentComplete = timePassed / timeToShakeFor;
      percentComplete *= 2;
      if(percentComplete > 1)
      {
        percentComplete = 2 - percentComplete;
      }
      camera.transform.position = startingPosition + (Vector3)UnityEngine.Random.insideUnitCircle * shakeMagnitude * percentComplete;

      float sleepTime = UnityEngine.Random.Range(0, maxTimeBetweenShakes * (1 - percentComplete));
      yield return new WaitForSeconds(sleepTime);
      sleepTime = Mathf.Max(Time.deltaTime, sleepTime);
      timePassed += sleepTime;
    }
  }
}
