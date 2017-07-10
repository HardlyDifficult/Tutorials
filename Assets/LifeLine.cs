using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLine : MonoBehaviour {
  [SerializeField]
  float lifeCount;

	void Start () {
    PlayerController.onDeath += RefreshVisibility;
	}

  void OnDestroy()
  {
    PlayerController.onDeath -= RefreshVisibility;
  }

  void RefreshVisibility()
  {
    if(GameController.instance.lifeCounter < lifeCount)
    {
      StartCoroutine(ThrobToDeath());
    }
  }

  IEnumerator ThrobToDeath()
  {
    int frameCount = 100;
    int numberOfPulses = 5;
    int numberOfFramesPerPulse = frameCount / numberOfPulses;

    for(int i = 0; i < frameCount; i++)
    {
      gameObject.transform.localScale = (((float)frameCount - i) / frameCount) *  Vector3.one * (.5f + Mathf.Abs(Mathf.Sin((float)i / numberOfFramesPerPulse * Mathf.PI)));
      yield return 0;
    }
    Destroy(gameObject);
  }
}
