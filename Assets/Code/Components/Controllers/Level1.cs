using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : LevelManager
{
  #region Data
  Animator cloudAnimator;
  #endregion

  #region Init
  protected override void Awake()
  {
    base.Awake();

    cloudAnimator = GameObject.Find("EvilCloud").GetComponent<Animator>();
  }

  protected override void Start()
  {
    base.Start();

    cloudAnimator.Play("Level1Start");
  }
  #endregion

  #region API
  public override void YouWin()
  {
    base.YouWin();

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

    cloudAnimator.Play("Level1End");

    yield return new WaitForSeconds(timeToWaitAtEnd);

    SceneManager.LoadScene("BetweenLevels");
  }
  #endregion
}
