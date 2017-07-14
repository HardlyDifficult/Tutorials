using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Causes the sprite on this gameObject to fade in slowly.
/// Other components may call DisableMeTillComplete to pause until the fade is complete.
/// </summary>
public class AppearInSecondsAndFadeInSprite : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Used by DisableMeTillComplete for a notification when the fade is done.
  /// </summary>
  Action onComplete;

  /// <summary>
  /// How long to wait before showing anything.
  /// </summary>
  [SerializeField]
  float timeBeforeSpawn = 3;

  /// <summary>
  /// How long to lerp the alpha down to 0.
  /// </summary>
  [SerializeField]
  float lengthTillVisible = 1;
  #endregion

  #region Init
  /// <summary>
  /// On start, begin the fade effect coroutine.
  /// </summary>
  protected void Start()
  {
    StartCoroutine(FadeIn());
  }

  /// <summary>
  /// Called by other components (usually on the same object) to disable until the sprite is fully visible.
  /// </summary>
  /// <param name="script">The script which should be disabled.</param>
  public static void DisableMeTillComplete(
   MonoBehaviour script)
  {
    AppearInSecondsAndFadeInSprite appearIn = script.GetComponent<AppearInSecondsAndFadeInSprite>();
    if(appearIn != null)
    {
      script.enabled = false;
      appearIn.onComplete += () => script.enabled = true;
    }
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Executes the sprite fade in overtime.
  /// </summary>
  /// <returns>Used by coroutines to manage time.</returns>
  IEnumerator FadeIn()
  {
    Debug.Assert(timeBeforeSpawn >= 0);
    Debug.Assert(lengthTillVisible >= 0);

    // Hide sprites
    SpriteRenderer[] spriteList = gameObject.GetComponentsInChildren<SpriteRenderer>();
    Debug.Assert(spriteList.Length > 0);

    spriteList.SetAlpha(0);

    // Wait before the fade begins
    yield return new WaitForSeconds(timeBeforeSpawn);

    // Fade them back
    float timePassed = 0;
    while(timePassed < lengthTillVisible)
    {
      float percentComplete = timePassed / lengthTillVisible;
      spriteList.SetAlpha(percentComplete);

      // Wait for the next Update
      yield return 0;
      timePassed += Time.deltaTime;
    }

    // Clean up
    spriteList.SetAlpha(1);
    if(onComplete != null)
    {
      onComplete.Invoke();
    }
  }
  #endregion
}
