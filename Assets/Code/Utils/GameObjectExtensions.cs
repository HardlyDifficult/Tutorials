using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class GameObjectExtensions
{
  public static void CallIDie(
    this GameObject gameObject)
  {
    IHaveDeathEffect[] deathEffectList = gameObject.GetComponents<IHaveDeathEffect>();
    if(deathEffectList.Length == 0)
    { // If no die scripts, destroy instead
      MonoBehaviour.Destroy(gameObject);
      return;
    }

    for(int i = 0; i < deathEffectList.Length; i++)
    {
      IHaveDeathEffect deathEffect = deathEffectList[i];
      deathEffect.Die();
    }
  }
}
