using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ICareWhenPlayerDies
{
  #region Data
  [SerializeField]
  GameObject thingToSpawn;

  [SerializeField]
  float initialWaitTime;
  [SerializeField]
  float minTimeBetweenSpawns;
  [SerializeField]
  float maxTimeBetweenSpawns;

  [SerializeField]
  float enemyImpactMultiple;
  [SerializeField]
  float timeImpactMultiple;

  Coroutine routine;
  #endregion

  #region Properties
  public int enemyCount
  {
    get
    {
      return Bomb.bombCount + FlyGuy.flyGuyCount;
    }
  }
  #endregion

  #region Init
  void Start()
  {
    routine = StartCoroutine(SpawnEnemies());
  }
  #endregion

  #region Events
  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    StopCoroutine(routine);
    routine = StartCoroutine(SpawnEnemies());
  }
  #endregion

  #region Public API
  public static void StopAll()
  {
    Spawner[] spawnerList = GameObject.FindObjectsOfType<Spawner>();
    for(int i = 0; i < spawnerList.Length; i++)
    {
      Spawner spawner = spawnerList[i];
      Destroy(spawner);
    }
  }
  #endregion

  #region Private helpers
  IEnumerator SpawnEnemies()
  {
    yield return new WaitForSeconds(initialWaitTime);

    while(true)
    {
      float multiple = 1;
      // Goal: Go slower when there are a lot of bombs on screen already
      multiple += enemyCount * enemyImpactMultiple;
      // Goal: Go faster later in the level
      multiple -= Time.timeSinceLevelLoad * Time.timeSinceLevelLoad * timeImpactMultiple;

      float sleepTime = UnityEngine.Random.value * multiple;
      sleepTime = Mathf.Clamp(sleepTime, minTimeBetweenSpawns, maxTimeBetweenSpawns);

      yield return new WaitForSeconds(sleepTime);

      if(Player.instance != null 
        && (Player.instance.transform.position - transform.position).sqrMagnitude > 3)
      { // Don't spawn if the player is standing right there
        Instantiate(thingToSpawn, transform.position, Quaternion.identity);
      }
    }
  }
  #endregion
}
