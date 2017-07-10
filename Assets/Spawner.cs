using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
  #region Data
  [SerializeField]
  GameObject thingToSpawn;
  [SerializeField]
  float enemyImpactMultiple;
  [SerializeField]
  float timeImpactMultiple;
  [SerializeField]
  float initialWaitTime;
  Coroutine routine;
  #endregion

  #region Properties
  public int enemyCount
  {
    get
    {
      return 1 + Bomb.bombCount + FlyGuy.flyGuyCount;
    }
  }
  #endregion

  void Start()
  {
    routine = StartCoroutine(SpawnEnemies());
  }

  IEnumerator SpawnEnemies()
  {
    yield return new WaitForSeconds(initialWaitTime);
    while(true)
    {
      float multiple = 1;
      // Goal: Go slower when there are a lot of bombs on screen already
      multiple *= enemyCount * enemyImpactMultiple;
      // Goal: Go faster later in the level
      multiple /= 1 + Time.timeSinceLevelLoad * Time.timeSinceLevelLoad * timeImpactMultiple;
      yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f) * multiple);
      if(PlayerController.instance != null 
        && (PlayerController.instance.transform.position - transform.position).sqrMagnitude > 3)
      { // Don't spawn if the player is standing right there
        Instantiate(thingToSpawn, transform.position, Quaternion.identity);
      }
    }
  }
}
