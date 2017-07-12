using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For reliable results, execution order should be before movement scripts
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))] 
public class Player : MonoBehaviour, IDie
{
  #region Data
  public static Player instance;

  [NonSerialized]
  public Hammer currentWeapon;

  public Rigidbody2D myBody;
  Feet feet;
  Animator animator;
  WalkMovement walkMovement;
  JumpMovement jumpMovement;
  LadderMovement climbMovement;
  #endregion

  #region Init
  void Awake()
  {
    instance = this;
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponent<Feet>();
    animator = GetComponentInChildren<Animator>();
    walkMovement = GetComponent<WalkMovement>();
    jumpMovement = GetComponent<JumpMovement>();
    climbMovement = GetComponent<LadderMovement>();

    AppearInSecondsAndFadeInSprite.DisableMeTillComplete(this);
  }

  void OnDisable()
  { // End of level
    Update_Animation(shouldFreeze: true);
  }

  void OnDestroy()
  {
    if(instance == this)
    {
      instance = null;
    }
  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    FixedUpdate_MoveLeftRight();
    FixedUpdate_ClimbLadder();
  }

  void Update()
  {
    Update_Jump();
    Update_Animation();
  }

  public void Die()
  {
    GameObject.FindObjectOfType<LevelManager>().YouDied();
  }
  #endregion

  #region Private helpers
  void FixedUpdate_MoveLeftRight()
  {
    walkMovement.inputWalkDirection = Input.GetAxis("Horizontal");
  }

  void FixedUpdate_ClimbLadder()
  { 
    if(currentWeapon != null)
    { // You can't climb while swinging a hammer
      climbMovement.climbDirection = 0;
    }
    else
    {
      float vertical = Input.GetAxis("Vertical");
      climbMovement.climbDirection = vertical;
    }
  }

  void Update_Jump()
  {
    if(feet.isGrounded
      && Input.GetButtonDown("Jump"))
    {
      jumpMovement.hasJumpedSinceLastUpdate = true;
    }
  }
  
  void Update_Animation(
    bool shouldFreeze = false)
  {
    animator.SetFloat("Speed", shouldFreeze ? 0 : myBody.velocity.magnitude);
    animator.SetBool("isGrounded", shouldFreeze ? false : feet.isGrounded);
    animator.SetBool("isClimbing", shouldFreeze ? false : climbMovement.isOnLadder);
    animator.SetBool("hasWeapon", shouldFreeze ? false : currentWeapon != null);
  }
  #endregion
}
