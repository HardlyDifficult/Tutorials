using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A base for each specific level manager.  Every level should have a level manager object in the scene.
/// </summary>
public abstract class LevelManager : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How long to sleep after a win was reported before changing scenes.
  /// </summary>
  [SerializeField]
  protected float timeToWaitAtEnd = 3;

  /// <summary>
  /// Stores the player prefab.
  /// Loaded from the Resources directory / "Player".
  /// </summary>
  Player playerPrefab;

  /// <summary>
  /// True if the game has ended (may be in a final animation state before the scene closes).
  /// </summary>
  protected bool isGameOver;
  #endregion

  #region Init
  /// <summary>
  /// On awake, load the player prefab.
  /// </summary>
  protected virtual void Awake()
  {
    playerPrefab = Resources.Load<Player>("Player");
  }

  /// <summary>
  /// When the level starts, spawn a player.  The level subclass may implement more.
  /// </summary>
  protected virtual void Start()
  {
    Instantiate(playerPrefab);
  }
  #endregion

  #region Public API
  /// <summary>
  /// Report that the player has won the game.
  /// This method just jots a note, the level subclass should implement more.
  /// </summary>
  public virtual void YouWin()
  {
    if(isGameOver == true)
    { // You already won
      return;
    }

    isGameOver = true;
  }

  /// <summary>
  /// Report that the player has just died.  This may indicate the end of the game as well.
  /// If lives remain, notify other interested objects (via ICareWhenPlayerDies).
  /// If out of lives, start the end sequence.
  /// </summary>
  public void YouDied()
  {
    GameController.instance.lifeCounter--;
    if(GameController.instance.lifeCounter >= 0)
    { // Report the death to other interested objects
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
      // Spawn another player
      Instantiate(playerPrefab);
    }
    else
    { // Start end sequence
      isGameOver = true;
      StartCoroutine(PlayEndingYouLose());
    }
  }
  #endregion

  #region Helpers
  /// <summary>
  /// The you lost sequence: A brief pause followed by a scene change.
  /// </summary>
  IEnumerator PlayEndingYouLose()
  {
    yield return new WaitForSeconds(1);
    SceneManager.LoadScene("YouLose");
  }
  #endregion
}
