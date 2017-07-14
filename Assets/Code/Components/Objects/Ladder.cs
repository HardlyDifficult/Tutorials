using UnityEngine;

/// <summary>
/// Added to ladders in the world, providing data for other components (e.g. if it's a broken ladder).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Ladder : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Some entities cannot climb broken ladders.
  /// </summary>
  [SerializeField]
  bool isBrokenLadder;

  /// <summary>
  /// This is the trigger bounds for the ladder, used to determine when to get on/off.
  /// </summary>
  public Bounds bounds
  {
    get; private set;
  }
  #endregion

  #region Properties
  /// <summary>
  /// Some entities cannot climb broken ladders.
  /// </summary>
  /// <remarks>This is a public read-only view of the SerializeField above.</remarks>
  public bool isBroken
  {
    get
    {
      return isBrokenLadder;
    }
  }
  #endregion

  #region Init
  /// <summary>
  /// Initialize variables.
  /// </summary>
  protected void Awake()
  {
    Collider2D collider = GetComponent<Collider2D>();
    bounds = collider.bounds;
  }
  #endregion
}
