using UnityEngine;

/// <summary>
/// When enabled (e.g. via an animation), play the specified animation.
/// </summary>
public class PlayAnimationOnEnable : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The name of the animation state to play.
  /// </summary>
  [SerializeField]
  string animationToPlay;
  #endregion

  #region Init
  /// <summary>
  /// On enable, start the animation playback.
  /// </summary>
  void OnEnable()
  {
    GetComponent<Animator>().Play(animationToPlay);
  }
  #endregion
}
