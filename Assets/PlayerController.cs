using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField]
  float speed = 1;
  [SerializeField]
  float jumpSpeed = 1;
  [SerializeField]
  float characterHeight = 1;
  Rigidbody2D rigidBody;

  Feet feet;
  Vector2 previousWalkVelocity;
  float lastJumpTime;

  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
    feet = GetComponentInChildren<Feet>();
  }

  void Update()
  {
    if(feet.isGrounded)
    {
      Quaternion targetRotation = feet.lastFloorHit.transform.rotation;
      transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .4f);
    }
  }

  void FixedUpdate()
  {
    Vector2 velocity = rigidBody.velocity;
    if(velocity.x > 0 && velocity.x > previousWalkVelocity.x
      || velocity.x < 0 && velocity.x < previousWalkVelocity.x)
    {
      velocity.x -= previousWalkVelocity.x;
    }
    if(velocity.y > 0 && velocity.y > previousWalkVelocity.y
     || velocity.y < 0 && velocity.y < previousWalkVelocity.y)
    {
      velocity.y -= previousWalkVelocity.y;
    }

    previousWalkVelocity = new Vector2(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, 0);
    velocity += previousWalkVelocity;

    if(Input.GetButtonDown("Jump")
      && feet.isGrounded
      && Time.timeSinceLevelLoad - lastJumpTime > .5f)
    {
      velocity += (Vector2)transform.up * jumpSpeed;
      lastJumpTime = Time.timeSinceLevelLoad;
    }

    rigidBody.velocity = velocity;
  }
}
