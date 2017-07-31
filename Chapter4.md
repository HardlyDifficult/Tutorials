# 4) Ground detection & Ladders

## 4.1) Rotate entities when they walk the other way

Flip the entity when they switch between walking left and walking right.

<details><summary>How</summary>

 - Create script Code/Compenents/Movement/**RotateFacingDirection**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotateFacingDirection : MonoBehaviour
{
  static readonly Quaternion backwardsRotation 
    = Quaternion.Euler(0, 180, 0);

  Rigidbody2D myBody;

  bool _isGoingRight = true;

  public bool isGoingRight
  {
    get
    {
      return _isGoingRight;
    }
    private set
    {
      if(isGoingRight == value)
      { 
        return;
      }

      transform.rotation *= backwardsRotation;
      _isGoingRight = value;
    }
  }

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    Debug.Assert(myBody != null);
  }

  protected void Update()
  {
    float xVelocity = myBody.velocity.x;
    if(Mathf.Abs(xVelocity) > 0.1)
    { 
      isGoingRight = xVelocity > 0;
    }
  }
}
```

 - Add **RotateFacingDirection** to the character and the fly guy prefab.

<hr></details><br>
<details><summary>TODO</summary>

Why, the fly guy looks the same rotated.  Well may not be true for all art.  And simplifies the rotate to align with platforms coming up.

<hr></details>
<details><summary>What's a C# smart property?</summary>

In C#, data may be exposed as either a Field or a Property.  Fields are simply data as one would expect.  Properties are accessed in code like a field is, but they are capable of more.

In this example, when isGoingRight changes between true and false, the GameObject's transform is rotated so that the sprite faces the correct direction.  Leveraging the property changing to trigger the rotation change is an example of logic in the property making it 'smart'.

There are pros and cons to smart properties.  For example, one may argue that including the transform change when isGoingRight is modified hides the mechanic and makes the code harder to follow.  There are always alternatives if you prefer to not use smart properties.  For example:

```csharp
bool isGoingRightNow = xVelocity > 0;
if(isGoingRight != isGoingRightNow) 
{
  transform.rotation *= backwardsRotation;    
  isGoingRight = isGoingRightNow;
}
```

</details>

<details><summary>What's a Quaternion?</summary>

A Quaternion is how rotations are stored in a game engine.  They represent the rotation with (x, y, z, w) values, stored in this fashion because that it is an effecient way to do the necessary calculations when rendering on object on screen.

You could argue that this is overkill for a 2D game as in 2D the only rotation that may be applied is around the Z axis, and I would agree.  However remember that Unity is a 3D game engine.  When creating a 2D game, you are still in a 3D environment.  Therefore under the hood, Unity still optimizes its data for 3D.

Quaternions are not easy for people to understand.  When we think of rotations, we typically think in terms of 'Euler' (pronounced oil-er) rotations.  Euler rotations are degrees of rotation around each axis, e.g. (0, 0, 30) means rotate the object by 30 degrees around the Z axis.

In the inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quatenions directly or use Euler and then convert it back to Quatenion for storage.

Given a Quatenion, you can calculate the Euler value like so:

```csharp
Quaternion myRotationInQuaternion = transform.rotation;
Vector3 myRotationInEuler = myRotationInQuaternion.eulerAngles;
```

Given an Euler value, you can calculate the Quatenion:

```csharp
Quaternion rotationOfZ30Degrees = Quaternion.Euler(0, 0, 30);
```

Quaternions may be combined using Quaternion multiplication:

```csharp
Quaternion rotationOfZ60Degrees 
  = rotationOfZ30Degrees * rotationOfZ30Degrees;
```

</details>
<details><summary>TODO</summary>

TODO why Quaternion.Euler(0, 180, 0) when you said before 2D games only rotate around the z axis?

<hr></details>



<details><summary>Why not compare to 0 when checking if there is no movement?</summary>

In Unity, numbers are represented with the float data type.  Float is a way of representing decimal numbers but is a not precise representation like you may expect.  When you set a float to some value, internally it may be rounded ever so slightly.

The rounding that happens with floats allows operations on floats to be executed very quickly.  However it means we should never look for exact values when comparing floats, as a tiny rounding issue may lead to the numbers not being equal.

In the example above, as the velocity approaches zero, the significance of if the value is positive or negative, is lost.  It's possible that if we were to compare to 0 that at times the float may oscilate between a tiny negative value and a tiny positive value causing the sprite to flip back and forth.

</details>


## 4.2) Detect floors

Create a script to calculate the distance to and rotation of the floor under an entity.

<details><summary>How</summary>

 - Create a layer 'Floor'.
 - Select all the Platform GameObjects and change to Layer Floor.
 - Create script Code/Components/Movement/**FloorDetector**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloorDetector : MonoBehaviour
{
  static readonly Quaternion backwardsRotation 
    = Quaternion.Euler(0, 0, 180);

  public Collider2D myCollider
  {
    get; private set;
  }

  ContactFilter2D floorFilter;

  public bool isTouchingFloor
  {
    get; private set;
  }

  public Vector2? floorUp
  {
    get; private set;
  }

  public Quaternion? floorRotation
  {
    get; private set;
  }

  public float? distanceToFloor
  {
    get; private set;
  }

  protected void Awake()
  {
    myCollider = GetComponent<Collider2D>();

    floorFilter = new ContactFilter2D()
    {
      layerMask = LayerMask.GetMask(new[] { "Floor" }),
      useLayerMask = true
    };

    Debug.Assert(myCollider != null);
  }

  protected void FixedUpdate()
  {
    Collider2D floorWeAreStandingOn = DetectTheFloorWeAreStandingOn();
    isTouchingFloor = floorWeAreStandingOn != null;

    Collider2D floorUnderUs;
    if(floorWeAreStandingOn != null)
    {
      Vector2 up;
      Quaternion rotation;
      CalculateFloorRotation(floorWeAreStandingOn, out up, out rotation);
      floorUp = up;
      floorRotation = rotation;
      floorUnderUs = floorWeAreStandingOn;
    }
    else
    {
      floorUp = null;
      floorRotation = null;
      floorUnderUs = DetectFloorUnderUs();
    }

    distanceToFloor = CalculateDistanceToFloor(floorWeAreStandingOn, floorUnderUs);
  }

  Collider2D DetectTheFloorWeAreStandingOn()
  {
    Collider2D[] possibleResultList = new Collider2D[3];

    int foundColliderCound
      = Physics2D.OverlapCollider(myCollider, floorFilter, possibleResultList);

    for(int i = 0; i < foundColliderCound; i++)
    {
      Collider2D collider = possibleResultList[i];
      ColliderDistance2D distance = collider.Distance(myCollider);

      if(distance.distance >= -.1f
        && Vector2.Dot(Vector2.up, distance.normal) > 0)
      {
        return collider;
      }
    }

    return null;
  }

  Collider2D DetectFloorUnderUs()
  {
    RaycastHit2D[] result = new RaycastHit2D[1];
    if(Physics2D.Raycast(transform.position, Vector2.down, floorFilter, result) > 0)
    {
      return result[0].collider;
    }

    return null;
  }

  static void CalculateFloorRotation(
    Collider2D floorWeAreStandingOn,
    out Vector2 floorUp,
    out Quaternion floorRotation)
  {
    Debug.Assert(floorWeAreStandingOn != null);

    floorUp = floorWeAreStandingOn.transform.up;
    floorRotation = floorWeAreStandingOn.transform.rotation;
    if(Vector2.Dot(Vector2.up, floorUp) < 0)
    {
      floorUp = -floorUp;
      floorRotation *= backwardsRotation;
    }
  }
  
  float? CalculateDistanceToFloor(
    Collider2D floorWeAreStandingOn,
    Collider2D floorUnderUs)
  {
    if(floorWeAreStandingOn != null)
    {
      return 0;
    }
    else if(floorUnderUs != null)
    {
      float yOfTopOfFloor = floorUnderUs.bounds.max.y;

      if(floorUnderUs is BoxCollider2D)
      {
        BoxCollider2D boxCollider = (BoxCollider2D)floorUnderUs;
        yOfTopOfFloor += boxCollider.edgeRadius;
      }

      return myCollider.bounds.min.y - yOfTopOfFloor;
    }
    else
    {
      return null;
    }
  }
}
```

 - Add it to:
   - The character prefab.
   - The spike ball prefab.
   - The fly guy's **Feet** child GameObject (and apply changes to the fly guy prefab).

<hr></details><br>
<details><summary>TODO</summary>

TODO question - when changing layers, yes change children..
http://i.imgur.com/xFiD5Vc.png

TODO question - why not require floordetector component? / why GetComponentInChildren

<hr></details>




## 4.3) Prevent double jump

Update JumpMovement to prevent double jump and flying (by spamming space), by leveraging the FloorDetector just created.

<details><summary>How</summary>

 - Update JumpMovement with the following changes (or copy paste the full version TODO link):



<details><summary>Existing code</summary>

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
```

</details>

```csharp
[RequireComponent(typeof(FloorDetector))] 
```

<details><summary>Existing code</summary>

```csharp
public class JumpMovement : MonoBehaviour
{
  [SerializeField]
  AudioClip jumpSound;

  [SerializeField]
  float jumpSpeed = 7f;

  Rigidbody2D myBody;
```

</details>

```csharp
  FloorDetector floorDetector; 
```

<details><summary>Existing code</summary>

```csharp
  AudioSource audioSource;

  bool wasJumpRequestedSinceLastFixedUpdate;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
```

</details>

```csharp
    floorDetector = GetComponent<FloorDetector>(); 
```

<details><summary>Existing code</summary>

```csharp
    audioSource = GetComponent<AudioSource>();
  }

  public void Jump()
  {
    wasJumpRequestedSinceLastFixedUpdate = true;
  }

  protected void FixedUpdate()
  {
    if(wasJumpRequestedSinceLastFixedUpdate
```

</details>

```csharp
      && floorDetector.isTouchingFloor
```

<details><summary>Existing code</summary>

```csharp
      ) 
    {
      myBody.AddForce(
          new Vector2(0, jumpSpeed),
          ForceMode2D.Impulse);

      audioSource.PlayOneShot(jumpSound);
    }

    wasJumpRequestedSinceLastFixedUpdate = false;
  }
}
```

</details>


<hr></details><br>
<details><summary>TODO</summary>

TODO
TODO could add a jump cooldown by time as well - but that would not be a complete solution unless a long cooldown was used.

<hr></details>


## 4.4) Update wander to prefer traveling up hill

Update the WanderWalkController so that the fly guy is more likely to walk up hill than down.

<details><summary>How</summary>

 - Update the WanderWalkController as follows (or copy paste TODO link):


<details><summary>Existing code</summary>

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class WanderWalkController : MonoBehaviour
{
```

</details>

```csharp
  [SerializeField]
  float oddsOfGoingUpHill = .8f; 
```

<details><summary>Existing code</summary>

```csharp
  [SerializeField]
  float timeBeforeFirstWander = 10;

  [SerializeField]
  float minTimeBetweenReconsideringDirection = 1;

  [SerializeField]
  float maxTimeBetweenReconsideringDirection = 10;

  WalkMovement walkMovement;
```

</details>

```csharp
  FloorDetector floorDetector; 
```

<details><summary>Existing code</summary>

```csharp
  protected void Awake()
  {
    Debug.Assert(oddsOfGoingUpHill >= 0);
    Debug.Assert(timeBeforeFirstWander >= 0);

    walkMovement = GetComponent<WalkMovement>();
```

</details>

```csharp
    floorDetector = GetComponentInChildren<FloorDetector>(); 
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void Start()
  {
    StartCoroutine(Wander());
  }

  IEnumerator Wander()
  {
    walkMovement.desiredWalkDirection = 1;
    yield return new WaitForSeconds(timeBeforeFirstWander);

    while(true)
    {
      SelectARandomWalkDirection();

      float timeToSleep = UnityEngine.Random.Range(
        minTimeBetweenReconsideringDirection,
        maxTimeBetweenReconsideringDirection);
      yield return new WaitForSeconds(timeToSleep);
    }
  }

  void SelectARandomWalkDirection()
  {
```

</details>

```csharp
    float dot;
    if(floorDetector.floorUp != null)
    {
      dot = Vector2.Dot(floorDetector.floorUp.Value, Vector2.right);
    }
    else
    {
      dot = 0;
    }

    if(dot < 0)
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= oddsOfGoingUpHill ? 1 : -1;
    }
    else if(dot > 0)
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= oddsOfGoingUpHill ? -1 : 1;
    }
    else
    { 
```

<details><summary>Existing code</summary>

```csharp
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= .5f ? 1 : -1; 
```

</details>

```csharp
    }
```

<details><summary>Existing code</summary>

```csharp
  }
}
```

</details>




<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 4.5) Rotate to match the floor's angle

Create a script to rotate an entity, aligning with the floor when touching one, otherwise rotating back to the default position.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**RotateToAlignWithFloor**:

```csharp
using UnityEngine;

[RequireComponent(typeof(RotateFacingDirection))]
public class RotateToAlignWithFloor : MonoBehaviour
{
  static readonly Quaternion backwardsRotation
    = Quaternion.Euler(0, 180, 0);

  [SerializeField]
  float rotationLerpSpeed = .4f;

  FloorDetector floorDetector;

  RotateFacingDirection facingDirection;

  protected void Awake()
  {
    floorDetector = GetComponentInChildren<FloorDetector>();
    facingDirection = GetComponent<RotateFacingDirection>();

    Debug.Assert(floorDetector != null);
    Debug.Assert(facingDirection != null);
  }

  protected void Update()
  {
    Quaternion targetRotation;    
    if(floorDetector.floorRotation != null)
    {
      targetRotation = floorDetector.floorRotation.Value;
    }
    else
    {
      targetRotation = Quaternion.identity;
    }

    if(facingDirection.isGoingRight == false)
    {
      targetRotation *= backwardsRotation;
    }

    transform.rotation = Quaternion.Lerp(
      transform.rotation,
      targetRotation,
      rotationLerpSpeed * Time.deltaTime);
  }
}
```

 - Add it to the character and fly guy prefabs.

<hr></details><br>
<details><summary>TODO</summary>

TODO
TODO what is lerp (and slerp?)

<hr></details>


## 4.6) Add ladders to the world

Create GameObjects and layout ladders in the world and set their tag to Ladder.  

<details><summary>How</summary>

 - Create a parent Ladder GameObject, add the ladder sprite(s).  We are using **spritesheet_tiles_23** and **33**.
 - Order in Layer -2.
 - Position the ladder and repeat, creating several ladders - some which look broken.
   - The child sprite GameObjects should have a default Transform, with the execption of the Y position when multiple sprites are used.
   - It usually looks fine to overlap sprites a bit, as we do to get the space between ladder steps looking good.

<img src="http://i.imgur.com/CDwdJ3c.gif" width=500px />

 - Create a new parent GameObject to hold all the ladders (optional).
 - Create a tag for "Ladder".
 - Select all the ladder GameObjects and change their tag to Ladder.
 - Add FadeInThenEnable to all the ladders.

<hr></details><br>
<details><summary>TODO</summary>

TODO why tag and not layer here?

<hr></details>


## 4.7) Add trigger colliders to the ladders

Add BoxCollider2D to the ladders, size to use for climbing and set as trigger colliders.  

An entity will be able climb ladders when its bottom is above the bottom of the ladder's collider and its center is inside.

<details><summary>How</summary>

 - Add a BoxCollider2D and size it such that:
   - The width is thinner than the sprite (about .6).
   - The bottom of the collider:
     - Just below the platform for complete ladders.
     - Aligned with the last step of broken ladders.
   - The top of the collider is just above the upper platform.

<img src="http://i.imgur.com/r0k4eq3.png" width=300px />

 - Check 'Is Trigger'.

<hr></details><br>
<details><summary>TODO</summary>

TODO
TODO how do you know what size to make the collider?

<hr></details>



## 4.8) Add a script to climb ladders

Create a script to climb ladders for all entities to use, and update the player controller to match.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**LadderMovement**:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class LadderMovement : MonoBehaviour
{
  [NonSerialized]
  public float desiredClimbDirection;

  public event Action onGettingOnLadder;

  public event Action onGettingOffLadder;

  public bool isOnLadder
  {
    get
    {
      return ladderWeAreOn != null;
    }
  }

  [SerializeField]
  float climbSpeed = 60;

  Rigidbody2D myBody;

  Collider2D myCollider;

  FloorDetector floorDetector;

  GameObject _ladderWeAreOn;

  public GameObject ladderWeAreOn
  {
    get
    {
      return _ladderWeAreOn;
    }
    private set
    {
      if(_ladderWeAreOn == value)
      {
        return;
      }

      _ladderWeAreOn = value;

      if(ladderWeAreOn != null)
      {
        OnGettingOnLadder();
      }
      else
      {
        OnGettingOffLadder();
      }
    }
  }

  List<GameObject> currentLadderList;

  protected void Awake()
  {
    currentLadderList = new List<GameObject>();
    myBody = GetComponent<Rigidbody2D>();
    myCollider = GetComponent<Collider2D>();
    floorDetector = GetComponentInChildren<FloorDetector>();
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(collision.CompareTag("Ladder") == false)
    {
      return;
    }

    currentLadderList.Add(collision.gameObject);
  }

  protected void OnTriggerExit2D(
    Collider2D collision)
  {
    if(collision.gameObject == ladderWeAreOn)
    {
      GetOffLadder();
    }

    currentLadderList.Remove(collision.gameObject);
  }

  protected void FixedUpdate()
  {
    GameObject ladder = ladderWeAreOn;

    if(ladder == null)
    {
      ladder = FindClosestLadder();
      if(ladder == null)
      {
        return;
      }
    }

    Bounds ladderBounds = ladder.GetComponent<Collider2D>().bounds;
    Bounds entityBounds = floorDetector.myCollider.bounds;

    if(isOnLadder == false
      && Mathf.Abs(desiredClimbDirection) > 0.01
      && IsInBounds(ladderBounds, entityBounds))
    {
      if(
          desiredClimbDirection > 0 
            && entityBounds.min.y < ladderBounds.center.y
          || desiredClimbDirection < 0 
            && entityBounds.min.y > ladderBounds.center.y)
      {
        ladderWeAreOn = ladder;
      }
    }

    if(isOnLadder)
    {
      float currentVerticalVelocity = myBody.velocity.y;
      if(IsInBounds(ladderBounds, entityBounds) == false)
      {
        GetOffLadder();
      }
      else if(floorDetector.distanceToFloor < .3f
        && floorDetector.distanceToFloor > .1f)
      {
        if(currentVerticalVelocity > 0
            && entityBounds.min.y > ladderBounds.center.y)
        {
          GetOffLadder();
        }
        else if(currentVerticalVelocity < 0
          && entityBounds.min.y < ladderBounds.center.y)
        {
          GetOffLadder();
        }
      }

      if(isOnLadder)
      {
        myBody.velocity = new Vector2(myBody.velocity.x,
          desiredClimbDirection * climbSpeed * Time.fixedDeltaTime);
      }
    }
  }

  bool IsInBounds(
    Bounds ladderBounds,
    Bounds entityBounds)
  {
    float entityCenterX = entityBounds.center.x;
    if(ladderBounds.min.x > entityCenterX
      || ladderBounds.max.x < entityCenterX)
    {
      return false;
    }

    float entityFeetY = entityBounds.min.y;
    if(ladderBounds.min.y > entityFeetY
      || ladderBounds.max.y < entityFeetY)
    {
      return false;
    }

    return true;
  }

  public void GetOffLadder()
  {
    ladderWeAreOn = null;
  }

  void OnGettingOnLadder()
  {
    if(onGettingOnLadder != null)
    {
      onGettingOnLadder();
    }
  }

  void OnGettingOffLadder()
  {
    desiredClimbDirection = 0;

    if(onGettingOffLadder != null)
    {
      onGettingOffLadder();
    }
  }

  GameObject FindClosestLadder()
  {
    if(currentLadderList.Count == 0)
    {
      return null;
    }

    GameObject closestLadder = null;
    float distanceToClosestLadder = 0;
    for(int i = 0; i < currentLadderList.Count; i++)
    {
      GameObject ladder = currentLadderList[i];
      float distanceToLadder = (ladder.transform.position - transform.position).sqrMagnitude;
      if(closestLadder == null)
      {
        closestLadder = ladder;
        distanceToClosestLadder = distanceToLadder;
      }
      else
      {
        if(distanceToLadder < distanceToClosestLadder)
        {
          closestLadder = ladder;
          distanceToClosestLadder = distanceToLadder;
        }
      }
    }

    return closestLadder;
  }
}
```

 - Add it to the character, fly guy, and spike ball.
 - Update PlayerController as follows (or copy/paste TODO link):


<details><summary>Existing code</summary>

```csharp
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
[RequireComponent(typeof(JumpMovement))]
```

</details>

```csharp
[RequireComponent(typeof(LadderMovement))] 
```

<details><summary>Existing code</summary>

```csharp

public class PlayerController : MonoBehaviour
{
  WalkMovement walkMovement;

  JumpMovement jumpMovement;
```

</details>

```csharp
  LadderMovement ladderMovement; 
```

<details><summary>Existing code</summary>

```csharp
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    jumpMovement = GetComponent<JumpMovement>();
```

</details>

```csharp
    ladderMovement = GetComponent<LadderMovement>(); 
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void FixedUpdate()
  {
    walkMovement.desiredWalkDirection
      = Input.GetAxis("Horizontal");
```

</details>

```csharp
    ladderMovement.desiredClimbDirection 
      = Input.GetAxis("Vertical");
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void Update()
  {
    if(Input.GetButtonDown("Jump"))
    {
      jumpMovement.Jump();
    }
  }
}
```

</details>



<hr></details><br>
<details><summary>TODO</summary>

TODO
Can't go down.
Why does he pop a bit on the way up?

<hr></details>


## 4.9) Disable physics when climbing

While climbing a ladder, disable physics.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**DisablePhysics**:

```csharp
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DisablePhysics : MonoBehaviour
{
  Rigidbody2D myBody;
  List<Collider2D> impactedColliderList;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();

    impactedColliderList = new List<Collider2D>();
    Collider2D[] colliderList = GetComponentsInChildren<Collider2D>();
    for(int i = 0; i < colliderList.Length; i++)
    {
      Collider2D collider = colliderList[i];
      if(collider.isTrigger == false)
      {
        impactedColliderList.Add(collider);
      }
    }

    Debug.Assert(myBody != null);
  }

  protected void OnEnable()
  {
    for(int i = 0; i < impactedColliderList.Count; i++)
    {
      Collider2D collider = impactedColliderList[i];
      collider.isTrigger = true;
      myBody.gravityScale = 0;
    }
  }
   
  protected void OnDisable()
  {
    for(int i = 0; i < impactedColliderList.Count; i++)
    {
      Collider2D collider = impactedColliderList[i];
      collider.isTrigger = false;
      myBody.gravityScale = 1;
    }
  }
}
```

 - Add it to the character, fly guy, and spike ball.
 - Disable the DisablePhysics component on each prefab.
 - Update LadderMovement as follows (or copy paste TODO link):

<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(FloorDetector))]
```

</details>

```csharp
[RequireComponent(typeof(DisablePhysics))]
```

<details><summary>Existing code</summary>

```csharp
public class LadderMovement : MonoBehaviour
{
  [NonSerialized]
  public float desiredClimbDirection;

  public event Action onGettingOnLadder;

  public event Action onGettingOffLadder;

  public bool isOnLadder
  {
    get
    {
      return ladderWeAreOn != null;
    }
  }

  [SerializeField]
  float climbSpeed = 60;

  Rigidbody2D myBody;

  Collider2D myCollider;

  FloorDetector floorDetector;

  GameObject _ladderWeAreOn;

  public GameObject ladderWeAreOn
  {
    get
    {
      return _ladderWeAreOn;
    }
    private set
    {
      if(_ladderWeAreOn == value)
      {
        return;
      }

      _ladderWeAreOn = value;

      if(ladderWeAreOn != null)
      {
        OnGettingOnLadder();
      }
      else
      {
        OnGettingOffLadder();
      }
    }
  }
```

</details>

```csharp
  DisablePhysics disablePhysics; 
```

<details><summary>Existing code</summary>

```csharp
  List<GameObject> currentLadderList;

  protected void Awake()
  {
    currentLadderList = new List<GameObject>();
    myBody = GetComponent<Rigidbody2D>();
    myCollider = GetComponent<Collider2D>();
    floorDetector = GetComponentInChildren<FloorDetector>();
```

</details>

```csharp
    disablePhysics = GetComponent<DisablePhysics>();
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(collision.CompareTag("Ladder") == false)
    {
      return;
    }

    currentLadderList.Add(collision.gameObject);
  }

  protected void OnTriggerExit2D(
    Collider2D collision)
  {
    if(collision.gameObject == ladderWeAreOn)
    {
      GetOffLadder();
    }

    currentLadderList.Remove(collision.gameObject);
  }

  protected void FixedUpdate()
  {
    GameObject ladder = ladderWeAreOn;

    if(ladder == null)
    {
      ladder = FindClosestLadder();
      if(ladder == null)
      {
        return;
      }
    }

    Bounds ladderBounds = ladder.GetComponent<Collider2D>().bounds;
    Bounds entityBounds = myCollider.bounds;

    if(isOnLadder == false
      && Mathf.Abs(desiredClimbDirection) > 0.01
      && IsInBounds(ladderBounds, entityBounds))
    {
      if(
          desiredClimbDirection > 0 
            && entityBounds.min.y < ladderBounds.center.y
          || desiredClimbDirection < 0 
            && entityBounds.min.y > ladderBounds.center.y)
      {
        ladderWeAreOn = ladder;
      }
    }

    if(isOnLadder)
    {
      float currentVerticalVelocity = myBody.velocity.y;
      if(IsInBounds(ladderBounds, entityBounds) == false)
      {
        GetOffLadder();
      }
      else if(floorDetector.distanceToFloor < .3f
        && floorDetector.distanceToFloor > .1f)
      {
        if(currentVerticalVelocity > 0
            && entityBounds.min.y > ladderBounds.center.y)
        {
          GetOffLadder();
        }
        else if(currentVerticalVelocity < 0
          && entityBounds.min.y < ladderBounds.center.y)
        {
          GetOffLadder();
        }
      }

      if(isOnLadder)
      {
        myBody.velocity = new Vector2(myBody.velocity.x,
          desiredClimbDirection * climbSpeed * Time.fixedDeltaTime);
      }
    }
  }

  bool IsInBounds(
    Bounds ladderBounds,
    Bounds entityBounds)
  {
    float entityCenterX = entityBounds.center.x;
    if(ladderBounds.min.x > entityCenterX
      || ladderBounds.max.x < entityCenterX)
    {
      return false;
    }

    float entityFeetY = entityBounds.min.y;
    if(ladderBounds.min.y > entityFeetY
      || ladderBounds.max.y < entityFeetY)
    {
      return false;
    }

    return true;
  }


  public void GetOffLadder()
  {
    ladderWeAreOn = null;
  }

  void OnGettingOnLadder()
  {
```

</details>

```csharp
    disablePhysics.enabled = true; 
```

<details><summary>Existing code</summary>

```csharp
    if(onGettingOnLadder != null)
    {
      onGettingOnLadder();
    }
  }

  void OnGettingOffLadder()
  {
```

</details>

```csharp
    disablePhysics.enabled = false;
```

<details><summary>Existing code</summary>

```csharp

    desiredClimbDirection = 0;

    if(onGettingOffLadder != null)
    {
      onGettingOffLadder();
    }
  }

  GameObject FindClosestLadder()
  {
    if(currentLadderList.Count == 0)
    {
      return null;
    }

    GameObject closestLadder = null;
    float distanceToClosestLadder = 0;
    for(int i = 0; i < currentLadderList.Count; i++)
    {
      GameObject ladder = currentLadderList[i];
      float distanceToLadder = (ladder.transform.position - transform.position).sqrMagnitude;
      if(closestLadder == null)
      {
        closestLadder = ladder;
        distanceToClosestLadder = distanceToLadder;
      }
      else
      {
        if(distanceToLadder < distanceToClosestLadder)
        {
          closestLadder = ladder;
          distanceToClosestLadder = distanceToLadder;
        }
      }
    }

    return closestLadder;
  }
}
```

</details>

<hr></details><br>
<details><summary>TODO</summary>

TODO What do you mean by command pattern?
TODO why?

<hr></details>


## 4.10) Random climb controller

Create a script for the fly guy and spike ball to control when to climb a ladder.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**RandomClimbController**:

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LadderMovement))]
public class RandomClimbController : MonoBehaviour
{
  [SerializeField]
  float oddsOfClimbingLadderUp = .9f;

  [SerializeField]
  float oddsOfClimbingLadderDown = .1f;

  [SerializeField]
  float timeBeforeFirstPossibleClimb = 5;

  [SerializeField]
  float minTimeBetweenReconsideringDirection = 1;

  [SerializeField]
  float maxTimeBetweenReconsideringDirection = 10;

  LadderMovement ladderMovement;

  protected void Awake()
  {
    ladderMovement = GetComponent<LadderMovement>();
  }

  protected void Start()
  {
    StartCoroutine(Wander());
  }

  IEnumerator Wander()
  {
    yield return new WaitForSeconds(timeBeforeFirstPossibleClimb);

    while(true)
    {
      SelectARandomClimbDirection();
      float timeToSleep = UnityEngine.Random.Range(
        minTimeBetweenReconsideringDirection, 
        maxTimeBetweenReconsideringDirection);
      yield return new WaitForSeconds(timeToSleep);
    }
  }

  void SelectARandomClimbDirection()
  {
    if(ladderMovement.isOnLadder == false)
    {
      if(UnityEngine.Random.value <= oddsOfClimbingLadderUp)
      {
        ladderMovement.desiredClimbDirection = 1;
      }
      else if(UnityEngine.Random.value <= oddsOfClimbingLadderDown)
      {
        ladderMovement.desiredClimbDirection = -1;
      }
      else
      {
        ladderMovement.desiredClimbDirection = 0;
      }
    }
  }
}
```

 - Add it to the fly guy and spike ball.
 - On the spike ball, change:
   - Odds of climbing up to 0
   - Odds of climbing down to about .5

<hr></details><br>
<details><summary>TODO</summary>

TODO
They won't actually climb, just go up or down a touch then pop back.

<hr></details>


## 4.11) Stop walking off ladders

Stop WanderWalkController when climbing up or down.

<details><summary>How</summary>

 - Update WanderWalkController as follows (or copy/paste todo link):

<details><summary>Existing code</summary>

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class WanderWalkController : MonoBehaviour
{
  [SerializeField]
  float oddsOfGoingUpHill = .8f; 

  [SerializeField]
  float timeBeforeFirstWander = 10;

  [SerializeField]
  float minTimeBetweenReconsideringDirection = 1;

  [SerializeField]
  float maxTimeBetweenReconsideringDirection = 10;

  WalkMovement walkMovement;

  FloorDetector floorDetector;
```

</details>

```csharp
  LadderMovement ladderMovement; 
```

<details><summary>Existing code</summary>

```csharp
  protected void Awake()
  {
    Debug.Assert(oddsOfGoingUpHill >= 0);
    Debug.Assert(timeBeforeFirstWander >= 0);

    walkMovement = GetComponent<WalkMovement>();
    floorDetector = GetComponentInChildren<FloorDetector>();
```

</details>

```csharp
    ladderMovement = GetComponent<LadderMovement>(); 

    if(ladderMovement != null)
    {
      ladderMovement.onGettingOnLadder 
        += LadderMovement_onGettingOnLadder;
      ladderMovement.onGettingOffLadder 
        += LadderMovement_onGettingOffLadder;
    }
```

<details><summary>Existing code</summary>

```csharp
  }

  protected void Start()
  {
    StartCoroutine(Wander());
  }
```

</details>

```csharp
  void LadderMovement_onGettingOnLadder() 
  {
    walkMovement.desiredWalkDirection = 0;
  }

  void LadderMovement_onGettingOffLadder()
  {
    SelectARandomWalkDirection();
  }
```

<details><summary>Existing code</summary>

```csharp
  IEnumerator Wander()
  {
    walkMovement.desiredWalkDirection = 1;
    yield return new WaitForSeconds(timeBeforeFirstWander);

    while(true)
    {
      SelectARandomWalkDirection();

      float timeToSleep = UnityEngine.Random.Range(
        minTimeBetweenReconsideringDirection,
        maxTimeBetweenReconsideringDirection);
      yield return new WaitForSeconds(timeToSleep);
    }
  }

  void SelectARandomWalkDirection()
  {
```

</details>

```csharp
    if(ladderMovement != null && ladderMovement.isOnLadder) 
    {
      return;
    }
```

<details><summary>Existing code</summary>

```csharp
    float dot;
    if(floorDetector.floorUp != null)
    {
      dot = Vector2.Dot(floorDetector.floorUp.Value, Vector2.right);
    }
    else
    {
      dot = 0;
    }

    if(dot < 0)
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= oddsOfGoingUpHill ? 1 : -1;
    }
    else if(dot > 0)
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= oddsOfGoingUpHill ? -1 : 1;
    }
    else
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value <= .5f ? 1 : -1; 
    }
  }
}
```

</details>



<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>



## 4.12) Stop rolling off ladders

Create a script to stop the ball's momentum when getting on ladders, and restore it when getting off.

<details><summary>How</summary>

 - Create a script Code/Components/Movement/**StopMomentumOnLadder**:

```csharp
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LadderMovement))]
public class StopMomentumOnLadder : MonoBehaviour
{
  Rigidbody2D myBody;

  float previousAngularVelocity;

  float previousXVelocity;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();

    LadderMovement ladderMovement = GetComponent<LadderMovement>();
    ladderMovement.onGettingOffLadder 
      += ClimbLadder_onGettingOffLadder;
    ladderMovement.onGettingOnLadder 
      += LadderMovement_onGettingOnLadder;
  }

  void LadderMovement_onGettingOnLadder()
  {
    previousAngularVelocity = myBody.angularVelocity;
    previousXVelocity = myBody.velocity.x;
    myBody.velocity = Vector2.zero;
  }

  void ClimbLadder_onGettingOffLadder()
  {
    myBody.angularVelocity = -previousAngularVelocity;
    myBody.velocity = new Vector2(-previousXVelocity, myBody.velocity.y);
  }
}
```

 - Add it to the spike ball.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 4.13) Move towards the center of the ladder

Add a script to the fly guy and spike ball to direct them towards the center of a ladder while climbing.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**MoveTowardsCenterWhileClimbing**:

```csharp
using UnityEngine;

[RequireComponent(typeof(LadderMovement))]
public class MoveTowardsCenterWhileClimbing : MonoBehaviour
{
  [SerializeField]
  float lerpSpeed = .1f;

  LadderMovement ladderMovement;

  protected void Awake()
  {
    ladderMovement = GetComponent<LadderMovement>();

    Debug.Assert(ladderMovement != null);
  }

  protected void FixedUpdate()
  {
    GameObject ladder = ladderMovement.ladderWeAreOn;
    if(ladder != null)
    {
      float targetX = ladder.transform.position.x;
      float myX = transform.position.x;
      float deltaX = targetX - myX;
      if(Mathf.Abs(deltaX) > 0.01)
      {
        Vector2 target = transform.position;
        target.x += deltaX;
        transform.position = Vector2.Lerp(
          transform.position, 
          target, 
          lerpSpeed);
      }
    }
  }
}
```

 - Add it to fly guy and spike ball.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## 4.14) Test

TODO

# Next chapter

[Chapter 5](https://github.com/hardlydifficult/Platformer/blob/master/Chapter5.md).
