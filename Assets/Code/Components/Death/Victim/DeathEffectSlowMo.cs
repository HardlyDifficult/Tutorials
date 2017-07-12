using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections;

namespace HD
{
  public class DeathEffectSlowMo : MonoBehaviour, IDie
  {
    [SerializeField]
    float timeTillPaused = 1;
    [SerializeField]
    float timePausedTillSpeedUp = 1;
    [SerializeField]
    float timeForSpeedUp = 1;
    [SerializeField]
    int pointsPerDeath;

    public void Die()
    {
      Explosion.ExplodeAt(gameObject.transform.position);
      GameController.instance.timeController.SlowDownAndSpeedUp(timeTillPaused, timePausedTillSpeedUp, timeForSpeedUp);
      GameController.instance.points += pointsPerDeath;
      Destroy(gameObject);
    }
  }
}
