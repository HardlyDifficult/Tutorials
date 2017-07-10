using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  #region Data
  public static GameController instance;
  [NonSerialized]
  public Bounds screenBounds;
  public int lifeCounter;
  [SerializeField]
  float timeToWaitAtEnd;
  public int points;
  bool isLevelOver;
  [SerializeField]
  PlayerController playerPrefab;
  #endregion

  #region Init
  void Awake()
  {
    if(instance != null)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
    instance = this;
    PlayerController.onDeath += Player_onDeath;

    Vector2 screenSize = new Vector2((float)Screen.width / Screen.height, 1) * Camera.main.orthographicSize * 2;
    screenBounds = new Bounds(Vector2.zero, screenSize);

    SceneManager.sceneLoaded += SceneManager_sceneLoaded;
  }

  void SceneManager_sceneLoaded(
    Scene sceneLoaded, 
    LoadSceneMode arg1)
  {
    if(sceneLoaded.name == "Level1")
    {
      SpawnPlayer();
    }
  }

  void Cleanup()
  {
    PlayerController.onDeath -= Player_onDeath;
  }

  void OnApplicationQuit()
  {
    isLevelOver = true;
  }
  #endregion

  #region API
  public void YouWin()
  {
    isLevelOver = true;

    StartCoroutine(PlayEnding());
  }
  #endregion

  #region Private Helpers
  IEnumerator PlayEnding()
  {
    Spawner[] spawnerList = GameObject.FindObjectsOfType<Spawner>();
    for(int i = 0; i < spawnerList.Length; i++)
    {
      Spawner spawner = spawnerList[i];
      Destroy(spawner);
    }

    PlayerController.instance.myBody.constraints = RigidbodyConstraints2D.FreezeAll;
    PlayerController.instance.enabled = false; // Disable player controls
    GameObject cloud = GameObject.Find("EvilCloud");
    cloud.GetComponent<Animator>().Play("EvilCloudEnd");
    yield return new WaitForSeconds(timeToWaitAtEnd);
    SceneManager.LoadScene("BetweenLevels");
  }

  void Player_onDeath()
  {
    if(isLevelOver)
    {
      return;
    }
    lifeCounter--;
    if(lifeCounter >= 0)
    {
      SpawnPlayer();
    }
    else
    {
      isLevelOver = true;
      StartCoroutine(EndOfGame());
    }
  }

  void SpawnPlayer()
  {
    PlayerController player = Instantiate(playerPrefab);

    Collider2D[] hitList = Physics2D.OverlapCircleAll(player.transform.position, 1, LayerMask.GetMask(new[] { "Enemy" }));
    for(int i = 0; i < hitList.Length; i++)
    {
      Collider2D hit = hitList[i];
      Destroy(hit.gameObject);
    }
  }

  IEnumerator EndOfGame()
  {
    yield return new WaitForSeconds(1);
    SceneManager.LoadScene("YouLose");
  }
  #endregion
}
