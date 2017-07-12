using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelManager : MonoBehaviour
{
  #region Data
  [SerializeField]
  protected float timeToWaitAtEnd = 3;
  [SerializeField]
  Player playerPrefab;
  #endregion
  
  #region Public API
  public abstract void YouWin();

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
