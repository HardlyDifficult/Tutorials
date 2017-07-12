using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DieOverTimeWithSpriteFlash : MonoBehaviour
{
  [SerializeField]
  float lengthBeforeFlash;
  [SerializeField]
  float lengthToFlashFor;
  [SerializeField]
  float timePerColorChange;
  [SerializeField]
  float colorChangeTimeFactorPerFlash;

  void Start()
  {
    StartCoroutine(FlashToDeath());
  }

  IEnumerator FlashToDeath()
  {
    // Wait before starting flash
    yield return new WaitForSeconds(lengthBeforeFlash);

    // Flash over time
    SpriteRenderer[] spriteList = GetComponentsInChildren<SpriteRenderer>();
    float timePassed = 0;
    bool isRed = false;
    while(timePassed < lengthToFlashFor)
    {
      spriteList.SetColor(isRed ? Color.red : Color.white);
      isRed = !isRed;

      yield return new WaitForSeconds(timePerColorChange);
      timePerColorChange = Mathf.Max(Time.deltaTime, timePerColorChange);
      timePassed += timePerColorChange;
      timePerColorChange *= colorChangeTimeFactorPerFlash;
    }

    // Cleanup
    Destroy(gameObject);
  }
}
