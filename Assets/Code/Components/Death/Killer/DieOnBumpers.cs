using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace HD
{
  public class DieOnBumpers : MonoBehaviour
  {
    void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.CompareTag("Bumper")
        && Player.instance != null)
      {
        if(transform.position.y < Player.instance.transform.position.y)
        {
          Destroy(gameObject);
        }
      }
    }
  }
}
