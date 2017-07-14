using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Added to UI text to display the current number of points.
/// This will attempt to scroll points up for effect (vs simply incrementing by 100 at a time).
/// </summary>
[RequireComponent(typeof(Text))]
public class TextPoints : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How quickly the points displayed should increase to match the actual number of points the player has earned.
  /// </summary>
  [SerializeField]
  float scrollSpeedFactor = 5;

  /// <summary>
  /// The text component being updated with the current point value.
  /// </summary>
  Text text;

  /// <summary>
  /// The number of points last displayed, used to 'scroll' the points up to the current value.
  /// </summary>
  /// <remarks>Defaulting to -1 causes points to be dirty on the first frame</remarks>
  int previousPoints = -1;
  #endregion

  #region Init
  /// <summary>
  /// Initialize variables.
  /// </summary>
  protected void Start()
  {
    Debug.Assert(scrollSpeedFactor > 0);

    text = GetComponent<Text>();

    Debug.Assert(text != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// Refresh the points displayed.
  /// </summary>
  protected void Update()
  {
    if(previousPoints == GameController.instance.points)
    { // We are already displaying the current point value
      return;
    }

    // Scroll points
    int deltaPoints = GameController.instance.points - previousPoints;
    int pointsToDisplay = previousPoints + Math.Max(1, (int)(deltaPoints * scrollSpeedFactor * Time.deltaTime));
    previousPoints = pointsToDisplay;

    // Display the point value, which is less than or equal to the number of points the player has earned
    text.text = pointsToDisplay.ToStringWithCommas();
  }
  #endregion
}
