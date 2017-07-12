using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearInSecondsAndFadeInSprite : MonoBehaviour
{
  public Action onComplete;

  [SerializeField]
  float timeBeforeSpawn;
  [SerializeField]
  float lengthTillVisible;

  void Awake()
  {
    StartCoroutine(Spawn());
  }

  IEnumerator Spawn()
  {
    // Hide sprites
    SpriteRenderer[] spriteList = gameObject.GetComponentsInChildren<SpriteRenderer>();
    spriteList.SetAlpha(0);
    yield return new WaitForSeconds(timeBeforeSpawn);

    // Fade them back
    float timePassed = 0;
    while(timePassed < lengthTillVisible)
    {
      float percentComplete = timePassed / lengthTillVisible;
      spriteList.SetAlpha(percentComplete);

      yield return 0;
      timePassed += Time.deltaTime;
    }

    // Clean up
    spriteList.SetAlpha(1);
    if(onComplete != null)
    {
      onComplete.Invoke();
    }
  }

  public static void DisableMeTillComplete(
    MonoBehaviour script)
  {
    AppearInSecondsAndFadeInSprite appearIn = script.GetComponent<AppearInSecondsAndFadeInSprite>();
    if(appearIn != null)
    {
      script.enabled = false;
      appearIn.onComplete += () => script.enabled = true;
    }
  }
}
