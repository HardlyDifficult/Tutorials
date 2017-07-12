using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WalkMovement : MonoBehaviour
{
  #region Data
  static readonly Quaternion backwardsRotation = Quaternion.Euler(0, 180, 0);

  [NonSerialized]
  public float inputWalkDirection;

  [SerializeField]
  float movementSpeed = 100;
  [SerializeField]
  float rotationLerpSpeed = .4f;

  Rigidbody2D myBody;
  Feet feet;
  bool _isGoingRight = true;
  #endregion

  #region Properties
  bool isGoingRight
  {
    get
    {
      return _isGoingRight;
    }
    set
    {
      if(isGoingRight == value)
      {
        return;
      }

      transform.rotation *= backwardsRotation;
      _isGoingRight = value;
    }
  }
  #endregion

  #region Init
  void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponentInChildren<Feet>();
    isGoingRight = Vector2.Dot(Vector2.right, transform.right) > 0;
  }
  #endregion

  #region Events
  void FixedUpdate()
  {
    FixedUpdate_Move();
  }

  void Update()
  {
    Update_ConsiderFlippingEntity();
    Update_RotateCharacterToMatchPlatform();
  }
  #endregion

  #region Private helpers
  void FixedUpdate_Move()
  {
    myBody.velocity = new Vector2(inputWalkDirection * movementSpeed * Time.fixedDeltaTime, myBody.velocity.y);
  }

  void Update_ConsiderFlippingEntity()
  {
    float xVelocity = myBody.velocity.x;
    if(isGoingRight == false && xVelocity > 0.001
      || isGoingRight && xVelocity < -0.001)
    {
      isGoingRight = inputWalkDirection > 0;
    }
  }

  void Update_RotateCharacterToMatchPlatform()
  {
    Quaternion targetRotation = feet.floorRotation;
    if(isGoingRight == false)
    {
      targetRotation *= backwardsRotation;
    }
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
  }
  #endregion
}
