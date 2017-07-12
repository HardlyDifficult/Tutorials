using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KillOnContactWith : MonoBehaviour
{
  [SerializeField]
  LayerMask layersToKill;

  public event Action<GameObject> onHit;

  void OnCollisionEnter2D(
    Collision2D collision)
  {
    if(enabled == false)
    {
      return;
    }
    if(layersToKill.Includes(collision.gameObject.layer))
    {
      TryKilling(collision.gameObject);
    }
  }

  void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(enabled == false)
    {
      return;
    }
    if(layersToKill.Includes(collision.gameObject.layer))
    {
      TryKilling(collision.gameObject);
    }
  }

  /// <summary>
  /// Unity Hack to allow enable/disable
  /// </summary>
  void Start() {}

  void TryKilling(
    GameObject gameObjectWeJustHit)
  {
    if(onHit != null)
    {
      onHit(gameObjectWeJustHit);
    }
    Explosion.ExplodeAt(gameObjectWeJustHit.transform.position);
    IDie[] iDieList = gameObjectWeJustHit.GetComponents<IDie>();
    for(int i = 0; i < iDieList.Length; i++)
    {
      IDie iDie = iDieList[i];
      iDie.Die();
    }
  }
}