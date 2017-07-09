using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MoveStandard : MonoBehaviour
{
  PlayerController controller;

  void Awake()
  {
    controller = GetComponent<PlayerController>();
  }

  void FixedUpdate()
  {
    float walkSpeed = Input.GetAxis("Horizontal") * controller.speed * Time.fixedDeltaTime;
    controller.myBody.velocity = new Vector2(walkSpeed, controller.myBody.velocity.y);
  }
}
