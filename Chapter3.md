# 3) Advanced scripting

In chapter 3, TODO intro...

This assumes you completed chapter 2, or you can download the project so far. (TODO link)

TODO tutorial video link

TODO gif

demo build of level 3

## Spawn a flying enemy

Create a prefab for the fly guy reusing components from the spike ball and character.  Create a second spawner at the bottom for fly guys.

<details><summary>How</summary>

 - Select **spritesheet_jumper_30**, **84**, and **90** and drag them into the Hierarchy, creating Assets/Animations/**FlyGuyWalk**.
 - Set Order in Layer to 1.
 - Add it to a parent GameObject named "FlyGuy".
 - Set the Layer for FlyGuy to 'Enemy'.
 - Add a Rigidbody2D and freeze the Z rotation.
 - Add a CapsuleCollider2D and adjust the size.

<img src="http://i.imgur.com/d1lxoEj.png" width=150px />
 
 - Add WalkMovement.
 - Add KillOnContactWith and set the layermask to Player.
 - Drag in **spritesheet_tiles_43** and then drag in **47**, add them to a parent named "Door".
 - Set Order in Layer to -2.
 - Scale up the size of the door to about (1.5, 1.5, 1.5).
 - Move the door to the bottom left of the level and position its Y so that the midpoint of the Door approximitally aligns with the midpoint of the FlyGuy (at the height we would want it to spawn).

<img src="http://i.imgur.com/EjVJkZ4.gif" width=300px />

 - Move the sprite for the top into position, then vertex snap the bottom.

<img src="http://i.imgur.com/SF57oFs.gif" width=150px />

 - Create a prefab for 'FlyGuy' and delete the GameObject.
 - Add Spawner to the door and assign FlyGuy as the thing to spawn.
 - Change the initial wait time to 10.

</details>


## Make the fly guy walk

Add a script to the fly guy to drive random walk movement.

<details><summary>How</summary>

 - Create script Code/Compenents/Movement/**WanderWalkController**:

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class WanderWalkController : MonoBehaviour
{
  [SerializeField]
  float timeBeforeFirstWander = 5;

  [SerializeField]
  float minTimeBetweenReconsideringDirection = 1;

  [SerializeField]
  float maxTimeBetweenReconsideringDirection = 10;

  WalkMovement walkMovement;

  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
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
    walkMovement.desiredWalkDirection
      = UnityEngine.Random.value >= .5f ? 1 : -1;
  }
}
```

 - Add WanderWalkController to the FlyGuy prefab.

</details>


## Flying feet

Add a second collider so that the body of this entity is above the ground but does not kill a character walking underneath.

<details><summary>How</summary>

 - Drag the prefab into the Hierarchy so we can edit the GameObject.
 - Add an empty GameObject as a child under the FlyGuy.  Name it "Feet".
 - Create a Layer for "Feet" and assign it to the Feet GameObject.
 - Update the Physics 2D collision matrix to disable Feet / Player, Feet / Enemy, and Feet / Feet collisions.
 - Add a CircleCollider2D to the Feet and size and position it below the body.

<img src="http://i.imgur.com/VMPqiFE.png" width=300px />

 - Apply changes to the prefab and delete the GameObject.

<img src="http://i.imgur.com/vJFzcOk.png" width=300px />


<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## Fade in entities

Add a script to entities so they fade in before moving.

<details><summary>How</summary>

 - Create a script **SpriteExtensions** under Assets/Code/Utils and paste the following:

```csharp
using UnityEngine;

public static class SpriteExtensions
{
  public static void SetColor(
    this SpriteRenderer[] spriteList,
    Color color)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      sprite.color = color;
    }
  }

  public static void SetAlpha(
    this SpriteRenderer[] spriteList,
    float alpha)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      Color originalColor = sprite.color;
      sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
  }
}
```

 - Create a script **FadeInThenEnable** under Assets/Code/Compenents/Life and paste the following:

```csharp
using System.Collections;
using UnityEngine;

public class FadeInThenEnable : MonoBehaviour
{
  [SerializeField]
  float timeTillEnabled;

  [SerializeField]
  MonoBehaviour[] componentsToEnable;

  protected void Start()
  {
    StartCoroutine(FadeIn());
  }

  IEnumerator FadeIn()
  {
    SpriteRenderer[] spriteList
      = gameObject.GetComponentsInChildren<SpriteRenderer>();

    float timePassed = 0;
    while(timePassed < timeTillEnabled)
    {
      float percentComplete = timePassed / timeTillEnabled;
      spriteList.SetAlpha(percentComplete);

      yield return 0;

      timePassed += Time.deltaTime;
    }

    for(int i = 0; i < componentsToEnable.Length; i++)
    {
      MonoBehaviour component = componentsToEnable[i];
      component.enabled = true;
    }
  }
}
```

 - Add FadeInThenEnable to the character.
 - Disable the character's PlayerController.

<img src="http://i.imgur.com/5WtzPmv.png" width=300px />

 - Configure FadeInThenEnable:
   - Add 1 entry to the Components to Enable list.
   - Drag/drop the PlayerController into the list.

<img src="http://i.imgur.com/hrXMt1f.gif" width=300px />

 - Add FadeInThenEnable to the fly guy prefab, disabling the WanderWalkController and assigning that to the Components to Enable list.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>



## Rotate entities when they walk the other way

Flip the entity when they switch between walking left and walking right.

<details><summary>How</summary>

 - Create a script **RotateFacingDirection** under Assets/Code/Compenents/Movement and paste the following:

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

 - Add RotateFacingDirection to the character and the fly guy prefab.

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


## Create a GameController script

Create a singleton GameController to track points, lives, and hold global data such as the world size.

<details><summary>How</summary>

 - Create a script **GameController** under Assets/Code/Compenents/Controllers and paste the following:

```csharp
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  public static GameController instance;

  public event Action onLifeCounterChange;

  [SerializeField] 
  int _lifeCounter = 3; 

  public int lifeCounter
  {
    get
    {
      return _lifeCounter;
    }
    set
    {
      _lifeCounter = value;
      if(onLifeCounterChange != null)
      {
        onLifeCounterChange();
      }
    }
  }

  int originalLifeCount;

  public int points;

  public Bounds screenBounds
  {
    get; private set;
  }

  protected void Awake()
  {
    Debug.Assert(lifeCounter > 0);

    if(instance != null)
    {
      Destroy(gameObject);
      return;
    }

    instance = this;
    originalLifeCount = lifeCounter;

    Camera camera = Camera.main;
    Vector2 screenSize = new Vector2(
      (float)Screen.width / Screen.height,
      1);
    screenSize *= camera.orthographicSize * 2;
    screenBounds = new Bounds(
      (Vector2)camera.transform.position,
      screenSize);

    DontDestroyOnLoad(gameObject);

    SceneManager.sceneLoaded += SceneManager_sceneLoaded;

    Debug.Assert(originalLifeCount > 0);
    Debug.Assert(screenBounds.size.magnitude > 0);
  }

  void SceneManager_sceneLoaded(
    Scene scene,
    LoadSceneMode loadMode)
  {
    if(scene.name == "MainMenu")
    {
      Reset();
    }
  }

  void Reset()
  {
    lifeCounter = originalLifeCount;
    points = 0;

    Debug.Assert(lifeCounter > 0);
  }
}
```

  - Create a new GameObject named "GameController" and add the 'GameController' component.

<hr></details><br>
<details><summary>TODO</summary>

TODO

Moves the GameObject to a DontDestroyOnLoad section in the Hierarchy.

<hr></details>


## Death effect to decrement lives

Add a script to the character to decrement lives in the GameController on death.

<details><summary>How</summary>

 - Create a script **DeathEffectDecrementLives** under Assets/Code/Compenents/Death and paste the following:

```csharp
public class DeathEffectDecrementLives : DeathEffect
{
  public override float timeUntilObjectMayBeDestroyed
  {
    get
    {
      return 0;
    }
  }

  public override void PlayDeathEffects()
  {
    GameController.instance.lifeCounter--;
  }
}
```

 - Add it to the character.

<hr></details><br>
<details><summary>TODO</summary>

To test, look at the life count go down in the GameController component.

<hr></details>


## Respawn on death

Add scripts to respawn the character when he dies.

<details><summary>How</summary>

 - Create a script **ICareWhenPlayerDies** under Assets/Code/Compenents/Death and paste the following:

```csharp
public interface ICareWhenPlayerDies
{
  void OnPlayerDeath();
}
```
 - Create a script **LevelManager** under Assets/Code/Compenents/Controllers and paste the following:

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;
  
  protected void Start()
  {
    GameController.instance.onLifeCounterChange 
      += Instance_onLifeCounterChange;
    Instantiate(playerPrefab);
  }

  protected void OnDestroy()
  {
    GameController.instance.onLifeCounterChange 
      -= Instance_onLifeCounterChange;
  }

  void Instance_onLifeCounterChange()
  {
    if(isGameOver)
    {
      return;
    }

    if(GameController.instance.lifeCounter <= 0)
    {
      isGameOver = true;
      YouLose();
    }
    else
    {
      RestartLevel();
    }
  }

  public virtual void YouWin()
  {
    if(isGameOver == true)
    { 
      return;
    }

    isGameOver = true;
  }

  void RestartLevel()
  {
    GameObject[] gameObjectList = GameObject.FindObjectsOfType<GameObject>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      ICareWhenPlayerDies[] careList = gameObjectList[i].GetComponents<ICareWhenPlayerDies>();
      for(int j = 0; j < careList.Length; j++)
      {
        ICareWhenPlayerDies care = careList[j];
        care.OnPlayerDeath();
      }
    }
    Instantiate(playerPrefab);
  }
  
  void YouLose()
  {
    // TODO
  }
}
```

 - Add GameObject for LevelManager.
 - Position the character over the door.
 - Create a prefab for the character, and delete the GameObject.
 - In the LevelManager, assign the character prefab.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## Clear and restart the level on death

Add scripts to kill all the enemies and restart spawners when the character dies.

<details><summary>How</summary>

 - Create a script **SuicideWhenPlayerDies** under Assets/Code/Utils and paste the following:

```csharp
using UnityEngine;

public class SuicideWhenPlayerDies : MonoBehaviour, ICareWhenPlayerDies
{
  void ICareWhenPlayerDies.OnPlayerDeath()
  { 
    Destroy(gameObject);
  }
}
```

 - Add SuicideWhenPlayerDies to the fly guy and the spike ball prefabs.
 - Update the 'Spawner' script as follows (or copy/paste TODO link):

<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Instantiates a prefab at this object's location 
/// periodically.
/// </summary>
public class Spawner : MonoBehaviour
```

</details>

```csharp
  , ICareWhenPlayerDies 
```

<details><summary>Existing code</summary>

```csharp
{
  [SerializeField]
  GameObject thingToSpawn;

  [SerializeField]
  float initialWaitTime = 2;

  [SerializeField]
  float minTimeBetweenSpawns = .5f;

  [SerializeField]
  float maxTimeBetweenSpawns = 10;

  protected void Start()
  {
    StartCoroutine(SpawnEnemies());
  }
```

</details>

```csharp
  void ICareWhenPlayerDies.OnPlayerDeath() 
  {
    StopAllCoroutines();
    StartCoroutine(SpawnEnemies());
  }
```

<details><summary>Existing code</summary>

```csharp
  IEnumerator SpawnEnemies()
  {
    Debug.Assert(thingToSpawn != null);
    Debug.Assert(initialWaitTime >= 0);
    Debug.Assert(minTimeBetweenSpawns >= 0);
    Debug.Assert(
      maxTimeBetweenSpawns >= minTimeBetweenSpawns);

    yield return new WaitForSeconds(initialWaitTime);

    while(true)
    {
      Instantiate(
        thingToSpawn, 
        transform.position, 
        Quaternion.identity);

      float sleepTime = UnityEngine.Random.Range(
        minTimeBetweenSpawns, 
        maxTimeBetweenSpawns);
      yield return new WaitForSeconds(sleepTime);
    }
  }
}
```

</details>

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>



## Restrict movement to stay on screen

Create a script which ensures entities can not walk off screen.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**KeepOnScreen**:

```csharp
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KeepOnScreen : MonoBehaviour
{
  Rigidbody2D myBody;

  public event Action onAttemptToLeaveScreen;

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    Debug.Assert(myBody != null);
  }

  protected void Update()
  {
    Bounds screenBounds = GameController.instance.screenBounds;
    if(screenBounds.Contains(transform.position) == false)
    {
      transform.position =
        screenBounds.ClosestPoint(transform.position);
      if(onAttemptToLeaveScreen != null)
      {
        onAttemptToLeaveScreen();
      }
    }
  }
}
```

 - Add it to the character and fly guy prefabs.

<hr></details><br>
<details><summary>Why use bounds for these checks?</summary>

There are a few ways you could check for an entity walking off the edge of the screen.  I choose to use the Unity bounds struct because it has methods which make the rest of this component easy.  Specifically:

 - Contains: Check if the current position is on the screen.
 - ClosestPoint: Return the closest point on screen for the character, used when he is off-screen to teleport him back.

</details>

<details><summary>What does flipping the walk direction do?</summary>

Each frame the PlayerController sets the walk direction without consider the previous value.  So flipping the walk direction here is promptly overwritten by the PlayerController - resulting in little or no impact to movement in the game.

We included this logic because not all controllers are going to work the same way.  Later in the tutorial we will be adding another entity that uses WalkMovement by only setting desiredWalkDirection periodically.  For that entity, flipping the direction will cause the entity to bounce off the side of the screen and walk the other way.

This logic doesn't impact the character but it's not harmful either and it fits with the theme of this component, enabling reuse.

</details>

<details><summary>What's the different between setting transform.position and using myBody.MovePosition?</summary>

Updates to the Transform directly will teleport your character immediatelly and bypass all physics logic.  

Using the rigidbody.MovePosition method will smoothly transition the object to its new postion.  It's very fast, but if you try this and watch closely, MovePosition is animating a few frames on the way to the target position instead of going there immediatelly.

We are not suggesting one approach should always be used over the other - consider the use case and how you want your game to feel, sometimes teleporting is exactly the feature you're looking for.  

Be careful when you change position using either of these methods as opposed to using forces on the rigidbody.  It's possible that you teleport right into the middle of another object.  The next frame, Unity will try to react to that collision state and this may result in objects popping out in strange ways.

In this component we are setting transform.position for the teleport effect.  If rigidbody.MovePosition was used instead, occasionally issues would arrise as MovePosition competes with other forces on the object.

</details>


## Fly guy turns around when reaching the edge

Create a script to have the fly guy bounce off the edge of the screen and never stop walking.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**BounceOffScreenEdges**:

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(KeepOnScreen))]
[RequireComponent(typeof(WalkMovement))]
public class BounceOffScreenEdges : MonoBehaviour
{
  WalkMovement walkMovement;

  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    Debug.Assert(walkMovement != null);
  }

  protected void Start()
  {
    KeepOnScreen keepOnScreen = GetComponent<KeepOnScreen>();
    keepOnScreen.onAttemptToLeaveScreen 
      += KeepOnScreen_onAttemptToLeaveScreen;
  }

  void KeepOnScreen_onAttemptToLeaveScreen()
  {
    walkMovement.desiredWalkDirection
      = -walkMovement.desiredWalkDirection;
  }
}
```

 - Add it to the fly guy prefab.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>



## Detect floors

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

  Collider2D myCollider;

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




## Prevent double jump

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


## Update WanderWalkController to prefer traveling up hill

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
  float timeBeforeFirstWander = 5;

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
        = UnityEngine.Random.value >= oddsOfGoingUpHill ? 1 : -1;
    }
    else if(dot > 0)
    { 
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value >= oddsOfGoingUpHill ? -1 : 1;
    }
    else
    { 
```

<details><summary>Existing code</summary>

```csharp
      walkMovement.desiredWalkDirection
        = UnityEngine.Random.value >= .5f ? 1 : -1; 
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


## Rotate to match the floor's angle

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
    if(floorDetector.isTouchingFloor)
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


## Add ladders to the world

Create GameObjects and layout ladders in the world and set their tag to Ladder.  

<details><summary>How</summary>

 - Create a parent Ladder GameObject, add the ladder sprite(s).  We are using **spritesheet_tiles_23** and **33**.
 - Order in Layer -1.
 - Position the ladder and repeat, creating several ladders - some which look broken.
   - The child sprite GameObjects should have a default Transform, with the execption of the Y position when multiple sprites are used.
   - It usually looks fine to overlap sprites a bit, as we do to get the space between ladder steps looking good.

<img src="http://i.imgur.com/CDwdJ3c.gif" width=500px />

 - Create a new parent GameObject to hold all the ladders (optional).
 - Create a tag for "Ladder".
 - Select all the ladder GameObjects and change their tag to Ladder.

<hr></details><br>
<details><summary>TODO</summary>

TODO why tag and not layer here?

<hr></details>


## Ladder trigger colliders

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



## Add a script for the player to climb ladders

Create a script to climb ladders and update the player controller to match.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**LadderMovement**:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(FloorDetector))]
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

  GameObject ladderWeAreOn
  {
    get
    {
      return _ladderWeAreOn;
    }
    set
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

  public GameObject FindClosestLadder()
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

 - Add it to the character.
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


## Disable physics when climbing

While climbing a ladder, disable physics.

<details><summary>How</summary>

 - Create script Code/Utils/**ChangeBodyToKinematicCommand**:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class ChangeBodyToKinematicCommand
{
  readonly List<Rigidbody2D> impactedBodyList
    = new List<Rigidbody2D>();

  public ChangeBodyToKinematicCommand(
    GameObject gameObject)
  {
    Rigidbody2D[] bodyList 
      = gameObject.GetComponentsInChildren<Rigidbody2D>();
    for(int i = 0; i < bodyList.Length; i++)
    {
      Rigidbody2D body = bodyList[i];
      if(body.bodyType == RigidbodyType2D.Dynamic)
      {
        body.bodyType = RigidbodyType2D.Kinematic;
        impactedBodyList.Add(body);
      }
    }
  }

  public void Undo()
  {
    for(int i = 0; i < impactedBodyList.Count; i++)
    {
      Rigidbody2D body = impactedBodyList[i];
      body.bodyType = RigidbodyType2D.Dynamic;
    }
  }
}
```

 - Update LadderMovement as follows (or copy paste TODO link):



<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(FloorDetector))]
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

  GameObject ladderWeAreOn
  {
    get
    {
      return _ladderWeAreOn;
    }
    set
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
  ChangeBodyToKinematicCommand changeBodyCommand; 
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
    changeBodyCommand = new ChangeBodyToKinematicCommand(gameObject); 
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
    changeBodyCommand.Undo(); 
    changeBodyCommand = null;
```

<details><summary>Existing code</summary>

```csharp

    desiredClimbDirection = 0;

    if(onGettingOffLadder != null)
    {
      onGettingOffLadder();
    }
  }

  public GameObject FindClosestLadder()
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


## Random climber controller

Fly guy / bomb climb script.


## laddermovement (fly guy don't walk off the side of a ladder).

Change the wander walk.


## ladder vs bomb velocity

Save/restore velocity.

## Move towards the center of the ladder


## Prevent enemies spawning on top of the character

Update the door spawner so that it does not spawn if the character is too close.

<details><summary>How</summary>

 - Update the 'Spawner' script with the following (or copy/paste TODO link):

Use Collider2D overlap.


TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## Add points for jumping over enemies

TODO

## Test

GG

# Next chapter

[Chapter 4](https://github.com/hardlydifficult/Platformer/blob/master/Chapter4.md).
