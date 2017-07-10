using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ClimbLadder))]
public class PlayerController : MonoBehaviour
{
  #region Data
  public static PlayerController instance;

  public float maxSpeed = 1;
  public float jumpSpeed = 1;
  public float characterHeight = 1;

  [NonSerialized]
  public Hammer currentWeapon;

  public Rigidbody2D myBody;
  Feet feet;
  ClimbLadder climbLadder;
  Animator animator;
  bool hasJumpedSinceLastUpdate;

  MoveStandard moveController;
  #endregion

  #region Init
  void Awake()
  {
    instance = this;
    myBody = GetComponent<Rigidbody2D>();
    climbLadder = GetComponent<ClimbLadder>();
    feet = GetComponentInChildren<Feet>();
    animator = GetComponentInChildren<Animator>();
    moveController = GetComponent<MoveStandard>();
  }

  void OnDestroy()
  {
    instance = null;
  }
  #endregion

  void Update()
  {
    if(feet.isGrounded)
    {
      Update_Jump();
    }
    Update_ClimbLadder();
    Update_KeepInBounds();
    Update_Animation();
  }

  void FixedUpdate()
  {
    moveController.walkSpeed = Input.GetAxis("Horizontal"); // For reliable results, execution order should be before move standard

    if(hasJumpedSinceLastUpdate)
    {
      myBody.AddForce((Vector2)transform.up * jumpSpeed, ForceMode2D.Impulse);
      hasJumpedSinceLastUpdate = false;
    }
  }

  void Update_ClimbLadder()
  { // TODO separate player controls from movement (for bad guys)
    if(currentWeapon != null)
    { // You can't climb while swinging a hammer
      moveController.climbDirection = 0;
    }
    else
    {
      float vertical = Input.GetAxis("Vertical");
      moveController.climbDirection = vertical;
    }
  }


  void Update_Jump()
  {
    if(Input.GetButtonDown("Jump"))
    {
      hasJumpedSinceLastUpdate = true;
    }
  }

  void Update_KeepInBounds()
  {
    if(GameController.screenBounds.Contains(transform.position) == false)
    { // Player is out of bounds
      transform.position = GameController.screenBounds.ClosestPoint(transform.position);
    }
  }

  void Update_Animation()
  {
    animator.SetFloat("Speed", myBody.velocity.magnitude);
    animator.SetBool("isGrounded", feet.isGrounded);
    animator.SetBool("isClimbing", climbLadder.isOnLadder);
    animator.SetBool("hasWeapon", currentWeapon != null);
  }
}
