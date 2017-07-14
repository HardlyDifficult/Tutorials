using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sets this gameObject to DontDestroyOnLoad and then detects a scene change 
/// in order to cause a death effect which survives the scene.
/// </summary>
public class SuicidePostSceneChange : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Indicates a death is in progress.  This is used to detect multiple rapid scene changes.
  /// </summary>
  bool isDieing;
  #endregion

  #region Init
  /// <summary>
  /// On awake, set this object to DontDestroyOnLoad and register for scene change events.
  /// </summary>
  void Awake()
  {
    DontDestroyOnLoad(gameObject);
    SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
  }

  /// <summary>
  /// Deregister events.
  /// </summary>
  void OnDestroy()
  {
    SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
  }
  #endregion

  #region Events
  /// <summary>
  /// Called when the scene is changing.
  /// </summary>
  /// <param name="scene">The scene which is closing.</param>
  void SceneManager_sceneUnloaded(
    Scene scene)
  {
    if(isDieing)
    { // Second scene change since I started, let's bail
      Destroy(gameObject);
      return;
    }

    // Start death effects and then destroy object
    isDieing = true;
    gameObject.CallIDie();
  }
  #endregion
}
