using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearInSeconds : MonoBehaviour
{
  public Action onComplete;

  [SerializeField]
  float timeBeforeSpawn;
  [SerializeField]
  int framesTillVisible;

  void Start()
  {
    StartCoroutine(Spawn());
  }

  IEnumerator Spawn()
  {
    // Note setting SetActive to false prevents the WaitForSeconds from returning
    SpriteRenderer[] spriteList = gameObject.GetComponentsInChildren<SpriteRenderer>();
    SetAlpha(spriteList, 0);
    yield return new WaitForSeconds(timeBeforeSpawn);

    for(int i = 0; i < framesTillVisible; i++)
    {
      float percentComplete = (float)i / framesTillVisible;
      SetAlpha(spriteList, percentComplete);
      yield return 0;
    }
    SetAlpha(spriteList, 1);
    if(onComplete != null)
    {
      onComplete.Invoke();
    }
  }

  private static void SetAlpha(SpriteRenderer[] spriteList, float alpha)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      Color originalColor = sprite.color;
      sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
  }
}
