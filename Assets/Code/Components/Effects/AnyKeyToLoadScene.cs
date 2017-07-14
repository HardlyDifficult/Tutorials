using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads the given scene when any key (keyboard, mouse, or joystick) is pressed.
/// </summary>
public class AnyKeyToLoadScene : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The name of the scene to load.
  /// </summary>
  [SerializeField]
  string sceneName;
  #endregion

  #region Events
  /// <summary>
  /// On update, load the next scene if a key was pressed.
  /// </summary>
  protected void Update()
  {
    if(Input.anyKey)
    {
      SceneManager.LoadScene(sceneName);
    }
  }
  #endregion
}
