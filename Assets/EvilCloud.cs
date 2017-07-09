using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilCloud : MonoBehaviour
{
  public GameObject bombToSpawn;
  public float bombImpactMultiple;
  public float timeImpactMultiple, min, max;

  Coroutine routine;

  void Start()
  {
    routine = StartCoroutine(PoopBombs());
  }

  IEnumerator PoopBombs()
  {
   while(true)
    {
      float multiple = 1;
      // Goal: Go slower when there are a lot of bombs on screen already
      multiple *= (Bomb.bombCount + 1) * bombImpactMultiple;
      // Goal: Go faster later in the level
      multiple /= 1 + Time.timeSinceLevelLoad * Time.timeSinceLevelLoad * timeImpactMultiple;
      print(multiple);
      multiple = Mathf.Max(min, Mathf.Min(max, multiple));
      yield return new WaitForSeconds(UnityEngine.Random.Range(1, 5) * multiple);
      Instantiate(bombToSpawn, transform.position, Quaternion.identity);
    }
  }
}
