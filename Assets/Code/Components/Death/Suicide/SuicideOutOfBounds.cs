using UnityEngine;

/// <summary>
/// Suicide if this object falls off screen.
/// </summary>
public class SuicideOutOfBounds : MonoBehaviour
{
  #region Events
  /// <summary>
  /// On fixedUpdate, check if the object has fallen off screen.
  /// </summary>
  protected void FixedUpdate()
  {
    if(transform.position.y < -12)
    { // Fell out of bounds
      Destroy(gameObject);
    }
  }
  #endregion
}
