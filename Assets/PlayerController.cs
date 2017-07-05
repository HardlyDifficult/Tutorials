using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField]
  float speed = 1;
  [SerializeField]
  float jumpSpeed = 1;

  Rigidbody2D rigidBody;

  public bool isGrounded
  {
    get
    {
      if(Mathf.Abs(rigidBody.velocity.y) < .0001f)
      {
        return true;
      }
      return false;
    }
  }

  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
  }

  void FixedUpdate()
  {
    Vector2 velocity = rigidBody.velocity; ;

    float horizontal = Input.GetAxis("Horizontal");
    velocity.x = horizontal * speed * Time.fixedDeltaTime;

    if(Input.GetButtonDown("Jump")
      && isGrounded)
    {
      velocity.y += jumpSpeed;
    }

    rigidBody.velocity = velocity;
  }
}
