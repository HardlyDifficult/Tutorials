using UnityEngine;

/// <summary>
/// When enabled (e.g. via an animation), play the specified animation.
/// </summary>
[RequireComponent(typeof(Animator))]
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
  protected void OnEnable()
  {
    Debug.Assert(animationToPlay != null);

    Animator animator = GetComponent<Animator>();
    animator.Play(animationToPlay);
  }
  #endregion
}
