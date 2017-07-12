using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections;

namespace HD
{
  public class DeathEffectThrobToDeath : MonoBehaviour, IDie
  {
    [SerializeField]
    float lengthOfEffectInSeconds = 1;
    [SerializeField]
    int numberOfPulses = 5;


    void Awake()
    {
    }

    public void Die()
    {
      StartCoroutine(ThrobToDeath());
    }

    IEnumerator ThrobToDeath()
    {
      // Throb over time
      float timePerPulse = lengthOfEffectInSeconds / numberOfPulses;
      float timeRun = 0;
      while(timeRun < lengthOfEffectInSeconds)
      {
        float percentComplete = timeRun / lengthOfEffectInSeconds;
        float pulse = (.5f + Mathf.Abs(Mathf.Sin(Mathf.PI * timeRun / timePerPulse)));
        float scale = (1 - percentComplete) * pulse;
        gameObject.transform.localScale = Vector3.one * scale;

        yield return 0; // Wait till the next update
        timeRun += Time.deltaTime;
      }

      // Cleanup
      Destroy(gameObject);
    }
  }
}
