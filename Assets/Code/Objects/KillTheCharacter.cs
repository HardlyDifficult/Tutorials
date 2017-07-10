using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTheCharacter : MonoBehaviour
{
  void OnCollisionEnter2D(
    Collision2D collision)
  {
    TryKilling(collision.gameObject);
  }

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    TryKilling(collision.gameObject);
  }

  static void TryKilling(
    GameObject gameObjectWeJustHit)
  {
    if(gameObjectWeJustHit.CompareTag("Player"))
    {
      Explosion.ExplodeAt(gameObjectWeJustHit.transform.position);
      Destroy(gameObjectWeJustHit);
    }
  }
}