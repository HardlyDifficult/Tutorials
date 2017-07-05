using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinArea : MonoBehaviour
{
  void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.gameObject.CompareTag("Player"))
    {
      StartCoroutine(PlayEnding());
    }
  }

  IEnumerator PlayEnding()
  {
    // TODO start animation
    yield return new WaitForSeconds(2);
    SceneManager.LoadScene("BetweenLevels");
  }
}
