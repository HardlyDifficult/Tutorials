using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  #region Data
  public static GameController instance;

  [NonSerialized]
  public TimeController timeController;

  public int lifeCounter = 3;
  public int points;
  [NonSerialized]
  public Bounds screenBounds;
  #endregion

  #region Init
  void Awake()
  {
    if(instance != null)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
    instance = this;

    timeController = GetComponent<TimeController>();
    Vector2 screenSize = new Vector2((float)Screen.width / Screen.height, 1) * Camera.main.orthographicSize * 2;
    screenBounds = new Bounds(Vector2.zero, screenSize);
  }
  #endregion
}
