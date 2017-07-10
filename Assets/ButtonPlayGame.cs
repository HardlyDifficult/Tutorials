using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPlayGame : MonoBehaviour
{
  public void OnClick()
  {
    SceneManager.LoadScene("Level1");
  }
}
