using UnityEngine;

/// <summary>
/// Provides additional convenience methods for Unity's GameObject.
/// </summary>
public static class GameObjectExtensions
{
  /// <summary>
  /// This is used to animate the death of some entities.
  /// Triggers the death effect on the gameObject.  If there are no effects, Destroy the gameObject.
  /// Death effects will Destroy the gameObject when complete.
  /// </summary>
  /// <param name="gameObject">The gameObject to destroy.</param>
  /// <remarks>
  /// We use this approach vs triggering an effect on Destroy because unfortunitally that's too late
  /// to cause an animation / delay the actual destruction of the gameObject.
  /// </remarks>
  public static void CallIDie(
    this GameObject gameObject)
  {
    IHaveDeathEffect[] deathEffectList = gameObject.GetComponents<IHaveDeathEffect>();
    if(deathEffectList.Length == 0)
    { // If no die scripts, destroy instead
      MonoBehaviour.Destroy(gameObject);
      return;
    }

    for(int i = 0; i < deathEffectList.Length; i++)
    { // Start all death effects on this gameObject
      IHaveDeathEffect deathEffect = deathEffectList[i];
      deathEffect.Die();
    }
  }
}
