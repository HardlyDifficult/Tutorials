/// <summary>
/// A way for any object to be notified when the player dies.
/// This is an alternative pattern to using Events, which could have accomplished the same.
/// </summary>
public interface ICareWhenPlayerDies
{
  /// <summary>
  /// Called by the LevelManager when the player has died.
  /// </summary>
  void OnPlayerDeath();
}
