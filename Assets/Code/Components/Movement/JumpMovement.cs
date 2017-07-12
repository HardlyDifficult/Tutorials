using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class JumpMovement : MonoBehaviour
{
  #region Data
  [SerializeField]
  float jumpSpeed = 1;

  Rigidbody2D myBody;

  public bool hasJumpedSinceLastUpdate
  {
    private get;
    set;
  }
  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    if(hasJumpedSinceLastUpdate)
    {
      myBody.AddForce((Vector2)transform.up * jumpSpeed, ForceMode2D.Impulse);
      hasJumpedSinceLastUpdate = false;
    }
  }
  #endregion
}
