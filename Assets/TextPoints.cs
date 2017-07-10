using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TextPoints : MonoBehaviour
{
  [SerializeField]
  float scrollSpeedFactor;
  TextMesh text;
  int previousPoints;

  void Start()
  {
    text = GetComponent<TextMesh>();
    Debug.Assert(text != null);
  }

  void Update()
  {
    int currentPoints = GameController.instance.points;
    if(previousPoints == currentPoints)
    {
      return;
    }

    int deltaPoints = currentPoints - previousPoints;
    int pointsToDisplay = previousPoints + Math.Max(1, (int)(deltaPoints * scrollSpeedFactor * Time.deltaTime));
    text.text = pointsToDisplay.ToStringWithCommas();
    previousPoints = pointsToDisplay;
  }
}
