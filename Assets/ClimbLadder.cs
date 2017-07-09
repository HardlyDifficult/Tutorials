using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLadder : MonoBehaviour
{
  #region Data
  Rigidbody2D myBody;

  public float climbSpeed = 1;
  public bool canClimbBrokenLadders;
  public Ladder currentLadder
  {
    get; private set;
  }
  bool _isOnLadder;
  #endregion

  #region Properties
  public bool isOnLadder
  {
    get
    {
      return _isOnLadder;
    }
    set
    {
      print(value);
      _isOnLadder = value;

      // TODO this is weird
      if(isOnLadder)
      {
        myBody.GetComponent<Collider2D>().isTrigger = true;
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Feet"), LayerMask.NameToLayer("Floor"), true);
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Body"), LayerMask.NameToLayer("Floor"), true);
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Floor"), true);
        myBody.gravityScale = 0;
      }
      else
      {
        myBody.GetComponent<Collider2D>().isTrigger = false;
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Feet"), LayerMask.NameToLayer("Floor"), false);
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Body"), LayerMask.NameToLayer("Floor"), false);
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Floor"), true);
        myBody.gravityScale = 1;
      }
    }
  }
  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
  }
  #endregion

  #region Events
  void OnTriggerEnter2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    {
      return;
    }
    if(canClimbBrokenLadders || ladder.isBroken == false)
    {
      currentLadder = ladder;
    }
  }

  public void StartClimbing()
  {
    isOnLadder = true;
  }

  void OnTriggerExit2D(
    Collider2D collision)
  {
    Ladder ladder = collision.GetComponent<Ladder>();
    if(ladder == null)
    {
      return;
    }
    currentLadder = null;
    isOnLadder = false;
  }
  
  public void Climb(
    float verticalDirection)
  {
    myBody.velocity = new Vector2(myBody.velocity.x, verticalDirection * climbSpeed);
  }
  #endregion
}
