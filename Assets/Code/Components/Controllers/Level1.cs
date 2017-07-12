using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : LevelManager
{
  #region Init
  void Start()
  {
    SpawnPlayer();
  }
  #endregion

  #region API
  public override void YouWin()
  {
    StartCoroutine(PlayEndingYouWin());
  }
  #endregion

  #region Private helpers
  IEnumerator PlayEndingYouWin()
  {
    Spawner.StopAll();

    // Disable player movement and controls
    Player.instance.myBody.constraints = RigidbodyConstraints2D.FreezeAll;
    Player.instance.enabled = false;

    GameObject cloud = GameObject.Find("EvilCloud");
    cloud.GetComponent<Animator>().Play("EvilCloudEnd");

    yield return new WaitForSeconds(timeToWaitAtEnd);

    SceneManager.LoadScene("BetweenLevels");
  }
  #endregion
}
