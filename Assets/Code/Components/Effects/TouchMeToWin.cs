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
  public static int numberActive;

  /// <summary>
  /// A LayerMask to define which objects to trigger a win condition on.
  /// </summary>
  [SerializeField]
  LayerMask layerToTriggerOn;
  #endregion

  #region Init
  /// <summary>
  /// On start, add this to the number of touch to win's remaining in the world.
  /// </summary>
  protected void Start()
  {
    numberActive++;
  }

  /// <summary>
  /// On destroy, remove this from the number of touch to win's remaining in the world.
  /// </summary>
  protected void OnDestroy()
  {
    numberActive--;
  }
  #endregion

  #region Events
  /// <summary>
  /// On collision, trigger a win if the object which hit us matches the LayerMask.
  /// </summary>
  /// <param name="collision">The thing that hit us.</param>
  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    if(layerToTriggerOn.Includes(collision.gameObject.layer))
    {
      GameObject.FindObjectOfType<LevelManager>().YouWin();

      Destroy(this); // Prevent this script from responding to another collision.
    }
  }
  #endregion
}
