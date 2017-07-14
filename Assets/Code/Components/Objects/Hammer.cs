using UnityEngine;

/// <summary>
/// The main component for the hammer item.  This transitions the hammer from an item to something in the player's hand.
/// </summary>
public class Hammer : MonoBehaviour
{
  #region Data
  /// <summary>
  /// When equipting, this is the child position for the hammer relative to the entity.
  /// </summary>
  [SerializeField]
  Vector2 positionWhenEquipt = new Vector2 (.214f, -.321f);

  /// <summary>
  /// When equipting, this is the child rotation for the hammer relative to the entity.
  /// </summary>
  [SerializeField]
  Vector3 rotationWhenEquiptInEuler = new Vector3(0, 0, -90);

  /// <summary>
  /// The player which is holding this hammer, if any.  Used to ensure the player has only 1 at a time.
  /// </summary>
  Player currentPlayer;
  #endregion

  #region Init
  /// <summary>
  /// Inform the player that they are not holding a hammer anymore.
  /// </summary>
  protected void OnDestroy()
  {
    if(currentPlayer != null)
    { // Unassign hammer
      currentPlayer.currentWeapon = null;
    }
  }
  #endregion

  #region Events
  /// <summary>
  /// When a player touches this item, consider equipting the hammer.
  /// </summary>
  /// <param name="collision">The gameObject which touched the hammer.</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    Player player = collision.GetComponent<Player>();
    // If it was a player that touched us and there is not already a hammer in use
    if(player != null && currentPlayer == null && player.currentWeapon == null)
    {
      // Enable hammer
      currentPlayer = player;
      currentPlayer.currentWeapon = this;
      GetComponent<KillOnContactWith>().enabled = true;

      // Position hammer
      transform.SetParent(currentPlayer.transform);
      transform.localPosition = positionWhenEquipt;
      transform.localRotation = Quaternion.Euler(rotationWhenEquiptInEuler);

      // Start effects
      GetComponent<Animator>().enabled = true;
      gameObject.CallIDie();
    }
  }
  #endregion
}
