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
  float timeTillDeath = 5;
  #endregion

  #region Init
  /// <summary>
  /// On start, begin the countdown till Destroy.
  /// </summary>
  protected void Start()
  {
    Debug.Assert(timeTillDeath > 0);

    Destroy(gameObject, timeTillDeath);
  }
  #endregion
}
