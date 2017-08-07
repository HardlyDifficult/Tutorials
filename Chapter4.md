# 4) Ground detection & Ladders

TODO

## 4.1) Rotate entities when they walk the other way

Flip the entity when they switch between walking left and right.

<details><summary>How</summary>

 - Create script Components/Movement/**RotateFacingDirection**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotateFacingDirection : MonoBehaviour
{
  Rigidbody2D myBody;

  SpriteRenderer sprite;
  
  bool _isGoingLeft;

  public bool isGoingLeft
  {
    get
    {
      return _isGoingLeft;
    }
    private set
    {
      if(isGoingLeft == value)
      {
        return;
      }

      _isGoingLeft = value;
      sprite.flipX = isGoingLeft;
    }
  }

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    sprite = GetComponentInChildren<SpriteRenderer>();
  }

  protected void FixedUpdate()
  {
    float xVelocity = myBody.velocity.x;
    if(Mathf.Abs(xVelocity) > 0.1)
    {
      isGoingLeft = xVelocity < 0;
    }
  }
}
```

 - Add **RotateFacingDirection** to the character prefab.

<hr></details><br>
<details><summary>What did that do?</summary>

Each FixedUpdate, we determine which direction the entity is walking by its X velocity.  When the direction changes, we flip the sprite so that the character appears to be facing the other way.

<hr></details>
<details><summary>What's a C# smart property?</summary>

In C#, data may be exposed as either a Field or a Property.  Fields are simply data as one would expect.  Properties are accessed in code like a field is, but they are capable of more.

In this example, when isGoingRight changes between true and false, the GameObject's transform is rotated so that the sprite faces the correct direction.  Leveraging the property changing to trigger the rotation change is an example of logic in the property making it 'smart'.

There are pros and cons to smart properties.  For example, one may argue that including the transform change when isGoingRight is modified hides the mechanic and makes the code harder to follow.  There are always alternatives if you prefer to not use smart properties.  For example:

```csharp
bool isGoingLeftNow = xVelocity <> 0;
if(isGoingLeft != isGoingLeftNow) 
{
  sprite.flipX = isGoingLeft;
  isGoingLeft = isGoingLeftNow;
}
```

</details>
<details><summary>Why not compare to 0 when checking if there is no movement?</summary>

In Unity, numbers are represented with the float data type.  Float is a way of representing decimal numbers but is a not precise representation like you may expect.  When you set a float to some value, internally it may be rounded ever so slightly.

The rounding that happens with floats allows operations on floats to be executed very quickly.  However it means we should never look for exact values when comparing floats, as a tiny rounding issue may lead to the numbers not being equal.

In the example above, as the velocity approaches zero, the significance of if the value is positive or negative, is lost.  It's possible that if we were to compare to 0 that at times the float may oscillate between a tiny negative value and a tiny positive value causing the sprite to flip back and forth.

</details>


## 4.2) Detect floors

Create a script to calculate the distance to and rotation of the floor under an entity.

<details><summary>How</summary>

 - Create a layer 'Floor'.
 - Select all the Platform GameObjects and change to Layer Floor.
   - When prompted, select 'No, this object only'.
 - Create script Components/Movement/**FloorDetector**:

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloorDetector : MonoBehaviour
{
  static readonly Quaternion backwardsRotation
    = Quaternion.Euler(0, 0, 180);

  Collider2D myCollider;

  [SerializeField]
  ContactFilter2D floorFilter;

  Collider2D[] possibleCollisionResultList = new Collider2D[3];
  
  public Bounds feetBounds
  {
    get
    {
      return myCollider.bounds;
    }
  }

  public bool isTouchingFloor
  {
    get; private set;
  }

  public float? distanceToFloor
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

  protected void Awake()
  {
    myCollider = GetComponent<Collider2D>();
  }

  protected void FixedUpdate()
  {
    Collider2D floorWeAreStandingOn = DetectTheFloorWeAreStandingOn();
    isTouchingFloor = floorWeAreStandingOn != null;

    if(floorWeAreStandingOn != null)
    {
      CalculateFloorRotation(floorWeAreStandingOn);
      distanceToFloor = 0;
    }
    else
    {
      floorUp = null;
      floorRotation = null;
      Collider2D floorUnderUs = DetectFloorUnderUs();
      if(floorUnderUs != null)
      {
        distanceToFloor = CalculateDistanceToFloor(floorUnderUs);
      }
      else
      {
        distanceToFloor = null;
      }
    }
  }

  float CalculateDistanceToFloor(
    Collider2D floorUnderUs)
  {
    float yOfTopOfFloor = floorUnderUs.bounds.max.y;

    if(floorUnderUs is BoxCollider2D)
    {
      BoxCollider2D boxCollider = (BoxCollider2D)floorUnderUs;
      yOfTopOfFloor += boxCollider.edgeRadius;
    }

    return myCollider.bounds.min.y - yOfTopOfFloor;
  }

  static void CalculateFloorRotation(
    Collider2D floorWeAreStandingOn)
  {
    floorUp = floorWeAreStandingOn.transform.up;
    floorRotation = floorWeAreStandingOn.transform.rotation;
    if(Vector2.Dot(Vector2.up, floorUp.Value) < 0)
    {
      floorUp = -floorUp;
      floorRotation *= backwardsRotation;
    }
  }

  Collider2D DetectFloorUnderUs()
  {
    RaycastHit2D[] result = new RaycastHit2D[1];
    if(Physics2D.Raycast(
      transform.position, 
      Vector2.down,
      floorFilter, 
      result) > 0)
    {
      return result[0].collider;
    }

    return null;
  }

  Collider2D DetectTheFloorWeAreStandingOn()
  {
    int foundColliderCount
      = Physics2D.OverlapCollider(myCollider, floorFilter, possibleCollisionResultList);

    for(int i = 0; i < foundColliderCount; i++)
    {
      Collider2D collider = possibleCollisionResultList[i];
      ColliderDistance2D distance = collider.Distance(myCollider);

      if(distance.distance >= -.1f
        && Vector2.Dot(Vector2.up, distance.normal) > 0)
      {
        return collider;
      }
    }

    return null;
  }
}
```

 - Add **FloorDetector** to:
   - The Character prefab.
   - The Spike Ball prefab.
   - The Fly Guy's Feet child GameObject.
 - For each of those FloorDetector components, update the Floor Filter:
     - Check Use Layer Mask
     - Layer Mask: Floor

<hr></details><br>
<details><summary>What did that do?</summary>

The FloorDetector collects information about the floor under the entity for other components to leverage:

 - feetYPosition: The y position of the bottom of the entity's feet.
 - isTouchingFloor: True if the entity is currently on the ground vs jumping or falling.
 - floorUp: the normal of the floor the entity is standing on, or the direction perpendicular to the floor.
 - floorRotation: the rotation of the floor the entity is standing on.
 - distanceToFloor: how far above the floor the entity's feet currently are.  0 if isTouchingFloor.

Each FixedUpdate, we use OverlapCollider to find the floor we may be standing on.  We check multiple results and filter out instances which are overlapping the bottom of a platform (necessary because of the one-way platforms), if any remain - the closest is the floor we are on.

If we are standing on a floor we then get rotation information.  If the floor is upside down, we flip these stats as well.

If we are not standing on a floor, we Raycast below the entity to get the distanceToFloor.

<hr></details>
<details><summary>What's a C# Nullable type / what's the question mark after 'float'?</summary>

Structs in C# must have a value (as opposed to classes which may have a value or be null).  Sometimes this is limiting and another piece of information is required.  

Nullable types in C# are a feature which allows you to add one more possible value to any struct, by adding a question mark after the type. For example:

```csharp
bool? trueFalseOrNull;
trueFalseOrNull = null;
trueFalseOrNull = true;
trueFalseOrNull = false;
```

Often nullable types are used to indicate an error state or that no valid information is available.  Without the nullable feature, you may have implemented the same using another variable to indicate the state - or by using a magic number.

<hr></details>
<details><summary>What's C# 'is' do and how's it differ from 'as'?</summary>

In C#, 'is' may be used to check if an object is compatible with a given type - i.e. if a cast to that type would be successful.  For example:

```csharp
Collider2D floorUnderUs;
...
if(floorUnderUs is BoxCollider2D) 
{
  BoxCollider2D boxCollider = (BoxCollider2D)floorUnderUs;
  ...
}
```

'as' is a similar feature where instead of returning true or false, it returns null or the casted value.  For example:

```csharp
Collider2D floorUnderUs;
...
BoxCollider2D boxCollider = floorUnderUs as BoxCollider2D;
if(boxCollider != null) 
{
  ...
}
```

<hr></details>
<details><summary>What's Dot product do?</summary>

The Dot product is a fast operation which can be used to efficiently determine if two directions represented with Vectors are facing the same (or a similar) way.

In the visualization below, we are rotating two ugly arrows.  These arrows are pointing in a direction and we are using Vector2.Dot to compare those two directions.  The Dot product is shown as we rotate around.

<img src="http://i.imgur.com/XrjcWQm.gif" width=200px />

A few notables about Dot products:

 - '1' means the two directions are facing the same way.
 - '-1' means the two directions are facing opposite ways.
 - '0' means the two directions are perpendicular.
 - Numbers smoothly transition between these points, so .9 means that the two directions are nearly identical.
 - When two directions are not the same, the Dot product will not tell you which direction an object should rotate in order to make them align - it only informs you about how similar they are at the moment.  

For this visualization, we are calculating the Dot product like so:

```csharp
Vector2.Dot(gameObjectAToWatch.transform.up, gameObjectBToWatch.transform.up);
```

<hr></details>
<details><summary>When do you use OverlapCollider vs Raycast vs Distance vs Trigger*?</summary>

Unity offers a number of APIs for getting information about objects around you.  They are optimized for different use cases, and often you could have accomplished the same mechanic using a different API.

Until now in this tutorial we have been using Trigger* events (e.g. OnTriggerEnter2D).  These events push information to your script to react to.  Sometimes, like here, it's easier to pull the information.

We are using 3 different APIs to pull information in this script:

 - OverlapCollider returns the colliders which are touching this entity's collider.
 - Raycast projects a line and returns colliders intersecting with it (in order, closest first).  There are other 'cast' calls to project different shapes when needed, e.g. BoxCast.
 - collider.Distance returns precise information about the collision between two specific colliders, such as the contact point or if they are not touching the distance between them.

<hr></details>
<details><summary>Why add the edge radius to bounds max when calculating the floor's position?</summary>

When edge radius is used on a BoxCollider, the collider bounds represents the inner square of the collider (the size before edge is consider).  So in order to get the correct position we must add the edge radius in as well.

<hr></details>


## 4.3) Prevent double jump

Update JumpMovement to prevent double jump and flying (by spamming space), by leveraging the FloorDetector just created.

<details><summary>How</summary>

 - Update Components/Movement/**JumpMovement**:

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
<details><summary>What did that do?</summary>

We are leveraging the FloorDetector component in order to prevent jumps when the character is not touching the floor.

<hr></details>
<details><summary>Why not use a cooldown instead?</summary>

You may consider using a cooldown by time instead.  This would create a different play experience, and if the cooldown is short the player may be able to double jump (but not fly by spamming space).

You might also want both a cooldown and the floor detection.  Small changes to mechanics like this can change how the game feels while playing.

<hr></details>


## 4.4) Update wander to prefer traveling up hill

Update the WanderWalkController so that the fly guy is more likely to walk up hill than down.

<details><summary>How</summary>

 - Update Components/Movement/**WanderWalkController**:

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
    if(timeBeforeFirstWander > 0) 
    {
      yield return new WaitForSeconds(timeBeforeFirstWander);
    }

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
<details><summary>What did that do?</summary>

Leveraging the FloorDetector, we give the fly guy better odds at walking up a platform vs walking down one.  Without this component the fly guy enemies may collect at the bottom of the level - this keeps them mostly moving forward/up while still using RNG to keep the player on their toes.

<hr></details>
<details><summary>Why take the Dot product with Vector2.right?</summary>

Dot product is used to determine if two directions are pointing the same way.  We compare the floor's up direction (or its normal) to the world right.  If the dot product is positive then we know that the platform is traveling down and to the right; if negative the platform is down and to the left; and it would be 0 if the platform were flat.

<hr></details>


## 4.5) Rotate so feet are flat on the floor

Create a script to rotate an entity, aligning with the floor when touching one, otherwise rotating back to the default position.

<details><summary>How</summary>

 - Create script Components/Movement/**RotateToAlignWithFloor**:

```csharp
using UnityEngine;

public class RotateToAlignWithFloor : MonoBehaviour
{
  [SerializeField]
  float lerpSpeedToFloor = .4f;

  [SerializeField]
  float lerpSpeedWhileInAir = .05f;

  FloorDetector floorDetector;

  protected void Awake()
  {
    floorDetector
      = GetComponentInChildren<FloorDetector>();
  }

  protected void Update()
  {
    Quaternion rotation;
    float speed;
    if(floorDetector.floorRotation != null)
    {
      rotation = floorDetector.floorRotation.Value;
      speed = lerpSpeedToFloor;
    }
    else
    {
      rotation = Quaternion.identity;
      speed = lerpSpeedWhileInAir;
    }

    transform.rotation = Quaternion.Lerp(
      transform.rotation,
      rotation,
      speed * Time.deltaTime);
  }
}
```

 - Add **RotateToAlignWithFloor** to the Character and Fly Guy prefabs.

<hr></details><br>
<details><summary>What did that do?</summary>

When the entity is standing on a floor, we gradually rotate it so its feet are flat on the floor.  When jumping or falling, we slowly rotate back to facing straight up.

<hr></details>
<details><summary>What's 'Lerp' and how's it compare to 'Slerp'?</summary>

Lerp, or **l**inear int**erp**olation, is a fancy term for a simple concept.  Draw a line between two points and travel a certain percent along that path, returning the position you end on.  For example:

```csharp
void Start()
{
  Vector2 a = new Vector2(1, 5);
  Vector2 b = new Vector2(4, 11);
  Vector2 c = Vector2.Lerp(a, b, 1/3f);
  print(c); // == (2, 7)
}
```

Slerp, or **s**pherical **l**inear int**erp**olation, is similar to lerp but the change in position accelerates at the beginning and decelerates towards the end.  It's called spherical because it is following the path of a half circle instead of a straight line.

Here you can see lerp vs slerp with only position X changing (the large balls), and change X and Y.  All are moving given the same % progress.  Notice how the movement for slerp at beginning and end are traveling at a different speed than the lerp - but the positions match exactly at the start, middle, and end.

<img src="http://i.imgur.com/RiO7J0l.gif" width=300px />

<hr></details>


## 4.6) Add ladders to the world

Create GameObjects and layout ladders in the world.  Set their tag to Ladder.  

Add a BoxCollider2D to each of the ladders and size it to use for climbing and set it as a trigger collider. An entity will be able climb ladders when its bottom is above the bottom of the ladder's collider and its center is inside.

<details><summary>How</summary>

Layout ladders:

 - Create a parent Ladder GameObject, add the ladder sprite(s).  We are using **spritesheet_tiles_23** and **33**.
   - Order in Layer: -2.
 - Position the ladder and repeat, creating several ladders - some which look broken:
   - The child sprite GameObjects should have a default Transform, with the exception of the Y position when multiple sprites are used.
   - It usually looks fine to overlap sprites a bit, as we do to get the space between ladder steps looking good.

<img src="http://i.imgur.com/u299hoi.gif" width=500px />

 - Create a layer for "Ladder".
 - Select all the ladder GameObjects:
   - Change their layer to Ladder.
   - Add **FadeInThenEnable** to all the ladders.
 - Create a new parent GameObject to hold all the ladders.

<br>Add trigger colliders to the ladders:

 - Select all the Ladder GameObjects:
   - Add **BoxCollider2D**:
     - Check 'Is Trigger'.
     - Size it such that: 
       - The width to be thinner than the sprite (about .6).
       - The bottom of the collider is:
         - Just below the platform for complete ladders.
         - Aligned with the last step of broken ladders.
       - The top of the collider is just above the upper platform.

<img src="http://i.imgur.com/r0k4eq3.png" width=150px />


<hr></details>
<details><summary>What did that do?</summary>

Layout ladders:

Sprites are added for the ladders with a negative Order in Layer so it appears behind the platforms and entities.  A layer is created allowing us to identify the collisions with ladders later on.  Like the Hammers, ladders fade in at the start of the level.

<br>Add trigger colliders to the ladders:

We are using trigger colliders to define the area of a ladder that entities may climb.  For example, we made the collider thinner than the ladder itself so that entities cannot climb the edges (which may look strange.)  

<hr></details>


## 4.8) Add a script to climb ladders

Create a script to climb ladders for all entities to use, and update the player controller to match.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**LadderMovement**:

```csharp
using System;
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

  [SerializeField]
  ContactFilter2D ladderFilter;

  FloorDetector floorDetector;

  GameObject _ladderWeAreOn;

  Collider2D[] tempColliderList = new Collider2D[3];

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

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    myCollider = GetComponent<Collider2D>();
    floorDetector = GetComponentInChildren<FloorDetector>();
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
    Bounds entityBounds = floorDetector.feetBounds;

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

  GameObject FindClosestLadder()
  {
    int resultCount = myCollider.OverlapCollider(ladderFilter, tempColliderList);

    GameObject closestLadder = null;
    float distanceToClosestLadder = 0;
    for(int i = 0; i < resultCount; i++)
    {
      GameObject ladder = tempColliderList[i].gameObject;
      Vector2 delta = ladder.transform.position - transform.position;
      float distanceToLadder = delta.sqrMagnitude;
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

 - Add **LadderMovement** to the Character, Fly Guy, and Spike Ball.
 - Update Components/Controllers/**PlayerController**:

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
<details><summary>What did that do?</summary>

LadderMovement will climb up or down a ladder, given input from a controller (via desiredClimbDirection).  The PlayerController was updated to read up/down movement and feed that to the LadderMovement component.

LadderMovement offers the following APIs for other components:

 - isOnLadder
 - ladderWeAreOn
 - An event for when the entity first gets on a ladder and when they get off.

Each FixedUpdate, we get on a ladder nearby if we are in bounds and there is desired movement in the correct direction (i.e. we can't walk down starting at the bottom of a ladder).  

Once on a ladder, LadderMovement will hold the entity's y position by controlling its y velocity.

Note there are some issues at the moment - you can't go down a ladder and on the way up the entity may pop a bit.  Both fixed in the next section.

<hr></details>
<details><summary>Why use sqrMagnitude instead of magnitude?</summary>

In this example both magnitude and sqrMagnitude would give us the same result, as is the case anytime we are comparing if one distance is greater or less than another.  sqrMagnitude executes much faster, so its preferred anytime you do not require the precision that magnitude gives you.

To calculate magnitude, you first calculate the squared magnitude and then take the square root.  Taking the square root is a difficult operation.

</details>

## 4.9) Disable physics when climbing

While climbing a ladder disable physics, allowing entities to climb down.

<details><summary>How</summary>

 - Create script Components/Movement/**DisablePhysics**:

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

 - Add **DisablePhysics** to the Character, Fly Guy, and Spike Ball.
   - Disable the DisablePhysics component on each prefab.
 - Update Components/Movement/**LadderMovement**:

<details><summary>Existing code</summary>

```csharp
using System;
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

  [SerializeField]
  ContactFilter2D ladderFilter;

  FloorDetector floorDetector;

  GameObject _ladderWeAreOn;

  Collider2D[] tempColliderList = new Collider2D[3];

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
    Bounds entityBounds = floorDetector.feetBounds;

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

  GameObject FindClosestLadder()
  {
    int resultCount = myCollider.OverlapCollider(ladderFilter, tempColliderList);
    
    GameObject closestLadder = null;
    float distanceToClosestLadder = 0;
    for(int i = 0; i < resultCount; i++)
    {
      GameObject ladder = tempColliderList[i].gameObject;
      Vector2 delta = ladder.transform.position - transform.position;
      float distanceToLadder = delta.sqrMagnitude;
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
<details><summary>What did that do?</summary>

We disable physics (collisions and gravity) when getting on a ladder, and enable physics again when we get off.

The DisablePhysics component will disable collisions (by switching to trigger) and gravity (by setting gravityScale to 0) when enabled, and then restores the original values when disabled.

LadderMovement was updated to enable the DisablePhysics component when getting on ladders, and disable it when getting off.  The language here is confusing - but again enabling the DisablePhysics component turns off physics.

<hr></details>
<details><summary>What's a C# List?</summary>

In C#, a List is a an array which can easily and automatically be resized as needed.  As you add and remove elements, C# will manage the size of the array which holds the information.  It does not resize the array every time something is added or removed, it's optimized to try and limit those potentially expensive calls.  

When you create a List you give it the type of data it will contain.  We make the List for a specific type, as opposed to using objects, to communicate intent and for type safety - e.g. if we had a List<Dog> it's clear that Cats don't belong there, and if we attempted to add a Cat to the Dog list, C# would throw an error.

<hr></details>
<details><summary>What's rigidbody gravityScale do?</summary>

You can modify how much gravity impacts a specific object using its rigidbody's gravityScale.  Gravity scale is defined in percent, where 1 is the normal amount of gravity and 0 means gravity is disabled.

You can modify the gravity for all objects in the world using Project Settings -> Physics 2D -> Gravity, it defaults to (0, -9.81).

<hr></details>
<details><summary>Why store the impacted collider list?</summary>

This component is disabling all colliders on the GameObject which were not already triggers.  When we undo this change, we don't have a way to detect the colliders original state.  We store list so we can change those colliders to not triggers anymore, without unintentionally changing a collider which is always supposed to be a trigger.

<hr></details>


## 4.10) Random climb controller

Create a script for the fly guy and spike ball to control when to climb a ladder.

<details><summary>How</summary>

 - Create script Components/Movement/**RandomClimbController**:

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

 - Add **RandomClimbController** to the Fly Guy and Spike Ball.
 - On the Spike Ball, change:
   - Odds of climbing up: 0
   - Odds of climbing down: .5

<hr></details><br>
<details><summary>What did that do?</summary>

Not much yet.

This script will get the fly guy enemies to randomly climb up or down ladders, and the spike balls will randomly climb down.  The problem is they are still walking or rolling, so they quickly get off the ladder and then pop back on top of the platform.

This works by periodically picking a random desired climb direction on the LadderMovement component.  LadderMovement will not do anything with this input until the enemy is positioned on a ladder to climb.

<hr></details>
<details><summary>If I set both odds to 50%, why does it go up more often then down?</summary>

In order to keep the implementation simple, we are checking if we should go up before checking if we should go down.  This order results in effectively lowering the odds for going down.

For example, if both odds were 50%:
 - We have a 50% chance of going up.
 - If not, then we have a 50% chance to go down.

Since we only consider going down when we are not going up, the actual odds of going down in this example are 25%.

You could update this algorithm to calculate the odds correctly.

<hr></details>


## 4.11) Stop walking off ladders

Stop WanderWalkController when climbing up or down.

<details><summary>How</summary>

 - Update Components/Movement/**WanderWalkController**:

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
    if(timeBeforeFirstWander > 0) 
    {
      yield return new WaitForSeconds(timeBeforeFirstWander);
    }

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
<details><summary>What did that do?</summary>

This change prevents the fly guy from walking while on a ladder.  Fly guys will never stop moving in this game, they will walk constantly and when reaching a ladder they may climb straight up or straight down - and then resume walking.

<hr></details>
<details><summary>Why not stop the WalkMovement component instead?</summary>

Stopping the fly guy via the WalkMovement component instead of the WanderWalkController would work fine for the fly guy.  However we share the WalkMovement component with the Character as well, and don't want to prevent the player from being able to walk off the side of a ladder.

You could alternatively put this logic in WalkMovement with a flag to indicate if ladders should prevent walking or not.

<hr></details>
<details><summary>Why not deregister events here?</summary>

We are assuming that this component will never be removed from the GameObject.  So both WanderWalkController and WalkMovement are expected to exist from Awake till OnDestroy.  When a GameObject is destroyed, the registered events are automatically garbage collected.

If we wanted to optionally remove this component, we would want to deregister the events to prevent a memory leak or unexpected behaviour.

<hr></details>
<details><summary>Why not stop and restart the coroutine instead?</summary>

You could stop the coroutine when getting on a ladder and then restart it when you get off.  The coroutine from WanderWalkController would need to be updated for this to work, ensuring that when we resume we don't sleep for that initial wait time again.

<hr></details>


## 4.12) Stop rolling off ladders

Create a script to stop the ball's momentum when getting on ladders, and restore it when getting off.

<details><summary>How</summary>

 - Create a script Components/Movement/**StopMomentumOnLadder**:

```csharp
using UnityEngine;

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

 - Add **StopMomentumOnLadder** to the Spike Ball.

<hr></details><br>
<details><summary>What did that do?</summary>

When a spike ball gets on a ladder, we store its velocity (i.e. speed) and angular velocity (i.e. spin) and then set both to 0.  This stops momentum the ball had from rolling down platforms, allowing it to climb straight up or down the ladder.  

Once done climbing, we restore the momentum, but flip both values so that after getting off the ball is rolling in the opposite direction.

<hr></details>


## 4.13) Move towards the center of the ladder

Add a script to the fly guy and spike ball to direct them towards the center of a ladder while climbing.

<details><summary>How</summary>

 - Create script Components/Movement/**MoveTowardsCenterWhileClimbing**:

```csharp
using UnityEngine;

[RequireComponent(typeof(LadderMovement))]
public class MoveTowardsCenterWhileClimbing : MonoBehaviour
{
  [SerializeField]
  float speed = 1f;

  LadderMovement ladderMovement;

  protected void Awake()
  {
    ladderMovement = GetComponent<LadderMovement>();
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
        transform.position = Vector2.MoveTowards(
          transform.position, 
          target, 
          speed * Time.fixedDeltaTime);
      }
    }
  }
}
```

 - Add **MoveTowardsCenterWhileClimbing** to the Fly Guy and Spike Ball.

<hr></details><br>
<details><summary>What did that do?</summary>

Anytime an entity with this component is climbing a ladder, it will slowly move towards the center.  We use this on enemies because they will typically get on a ladder as soon as it is within range - but it looks better when they climb up/down the center instead of towards the edge.  

<hr></details>
<details><summary>Why not use velocity to move?</summary>

You could.  

MoveTowardsCenterWhileClimbing uses MoveTowards to update the transform.position directly instead of moving via the rigidbody as you normally would.  We do this as a simplification.  

If you use velocity, be careful when you overshoot the target a bit so the entity does not appear to wiggle back and forth trying to settle on the exact center position.

<hr></details>

## 4.14) Test

TODO

Debugging

 - If you can jump and start climbing a ladder you barely reach, check the contact filter configuration.

# Next chapter

[Chapter 5](https://github.com/hardlydifficult/Platformer/blob/master/Chapter5.md).
