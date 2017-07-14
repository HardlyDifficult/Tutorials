using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the first level - the intro and outro sequences.
/// </summary>
public class Level1 : LevelManager
{
  #region Data
  /// <summary>
  /// The evilCloud's animator, used to play intro and outro animations.
  /// </summary>
  Animator cloudAnimator;
  #endregion

  #region Init
  /// <summary>
  /// On awake, cache the animator.
  /// </summary>
  protected override void Awake()
  {
    base.Awake();

    cloudAnimator = GameObject.Find("EvilCloud").GetComponent<Animator>();

    Debug.Assert(cloudAnimator != null);
  }

  /// <summary>
  /// When the level starts, play the cloud's intro animation.
  /// </summary>
  protected override void Start()
  {
    base.Start();

    cloudAnimator.Play("Level1Start");
  }
  #endregion

  #region API
  /// <summary>
  /// When you win, play an outro animation and then load the next scene.
  /// </summary>
  public override void YouWin()
  {
    base.YouWin();

    StartCoroutine(PlayEndingYouWin());
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Executes the outro sequence and then loads the next scene.
  /// </summary>
  /// <returns>Used by coroutine to manage time.</returns>
  IEnumerator PlayEndingYouWin()
  {
    Spawner.StopAll();

    // Disable player movement and controls
    Player.instance.myBody.constraints = RigidbodyConstraints2D.FreezeAll;
    Player.instance.enabled = false;

    // Play evilCloud's outro animation
    cloudAnimator.Play("Level1End");

    // Wait for the animation/effects to complete
    yield return new WaitForSeconds(timeToWaitAtEnd);

    // Load the next scene
    SceneManager.LoadScene("BetweenLevels");
  }
  #endregion
}
