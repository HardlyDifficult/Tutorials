using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
  #region Data
  [SerializeField]
  Vector2 positionWhenEquipt;
  [SerializeField]
  Vector3 rotationWhenEquiptInEuler;

  Player currentPlayer;
  SpriteRenderer sprite;
  #endregion

  #region Init
  void Awake()
  {
    sprite = GetComponent<SpriteRenderer>();
  }

  void OnDestroy()
  {
    if(currentPlayer != null)
    { // Unassign hammer
      currentPlayer.currentWeapon = null;
    }
  }
  #endregion

  #region Events
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    Player player = collision.GetComponent<Player>();
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
      GetComponent<DieOverTimeWithSpriteFlash>().enabled = true;
    }
  }
  #endregion
}
