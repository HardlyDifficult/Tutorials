using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinArea : MonoBehaviour
{
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(collision.gameObject.CompareTag("Player"))
    {
      GameObject.FindObjectOfType<LevelManager>().YouWin();
    }
  }
}
