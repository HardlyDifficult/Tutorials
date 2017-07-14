using UnityEngine;

/// <summary>
/// Locks the gameObject's rotation to identity.
/// </summary>
/// <remarks>
/// This is used on a child gameObject for the bomb so that it's collider is not spinning as the parent rolls around.
/// </remarks>
public class RotateToIdentity : MonoBehaviour
{
  #region Events
  /// <summary>
  /// Each update, set the rotation.
  /// </summary>
  protected void Update()
  {
    transform.rotation = Quaternion.identity;
  }
  #endregion
}
