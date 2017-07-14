using UnityEngine;

/// <summary>
/// Destoy's objects which touch the bumper (off screen) if the player is higher on the map at the moment.
/// </summary>
public class KillOnBumpers : MonoBehaviour
{
  #region Events
  /// <summary>
  /// On collision, if we hit a bumper check the player location and consider Destroy.
  /// </summary>
  /// <param name="collision"></param>
  void OnCollisionEnter2D(
    Collision2D collision)
  {
    if(collision.gameObject.CompareTag("Bumper")
      && Player.instance == null
      || transform.position.y < Player.instance.transform.position.y)
    { 
      Destroy(gameObject);
    }
  }
  #endregion
}
