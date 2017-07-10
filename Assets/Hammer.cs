using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
  [SerializeField]
  Vector2 positionWhenEquipt;
  [SerializeField]
  Vector3 rotationWhenEquiptInEuler;

  PlayerController currentPlayer;
  SpriteRenderer sprite;
  [SerializeField]
  int pointsPerHammerKill;

  void Awake()
  {
    sprite = GetComponent<SpriteRenderer>();
  }

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    PlayerController player = collision.GetComponentInParent<PlayerController>();
    if(currentPlayer == null && player != null && player.currentWeapon == null)
    {
      currentPlayer = player;
      currentPlayer.currentWeapon = this;
      transform.SetParent(currentPlayer.transform);
      transform.localPosition = positionWhenEquipt;
      transform.localRotation = Quaternion.Euler(rotationWhenEquiptInEuler);
      StartCoroutine(HammerTime());
    }
    else if(currentPlayer != null // Weapon is equipt
      && collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) // And hit a bad guy
    {
      StartCoroutine(SlowDeath(collision.gameObject));
    }
  }

  IEnumerator SlowDeath(
    GameObject gameObject)
  {
    const int slowLoopCount = 5;
    for(int i = slowLoopCount - 1; i >= 0; i--)
    {
      Time.timeScale = i * (1f / slowLoopCount);
      yield return new WaitForSecondsRealtime(.001f);
    }
    Explosion.ExplodeAt(gameObject.transform.position);
    yield return new WaitForSecondsRealtime(.1f);
    Destroy(gameObject);
    GameController.instance.points += pointsPerHammerKill;
    yield return new WaitForSecondsRealtime(.25f);
    const int speedUpLoopCount = 30;
    for(int i = 0; i < speedUpLoopCount; i++)
    {
      Time.timeScale = i * (1f / speedUpLoopCount);
      yield return new WaitForSecondsRealtime(.001f);
    }
    Time.timeScale = 1;
  }

  void OnDestroy()
  {
    if(currentPlayer != null)
    {
      currentPlayer.currentWeapon = null;
    }
  }

  IEnumerator HammerTime()
  {
    GetComponent<Animator>().enabled = true;
    yield return new WaitForSeconds(5);
    for(int i = 0; i < 20; i++)
    {
      sprite.color = i % 2 == 0 ? Color.white : Color.red;
      yield return new WaitForSeconds(1f / (i + 1));
    }
    Destroy(gameObject);
  }
}
