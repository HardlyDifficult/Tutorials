using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLine : MonoBehaviour, ICareWhenPlayerDies
{
  [SerializeField]
  float lifeCount;
  
  void ICareWhenPlayerDies.OnPlayerDeath()
  {
     if(GameController.instance.lifeCounter < lifeCount)
    {
      GetComponent<IDie>().Die();
    }
  }
}
