using System.Collections;
using UnityEngine;

/// <summary>
/// Causes the sprite to flash colors (between white/red) faster and faster until Destroy.
/// </summary>
public class DeathEffectSpriteFlash : MonoBehaviour, IHaveDeathEffect
{
  #region Data
  /// <summary>
  /// How long to wait before starting the death visual effects.
  /// </summary>
  [SerializeField]
  float lengthBeforeFlash = 7; 

  /// <summary>
  /// How long to flash before Destroy.
  /// </summary>
  [SerializeField]
  float lengthToFlashFor = 5;

  /// <summary>
  /// How quickly to switch between colors.
  /// </summary>
  [SerializeField]
  float timePerColorChange = .75f;

  /// <summary>
  /// How much to scale down the timePerColorChange after each color change.
  /// </summary>
  [SerializeField]
  float colorChangeTimeFactorPerFlash = .85f;
  #endregion

  #region API
  /// <summary>
  /// Called when this object should die.  Triggers the flash effect before Destroy.
  /// </summary>
  void IHaveDeathEffect.Die()
  {
    StartCoroutine(FlashToDeath());
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the flash effect and then Destroy gameObject.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator FlashToDeath()
  {
    // Wait before starting flash
    yield return new WaitForSeconds(lengthBeforeFlash);

    // Flash over time
    SpriteRenderer[] spriteList = GetComponentsInChildren<SpriteRenderer>();
    float timePassed = 0;
    bool isRed = false;
    while(timePassed < lengthToFlashFor)
    {
      spriteList.SetColor(isRed ? Color.red : Color.white);
      isRed = !isRed;

      yield return new WaitForSeconds(timePerColorChange);
      timePerColorChange = Mathf.Max(Time.deltaTime, timePerColorChange);
      timePassed += timePerColorChange;
      timePerColorChange *= colorChangeTimeFactorPerFlash;
    }

    // Cleanup
    Destroy(gameObject);
  }
  #endregion
}
