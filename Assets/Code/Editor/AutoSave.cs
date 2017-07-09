using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HD
{
  /// <summary>
  /// Auto saves the scene and project everytime you click play.
  /// </summary>
  [InitializeOnLoad]
  public class AutoSave
  {

    static AutoSave()
    {
      EditorApplication.playmodeStateChanged = AutoSaveOsStateChanged;
    }

    static void AutoSaveOsStateChanged()
    {
      if(EditorApplication.isPlaying)
      {
        return;
      }

      EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
      AssetDatabase.SaveAssets();
    }
  }
}