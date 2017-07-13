using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2 : LevelManager
{
  #region Data
  GameObject cloud;
  Animator cloudAnimator;
  #endregion

  #region Init
  protected override void Awake()
  {
    base.Awake();

    BreakawayOnTrigger.onBreakaway += BreakawayOnTrigger_onBreakaway;
    cloud = GameObject.Find("EvilCloud");
    cloudAnimator = cloud.GetComponentInChildren<Animator>();
  }

  protected override void Start()
  {
    base.Start();

    cloudAnimator.Play("Level2Start");
  }

  protected void OnDestroy()
  {
    BreakawayOnTrigger.onBreakaway -= BreakawayOnTrigger_onBreakaway;
  }
  #endregion

  void BreakawayOnTrigger_onBreakaway()
  {
    if(BreakawayOnTrigger.numberOfInTactBlocks == 0)
    {
      YouWin();
    }
  }

  public override void YouWin()
  {
    if(BreakawayOnTrigger.numberOfInTactBlocks > 0)
    { // You didn't really win yet
     return;
    }

    base.YouWin();

    StartCoroutine(PlayWin());
  }

  IEnumerator PlayWin()
  {
    yield return new WaitForSeconds(1);
    cloud.GetComponent<IDie>().Die();
    yield return new WaitForSeconds(3);
    SceneManager.LoadScene("YouWin");
  }
}
