/// <summary>
/// Any/all component(s) on the gameObject may use this to add
/// effects / animation on death.
/// </summary>
public interface IHaveDeathEffect
{
  /// <summary>
  /// Called when the object is ready to be Destroyed.
  /// </summary>
  void Die();
}
