using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Exposes public methods to change scenes.
/// </summary>
/// <remarks>This is placed on the canvas and used by UI (e.g. buttons) on press events.</remarks>
public class LoadScene : MonoBehaviour
{
  #region API
  /// <summary>
  /// Change the scene to a specific level.
  /// </summary>
  /// <param name="levelNumber">1 or 2</param>
  public void LoadLevel(
    int levelNumber)
  {
    Debug.Assert(levelNumber == 1 || levelNumber == 2);

    SceneManager.LoadScene("Level" + levelNumber);
  }

  /// <summary>
  /// Change the scene to the main menu.
  /// </summary>
  public void LoadMenu()
  {
    SceneManager.LoadScene("MainMenu");
  }
  #endregion
}
