using UnityEngine;

/// <summary>
/// Controls forces for jumping.  Another component interfaces with this to inform when to jump.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Feet))]
public class JumpMovement : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The sound to play when the character starts their jump.
  /// </summary>
  [SerializeField]
  AudioClip jumpSound;

  /// <summary>
  /// How much force to apply on jump.
  /// </summary>
  [SerializeField]
  float jumpSpeed = 6.5f;

  /// <summary>
  /// Used to add force on jump.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Used to determine if you are on the ground.
  /// </summary>
  Feet feet;

  /// <summary>
  /// The audioSource to play sound effects with.
  /// </summary>
  /// <remarks>Cached here for performance</remarks>
  AudioSource audioSource;
  #endregion

  #region Init
  /// <summary>
  /// On awake, initialize variables.
  /// </summary>
  protected void Awake()
  {
    Debug.Assert(jumpSpeed >= 0);

    audioSource = Camera.main.GetComponent<AudioSource>();
    myBody = GetComponent<Rigidbody2D>();
    feet = GetComponent<Feet>();

    Debug.Assert(myBody != null);
    Debug.Assert(feet != null);
  }
  #endregion

  #region API
  /// <summary>
  /// Adds force to the body to make the entity jump.
  /// </summary>
  public void Jump()
  {
    if(feet.isGrounded == false)
    { // Can't jump if you are not grounded
      return;
    }

    audioSource.PlayOneShot(jumpSound);
    myBody.AddForce((Vector2)transform.up * jumpSpeed, ForceMode2D.Impulse);
  }
  #endregion
}
