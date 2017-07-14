using UnityEngine;
using System;

/// <summary>
/// Controls the entity's walk movement.
/// Another component drives walk via inputWalkDirection.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class WalkMovement : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The rotation that's applied when looking left (vs right).
  /// </summary>
  /// <remarks>Cached here for performance.</remarks>
  static readonly Quaternion backwardsRotation = Quaternion.Euler(0, 180, 0);

  /// <summary>
  /// Set by another component to inform this component it should walk and which direction.
  /// </summary>
  [NonSerialized]
  public float inputWalkDirection;

  /// <summary>
  /// How quickly the entity walks.
  /// </summary>
  [SerializeField]
  float movementSpeed = 100;

  /// <summary>
  /// How quickly the entity rotates so that it's feet are both on the floor.
  /// </summary>
  [SerializeField]
  float rotationLerpSpeed = .4f;

  /// <summary>
  /// Used to control movement.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Used to know the rotation of the floor we are on.
  /// </summary>
  Feet feet;

  /// <summary>
  /// The direction we are currently walking, used to know when we turn around.
  /// </summary>
  /// <remarks>Defaults to true as our prefabs are configured facing right.</remarks>
  bool _isGoingRight = true;
  #endregion

  #region Properties
  /// <summary>
  /// The direction we are currently walking.
  /// When changed, flips the rotation so the entity is facing forward.
  /// </summary>
  bool isGoingRight
  {
    get
    {
      return _isGoingRight;
    }
    set
    {
      if(isGoingRight == value)
      { // The value is not changing
        return;
      }

      // Flip the entity
      transform.rotation *= backwardsRotation;
      _isGoingRight = value;
    }
  }
  #endregion

  #region Init
  /// <summary>
  /// Initialize variables.
  /// </summary>
  protected void Awake()
  {
    Debug.Assert(movementSpeed > 0);
    Debug.Assert(rotationLerpSpeed >= 0);

    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponentInChildren<Feet>();
    isGoingRight = Vector2.Dot(Vector2.right, transform.right) > 0;

    Debug.Assert(myBody != null);
    Debug.Assert(feet != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// Set velocity on the entity given the inputWalkDirection.
  /// Any y velocity is preserved, this allows gravity to continue working.
  /// </summary>
  /// <remarks>
  /// With this approach, forces may not be used to impact the x on this entity.  
  /// E.g. if we wanted a fan which slowly pushed characters to the left, the force added would be lost here.
  /// This matches the Unity example character asset as enabling forces on both dimentions cause movement to feel strange 
  /// or leads to other experience problems which quickly complicate the code 
  /// (but possible of course, just thing things through).
  /// </remarks>
  protected void FixedUpdate()
  {
    myBody.velocity = new Vector2(inputWalkDirection * movementSpeed * Time.fixedDeltaTime, myBody.velocity.y);
  }

  /// <summary>
  /// Rotate the entity given the walk direction and the floor's angle.
  /// </summary>
  protected void Update()
  {
    Update_ConsiderFlippingEntity();
    Update_RotateCharacterToMatchPlatform();
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Look at the current velocity and consider flipping the sprite if the direction has changed
  /// </summary>
  void Update_ConsiderFlippingEntity()
  {
    float xVelocity = myBody.velocity.x;
    if(isGoingRight == false && xVelocity > 0.001
      || isGoingRight && xVelocity < -0.001)
    { // Direction changed
      isGoingRight = inputWalkDirection > 0;
    }
  }

  /// <summary>
  /// Rotate the entity so that both feet are flat on the floor
  /// </summary>
  void Update_RotateCharacterToMatchPlatform()
  {
    Quaternion targetRotation = feet.floorRotation;
    if(isGoingRight == false)
    { // If the entity is flipped, also flip the target rotation we are lerping towards
      targetRotation *= backwardsRotation;
    }

    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
  }
  #endregion
}
