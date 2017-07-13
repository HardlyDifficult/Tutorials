using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelManager : MonoBehaviour
{
  #region Data
  public event Action onWin;
  [SerializeField]
  protected float timeToWaitAtEnd = 3;
  Player playerPrefab;
  protected bool isGameOver;
  #endregion

  #region Init
  protected virtual void Awake()
  {
    playerPrefab = Resources.Load<Player>("Character");
  }

  protected virtual void Start()
  {
    SpawnPlayer();
  }
  #endregion

  #region Public API
  public virtual void YouWin()
  {
    if(isGameOver == true)
    { // You already won
      return;
    }

    isGameOver = true;
    if(onWin != null)
    {
      onWin();
    }
  }

  public void YouDied()
  {
    GameController.instance.lifeCounter--;
    if(GameController.instance.lifeCounter >= 0)
    {
      GameObject[] gameObjectList = GameObject.FindObjectsOfType<GameObject>();
      for(int i = 0; i < gameObjectList.Length; i++)
      {
        ICareWhenPlayerDies[] careList = gameObjectList[i].GetComponents<ICareWhenPlayerDies>();
        for(int j = 0; j < careList.Length; j++)
        {
          ICareWhenPlayerDies care = careList[j];
          care.OnPlayerDeath();
        }
      }
      SpawnPlayer();
    }
    else
    {
      isGameOver = true;
      StartCoroutine(PlayEndingYouLose());
    }
  }
  #endregion

  #region Helpers
  protected void SpawnPlayer()
  {
    Instantiate(playerPrefab);
  }

  IEnumerator PlayEndingYouLose()
  {
    yield return new WaitForSeconds(1);
    SceneManager.LoadScene("YouLose");
  }

  #endregion
}
