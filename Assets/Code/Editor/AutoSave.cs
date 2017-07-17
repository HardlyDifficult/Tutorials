using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Auto saves the scene and project everytime you click play.
/// 
/// This happens before the game runs so even if that run causes 
/// a crash, your work is safe.
/// </summary>
[InitializeOnLoad]
public class AutoSave
{
  /// <summary>
  /// Called automatically c/o InitializeOnLoad.  Registers for
  /// play mode events.
  /// </summary>
  static AutoSave()
  {
    EditorApplication.playmodeStateChanged 
      += OnPlaymodeStateChanged;
  }

  /// <summary>
  /// When the play mode changes, consider saving.
  /// </summary>
  static void OnPlaymodeStateChanged()
  {
    if(EditorApplication.isPlaying)
    { // If currently playing, don't save
      return;
    }

    // Save!  
    EditorSceneManager.SaveOpenScenes();
  }
}
