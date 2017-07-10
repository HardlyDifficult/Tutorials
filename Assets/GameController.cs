using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

  public class GameController : MonoBehaviour
  {
    public static Bounds screenBounds;

    void Start()
    {
      Vector2 screenSize = new Vector2((float)Screen.width / Screen.height, 1) * Camera.main.orthographicSize * 2;
      screenBounds = new Bounds(Vector2.zero, screenSize);
    }
  }
