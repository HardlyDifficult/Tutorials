using UnityEngine;

/// <summary>
/// When enabled (e.g. via animation), play an audio clip via the main camera's AudioSource.
/// </summary>
public class PlayClipOnEnable : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The audio clip to play.
  /// </summary>
  [SerializeField]
  AudioClip clip;
  #endregion

  #region Init
  /// <summary>
  /// On enable, play the specified clip using the Camera's AudioSource.
  /// </summary>
  protected void OnEnable()
  {
    Debug.Assert(clip != null);

    AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
    audioSource.PlayOneShot(clip);
  }
  #endregion
}
