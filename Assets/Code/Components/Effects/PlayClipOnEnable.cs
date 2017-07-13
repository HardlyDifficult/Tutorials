using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClipOnEnable : MonoBehaviour
{
  [SerializeField]
  AudioClip clip;

  void OnEnable()
  {
    Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);
  }
}
