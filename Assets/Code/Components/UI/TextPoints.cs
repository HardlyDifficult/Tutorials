using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TextPoints : MonoBehaviour
{
  [SerializeField]
  float scrollSpeedFactor = 5;
  Text text;
  int previousPoints = -1; // -1 causes points to be dirty on the first frame

  void Start()
  {
    text = GetComponent<Text>();
    Debug.Assert(text != null);
  }

  void Update()
  {
    int currentPoints = GameController.instance.points;
    if(previousPoints == currentPoints)
    {
      return;
    }

    // Scroll points
    int deltaPoints = currentPoints - previousPoints;
    int pointsToDisplay = previousPoints + Math.Max(1, (int)(deltaPoints * scrollSpeedFactor * Time.deltaTime));
    previousPoints = pointsToDisplay;

    text.text = pointsToDisplay.ToStringWithCommas();
  }
}
