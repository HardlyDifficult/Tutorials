using UnityEngine;

/// <summary>
/// Triggers YouWin on the LevelManager when the player touches this object.
/// </summary>
public class TouchMeToWin : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The number of touch to win objects remaining in this world.
  /// </summary>
  public static int totalNumberActive;

  /// <summary>
  /// A LayerMask to define which objects to trigger a win condition on.
  /// </summary>
  [SerializeField]
  LayerMask layerToTriggerOn;

  /// <summary>
  /// Tracks if this object has already been decremented from the totalNumberActive.
  /// </summary>
  bool isActive;
  #endregion

  #region Init
  /// <summary>
  /// On start, add this to the number of touch to win's remaining in the world.
  /// </summary>
  protected void Start()
  {
    totalNumberActive++;
    isActive = true;
  }

  /// <summary>
  /// On destroy, remove this from the number of touch to win's remaining in the world.
  /// </summary>
  protected void OnDestroy()
  {
    if(isActive)
    {
      totalNumberActive--;
    }

    Debug.Assert(totalNumberActive >= 0);
  }
  #endregion

  #region Events
  /// <summary>
  /// On collision, trigger a win if the object which hit us matches the LayerMask.
  /// </summary>
  /// <param name="collision">The thing that hit us.</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    CheckForWin(collision.gameObject);
  }

  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    CheckForWin(collision.gameObject);
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Checks if the gameObject is included in the layerMask, if so call YouWin on the levelManager;
  /// </summary>
  /// <param name="gameObject">The gameObject we just touched.</param>
  void CheckForWin(
    GameObject gameObject)
  {
    if(isActive == false)
    { // Already triggered this object
      return;
    }

    if(layerToTriggerOn.Includes(gameObject.layer))
    {
      isActive = false;
      totalNumberActive--;
      GameObject.FindObjectOfType<LevelManager>().YouWin();
    }
  }
  #endregion
}
