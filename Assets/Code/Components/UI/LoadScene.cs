using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

  public class LoadScene : MonoBehaviour
  {
    public void LoadLevel(
      int levelNumber)
    {
      SceneManager.LoadScene("Level" + levelNumber);
    }

    public void LoadMenu()
    {
      SceneManager.LoadScene("MainMenu");
    }
  }
