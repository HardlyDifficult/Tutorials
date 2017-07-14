using UnityEngine;

/// <summary>
/// On start, triggers a countdown till Destroying the gameObject.
/// </summary>
public class SuicideInSeconds : MonoBehaviour
{
  #region Data
  /// <summary>
  /// How long till this object should be destroyed.
  /// </summary>
  [SerializeField]
  float timeTillDeath;
  #endregion

  #region Init
  /// <summary>
  /// On start, begin the countdown till Destroy.
  /// </summary>
  void Start()
  {
    Destroy(gameObject, timeTillDeath);
  }
  #endregion
}
