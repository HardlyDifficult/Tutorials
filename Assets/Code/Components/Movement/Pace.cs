using UnityEngine;

/// <summary>
/// Causes an entity to walk with a static speed.  
/// The KeepMovementOnScreen will cause the entity to bounce back/forth, creating a pacing effect.
/// </summary>
[RequireComponent(typeof(WalkMovement))]
public class Pace : MonoBehaviour
{
  #region Init
  /// <summary>
  /// On start, start walking
  /// </summary>
  protected void Start()
  {
    WalkMovement walkMovement = GetComponent<WalkMovement>();
    walkMovement.inputWalkDirection = 1;
  }
  #endregion
}
