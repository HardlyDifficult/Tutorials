using System.Collections;
using UnityEngine;

/// <summary>
/// Instantiates an entity at this gameObject's location periodically.
/// </summary>
public class Spawner : MonoBehaviour, ICareWhenPlayerDies
{
  #region Data
  /// <summary>
  /// Set to the prefab to instantiate.
  /// </summary>
  [SerializeField]
  GameObject thingToSpawn;

  /// <summary>
  /// How long to wait before spawning begins.
  /// </summary>
  [SerializeField]
  float initialWaitTime = 2;

  /// <summary>
  /// The least amount of time between each entity spawn.
  /// </summary>
  [SerializeField]
  float minTimeBetweenSpawns = .5f;

  /// <summary>
  /// The most amount of time between each entity spawn.
  /// </summary>
  [SerializeField]
  float maxTimeBetweenSpawns = 10;

  /// <summary>
  /// A weight for how much the number of active enemies in the world slow the spawn rate.
  /// </summary>
  [SerializeField]
  float enemyImpactMultiple = 30;

  /// <summary>
  /// A weight for how much the amount of time you have been in the level increases the spawn rate.
  /// </summary>
  [SerializeField]
  float timeImpactMultiple = .02f;
  #endregion

  #region Properties
  /// <summary>
  /// The total number of active enemies in the world (sum of bombs and flyguys).
  /// </summary>
  public int enemyCount
  {
    get
    {
      return Bomb.bombCount + FlyGuy.flyGuyCount;
    }
  }
  #endregion

  #region Init
  /// <summary>
  /// On start, begin the spawn coroutine.
  /// </summary>
  protected void Start()
  {
    StartCoroutine(SpawnEnemies());
  }
  #endregion

  #region Events
  /// <summary>
  /// When the player dies, stop the current spawner and then restart it.  
  /// This causes the settings to reset - e.g. any wait time at the start is executed again.
  /// </summary>
  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    StopAllCoroutines();
    StartCoroutine(SpawnEnemies());
  }
  #endregion

  #region Public API
  /// <summary>
  /// Stops all spawners in the world.
  /// Uses destroy, there is no resuming without reloading the scene.
  /// </summary>
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
  /// <summary>
  /// Executes the spawn routine, periodically spawning a new enemy entity.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator SpawnEnemies()
  {
    Debug.Assert(thingToSpawn != null);
    Debug.Assert(initialWaitTime > 0);
    Debug.Assert(minTimeBetweenSpawns >= 0);
    Debug.Assert(maxTimeBetweenSpawns > 0);
    Debug.Assert(maxTimeBetweenSpawns > minTimeBetweenSpawns);

    // Wait before first spawn
    yield return new WaitForSeconds(initialWaitTime);

    while(true)
    {
      if(Player.instance != null
        && (Player.instance.transform.position - transform.position).sqrMagnitude > 3)
      { // Don't spawn if the player is standing right there
        Instantiate(thingToSpawn, transform.position, Quaternion.identity);
      }

      // Sleep a random amount of time before the next spawn
      float multiple = 1;
      // Goal: Go slower when there are a lot of bombs on screen already
      multiple += enemyCount * enemyImpactMultiple;
      // Goal: Go faster later in the level
      multiple -= Time.timeSinceLevelLoad * Time.timeSinceLevelLoad * timeImpactMultiple;
      float sleepTime = UnityEngine.Random.value * multiple;
      sleepTime = Mathf.Clamp(sleepTime, minTimeBetweenSpawns, maxTimeBetweenSpawns);
      yield return new WaitForSeconds(sleepTime);
    }
  }
  #endregion
}
