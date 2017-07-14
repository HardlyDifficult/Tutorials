using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level 2, namely the intro/outro sequence.
/// </summary>
public class Level2 : LevelManager
{
  #region Data
  /// <summary>
  /// The evil cloud on this level, used to trigger a death effect in the end sequence.
  /// </summary>
  GameObject cloud;

  /// <summary>
  /// The evilCloud's animator, used to play intro and outro animations.
  /// </summary>
  Animator cloudAnimator;
  #endregion

  #region Init
  /// <summary>
  /// On awake, register for event on breakable blocks breaking; and populate variables.
  /// </summary>
  protected override void Awake()
  {
    base.Awake();

    cloud = GameObject.Find("EvilCloud");
    Debug.Assert(cloud != null);
    cloudAnimator = cloud.GetComponentInChildren<Animator>();
  }

  /// <summary>
  /// When the level starts, play the evilCloud's intro animation.
  /// </summary>
  protected override void Start()
  {
    base.Start();

    cloudAnimator.Play("Level2Start");
  }
  #endregion
  
  #region API
  /// <summary>
  /// Called by the mushroom (touch to win the game).
  /// Ensure's that all the breakable blocks have broken first as-well.
  /// If win is legit, begins end sequence.
  /// </summary>
  public override void YouWin()
  {
    if(BreakawayOnTrigger.numberOfInTactBlocks > 0
      || TouchMeToWin.numberActive > 0)
    { // You didn't really win yet
      return;
    }
    if(isGameOver)
    { // Already over (just in case this was called twice)
      return;
    }

    // Begin end sequence
    base.YouWin();
    StartCoroutine(PlayWin());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the end sequence: Triggering a death effect and then loading the next scene.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator PlayWin()
  {
    yield return new WaitForSeconds(1);
    cloud.CallIDie();
    yield return new WaitForSeconds(timeToWaitAtEnd);
    SceneManager.LoadScene("YouWin");
  }
  #endregion
}
