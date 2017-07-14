using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks information which persists between scenes (i.e. life/points).
/// 
/// Singleton: This should appear in every scene, and only the first will survive.
/// </summary>
[RequireComponent(typeof(TimeController))]
public class GameController : MonoBehaviour
{
  #region Data
  /// <summary>
  /// A convenient way to access this singleton.
  /// </summary>
  /// <remarks>Optional, could have used GameObject.FindObjectByType instead.</remarks>
  public static GameController instance;

  /// <summary>
  /// Subcontroller for time manipulation.
  /// </summary>
  [NonSerialized]
  public TimeController timeController;

  /// <summary>
  /// Player's starting or remaining life.  Set default in the inspector.
  /// Resets to default value at the main menu.
  /// </summary>
  public int lifeCounter = 3;

  /// <summary>
  /// Stores the original lifeCounter value, allowing us to reset to the between games.
  /// </summary>
  int originalLifeCount;

  /// <summary>
  /// Player's total points so far.
  /// Resets at the main menu.
  /// </summary>
  [NonSerialized]
  public int points;

  /// <summary>
  /// The visible area of the world.  Used to react to things being at the edge of the screen.
  /// </summary>
  public Bounds screenBounds
  {
    get; private set;
  }
  #endregion

  #region Init
  /// <summary>
  /// On awake, destroy gameobject if this is the second game controller (guaranteeing one).
  /// Populate variables and subscribe for scene change events.
  /// </summary>
  protected void Awake()
  {
    if(instance != null)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
    instance = this;

    originalLifeCount = lifeCounter;
    timeController = GetComponent<TimeController>();
    Vector2 screenSize = new Vector2((float)Screen.width / Screen.height, 1) * Camera.main.orthographicSize * 2;
    screenBounds = new Bounds(Vector2.zero, screenSize);

    Reset();

    SceneManager.sceneLoaded += SceneManager_sceneLoaded;
  }
  #endregion

  #region Events
  /// <summary>
  /// On scene change, consider reset game data (i.e. points/life).
  /// </summary>
  /// <param name="scene">The scene being loaded ATM.</param>
  /// <param name="loadMode">ignored</param>
  void SceneManager_sceneLoaded(
    Scene scene, 
    LoadSceneMode loadMode)
  {
    if(scene.name == "MainMenu")
    {
      Reset();
    }
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Resets life and points in preparation for a new game.
  /// </summary>
  void Reset()
  {
    lifeCounter = originalLifeCount;
    points = 0;
  }
  #endregion
}
