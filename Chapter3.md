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

 - Create a script **WanderWalkController** under Assets/Code/Compenents/Movement and paste the following:

```csharp
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WalkMovement))]
public class WanderWalkController : MonoBehaviour
{
  [SerializeField]
  float oddsOfGoingUpHill = .8f;

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

Create a script which ensures the character can not walk off screen.

<details><summary>How</summary>

 - Create a C# script "KeepWalkMovementOnScreen" under Assets/Code/Components/Movement.
 - Select the Character GameObject and add the KeepWalkMovementOnScreen component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Ensures that the entity stays on the screen. 
/// It will flip the current walk direction automatically 
/// (which has no impact on the Player but causes enemies to bounce).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KeepWalkMovementOnScreen : MonoBehaviour
{
  #region Data
  /// <summary>
  /// Used to determine if we are currently moving.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Used to cause the entity to start walking the 
  /// opposite direction when it hits the edge of the screen.
  /// 
  /// This is not required and may be null.
  /// </summary>
  WalkMovement walkMovement;
  #endregion

  #region Init
  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    walkMovement = GetComponent<WalkMovement>();

    

    Debug.Assert(myBody != null);
  }
  #endregion

  #region Events
  /// <summary>
  /// A Unity event, called each frame.
  /// 
  /// If the entity is off screen, pop it back 
  /// and flip the walk direction.
  /// </summary>
  protected void Update()
  {
    // Check if the entity is off screen
    if(GameController.instance.screenBounds.Contains(transform.position) == false)
    { 
      // Move the entity back to the edge of the screen
      transform.position =
        GameController.instance.screenBounds.ClosestPoint(transform.position);
      if(walkMovement != null)
      {
        // Flip the walk direction
        walkMovement.desiredWalkDirection 
          = -walkMovement.desiredWalkDirection;
      }
    }
  }
  #endregion
}
```

</details>

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




TODO WanderWalk needs floordetector, laddermovement, and GameController screen bounds.
 - Add components FloorDetector,  KeepWalkMovementOnScreen, LadderMovement, RotateFacingDirection.
 - Prevent walking into walls?

## Detect floors

Create a script to calculate the distance to and rotation of the floor under an entity.

<details><summary>How</summary>

 - Create a layer 'Floor'.
 - Select all the Platform GameObjects and change to Layer Floor.
   - When prompted, you can 'Yes, change children'.
 - Create a C# script "FloorDetector" under Assets/Code/Components/Movement.
 - Select the Character and add the FloorDetector component.
 - Paste in the following code:

```csharp
using UnityEngine;

/// <summary>
/// Used to determine if the entity is on the ground.  
/// Also provides properties about the ground we are standing on.
/// 
/// This component may be placed on the main entity GameObject 
/// or a child GameObject.  A child may be used to offset the feet
/// from the collider used for other things.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FloorDetector : MonoBehaviour
{
  /// <summary>
  /// The rotation that's applied when a floor is upside down.
  /// </summary>
  static readonly Quaternion backwardsRotation = Quaternion.Euler(0, 0, 180);

  /// <summary>
  /// The collider on this gameObject, used to determine if we 
  /// are currently on the ground (vs jumping or falling).
  /// </summary>
  Collider2D myCollider;

  /// <summary>
  /// Sets a LayerMask to 'Floor' for use when calling Physics 
  /// to check if we are on ground.
  /// </summary>
  ContactFilter2D floorFilter;

  /// <summary>
  /// True if the entity is currently standing on the ground.
  /// </summary>
  public bool isTouchingFloor
  {
    get; private set;
  }

  /// <summary>
  /// The up direction / normal for the floor we are standing on.
  /// Null if we isTouchingFloor == false.
  /// </summary>
  public Vector2? floorUp
  {
    get; private set;
  }

  /// <summary>
  /// The rotation for the floor we are standing on.
  /// Null if we isTouchingFloor == false.
  /// </summary>
  public Quaternion? floorRotation
  {
    get; private set;
  }

  /// <summary>
  /// How far above the floor we are ATM.  
  /// 0 if isTouchingFloor.
  /// Null if there is no floor under us.
  /// </summary>
  public float? distanceToFloor
  {
    get; private set;
  }

  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
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

  /// <summary>
  /// A Unity event, called every x ms of game time.
  /// 
  /// Checks for floor and updates properties.
  /// </summary>
  protected void FixedUpdate()
  {
    Collider2D floorWeAreStandingOn = DetectTheFloorWeAreStandingOn();
    isTouchingFloor = floorWeAreStandingOn != null;

    Collider2D floorUnderUs;
    if(floorWeAreStandingOn != null)
    {
      floorUp = CalculateFloorUp(floorWeAreStandingOn);
      floorRotation = CalculateFloorRotation(floorWeAreStandingOn);
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

  /// <summary>
  /// Returns the collider for the floor / platform we are 
  /// standing on, if we are not in the air.
  /// </summary>
  /// <returns>The floor's collider, or null.</returns>
  Collider2D DetectTheFloorWeAreStandingOn()
  {
    Collider2D[] possibleResultList = new Collider2D[3];

    // Ask Unity which floors we are colliding with
    int foundColliderCound
      = Physics2D.OverlapCollider(myCollider, floorFilter, possibleResultList);

    for(int i = 0; i < foundColliderCound; i++)
    {
      Collider2D collider = possibleResultList[i];
      ColliderDistance2D distance = collider.Distance(myCollider);

      // If my collider is on or above the floor
      // (vs jumping up through a floor)
      // and we are making contact with the top (vs bottom) 
      if(distance.distance >= -.1f
        && Vector2.Dot(Vector2.up, distance.normal) > 0)
      {
        return collider;
      }
    }

    // Didn't find a valid floor, we must be in the air.
    return null;
  }

  /// <summary>
  /// If we are not standing on a floor, this may be used
  /// to raycast from the center of the entity downwards,
  /// looking for the first floor underneath us.
  /// </summary>
  /// <returns>The floor's collider, or null.</returns>
  Collider2D DetectFloorUnderUs()
  {
    // Raycast to find any floor under us if we can.
    RaycastHit2D[] result = new RaycastHit2D[1];
    if(Physics2D.Raycast(transform.position, Vector2.down, floorFilter, result) > 0)
    {
      return result[0].collider;
    }

    // Can't find any floor
    // this should never happen with our level design.
    return null;
  }

  /// <summary>
  /// If we are standing on a floor, this may be used
  /// to determine its up direction.
  /// </summary>
  /// <returns>
  /// The floor 'up' normally.  
  /// 'Down' when the floor is upsidedown.
  /// i.e. always facing positive Y.
  /// </returns>
  static Vector2 CalculateFloorUp(
    Collider2D floorWeAreStandingOn)
  {
    Debug.Assert(floorWeAreStandingOn != null);

    // The transform up represents the platform's normal because any rotation in the platform sprite 
    // is part of it's gameObject (vs drawn with rotation or rotated in a child object).
    Vector2 floorUp = floorWeAreStandingOn.transform.up;
    if(Vector2.Dot(Vector2.up, floorUp) >= 0)
    {
      return floorUp;
    }
    else
    {
      // Use down instead
      return -floorUp;
    }
  }

  /// <summary>
  /// If we are standing on a floor, this may be used
  /// to determine its rotation.
  /// </summary>
  /// <returns>
  /// The floor rotation normally.  
  /// The floor rotation * (0, 0, 180) when the floor is upsidedown.
  /// i.e. always facing the world up.
  /// </returns>
  static Quaternion CalculateFloorRotation(
    Collider2D floorWeAreStandingOn)
  {
    Debug.Assert(floorWeAreStandingOn != null);

    Quaternion floorRotation = floorWeAreStandingOn.transform.rotation;
    if(Quaternion.Dot(floorRotation, Quaternion.identity) >= 0)
    {
      return floorRotation;
    }
    else
    {
      return floorRotation * backwardsRotation;
    }
  }

  /// <summary>
  /// Determines the distance to the closest floor.
  /// </summary>
  /// <returns>
  /// 0 if standing on a floor.
  /// > 0 if there is floor under us.
  /// null if we couldn't find a floor.
  /// </returns>
  float? CalculateDistanceToFloor(
    Collider2D floorWeAreStandingOn,
    Collider2D floorUnderUs)
  {
    if(floorWeAreStandingOn != null)
    {
      // If standing, distance is assumed to be 0
      return 0;
    }
    else if(floorUnderUs != null)
    {
      // Compare bounds to determine the separation between them
      float yOfTopOfFloor = floorUnderUs.bounds.max.y;

      // If an edgeRadius was used, this must be added to the bounds info
      if(floorUnderUs is BoxCollider2D)
      {
        BoxCollider2D boxCollider = (BoxCollider2D)floorUnderUs;
        yOfTopOfFloor += boxCollider.edgeRadius;
      }

      return myCollider.bounds.min.y - yOfTopOfFloor;
    }
    else
    {
      // Couldn't find a floor
      return null;
    }
  }
}
```

</details>

TODO question - when changing layers, yes change children..
http://i.imgur.com/xFiD5Vc.png

TODO question - why not require floordetector component? / why GetComponentInChildren



## Prevent double jump

Update JumpMovement to prevent double jump and flying (by spamming space), by leveraging the FloorDetector just created.

<details><summary>How</summary>

 - Update JumpMovement with the following changes (or copy paste the full version TODO link).

<details><summary>Existing code</summary>

```csharp
using UnityEngine;

/// <summary>
/// Controls the entity's jump.  
/// 
/// Another component drives when to jump via Jump().
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class JumpMovement : MonoBehaviour
{
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
```

</details>

```csharp
  /// <summary>
  /// Used to confirm we are grounded before jumping.
  /// </summary>
  FloorDetector floorDector; 
```

<details><summary>Existing code</summary>

```csharp
  /// <summary>
  /// Used to play sound effects.
  /// </summary>
  AudioSource audioSource;

  /// <summary>
  /// Used to process events in FixedUpdate that 
  /// may have been captured on Update.
  /// </summary>
  bool wasJumpRequestedSinceLastFixedUpdate;

  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    
```

</details>

```csharp
    floorDector = GetComponentInChildren<FloorDetector>(); 
    Debug.Assert(floorDector != null); 
```

<details><summary>Existing code</summary>

```csharp
    audioSource = GetComponent<AudioSource>();

    Debug.Assert(myBody != null);
    Debug.Assert(audioSource != null);
  }

  /// <summary>
  /// Adds force to the body to make the entity jump.
  /// </summary>
  public void Jump()
  {
    Debug.Assert(jumpSpeed >= 0,
      "jumpSpeed must not be negative");

    wasJumpRequestedSinceLastFixedUpdate = true;
  }

  protected void FixedUpdate()
  {
    if(wasJumpRequestedSinceLastFixedUpdate)
    {
```

</details>

```csharp
      if(floorDector.isTouchingFloor) 
      {
```

<details><summary>Existing code</summary>


```csharp
        // Jump!
        myBody.AddForce(
            new Vector2(0, jumpSpeed),
            ForceMode2D.Impulse);

        // Play the sound effect
        audioSource.PlayOneShot(jumpSound);
```

</details>

```csharp
      } 
```

<details><summary>Existing code</summary>

```csharp
      // Clear the jump flag, enabling the next jump
      wasJumpRequestedSinceLastFixedUpdate = false;
    }
  }
}
```
</details>

</details>

TODO could add a jump cooldown by time as well - but that would not be a complete solution unless a long cooldown was used.


## Rotate to match the floor's angle

Create a script to rotate an entity, aligning with the floor when touching one, otherwise rotating back to the default position.

<details><summary>How</summary>

```csharp
using UnityEngine;

/// <summary>
/// When on floor, rotates an entity to align with the floor. 
/// 
/// When in air, rotates towards identity 
/// (back to standing straight up).
/// </summary>
[RequireComponent(typeof(RotateFacingDirection))]
public class RotateToAlignWithFloor : MonoBehaviour
{
  /// <summary>
  /// The rotation that's applied when looking left (vs right).
  /// </summary>
  /// <remarks>Cached here for performance.</remarks>
  static readonly Quaternion backwardsRotation 
    = Quaternion.Euler(0, 180, 0);

  /// <summary>
  /// How quickly the entity rotates so that 
  /// its feet are both on the floor.
  /// </summary>
  [SerializeField]
  float rotationLerpSpeed = .4f;

  /// <summary>
  /// Used to get info about the floor we are over.
  /// </summary>
  FloorDetector floorDetector;

  /// <summary>
  /// Used to determine the current facing direction.
  /// </summary>
  RotateFacingDirection facingDirection;

  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    floorDetector = GetComponentInChildren<FloorDetector>();
    facingDirection = GetComponent<RotateFacingDirection>();

    Debug.Assert(floorDetector != null);
    Debug.Assert(facingDirection != null);
  }

  /// <summary>
  /// A Unity event, called each frame.
  /// 
  /// Update the entities rotation.
  /// </summary>
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
      // If the entity is flipped, also flip the target 
      // rotation we are lerping towards
      targetRotation *= backwardsRotation;
    }

    transform.rotation = Quaternion.Lerp(
      transform.rotation, 
      targetRotation, 
      rotationLerpSpeed * Time.deltaTime);
  }
}
```

</details>

TODO what is lerp (and slerp?)


## Add Ladder sprites to the world

Layout ladders in the world.  We are using [Kenney.nl's Platformer Pack Redux](http://kenney.nl/assets/platformer-pack-redux) spritesheet_tiles sprites 23 and 33.

<details><summary>How</summary>

 - Create a parent Ladder GameObject, add sprite(s) for the look you want.
   - You may want to 'Flip' the Y in the SpriteRenderer to mirror the top and bottom on some ladders.
   - The child sprite GameObjects should have a default Transform, with the execption of the Y position when multiple sprites are used.
   - It usually looks fine to overlap sprites a bit, as we do to get the space between ladder steps looking good.
 - Set the SpriteRenderers' Order is Layer to -1.
 - Position the ladder and repeat, creating several ladders - some which look broken.
 - Create a new parent GameObject to hold all the ladders (optional).

<img src="http://i.imgur.com/NtZZZxD.gif" />

</details>


## Ladder trigger colliders

Create trigger colliders for each of the ladders to be used for climbing.

<details><summary>How</summary>

 - Add BoxCollider2D.
 - Size them so that:
   - The width is thinner than the sprite.
   - The bottom of the collider is about half the character's height above the lower platform.
   - The top of the collider is about half the character's height above the upper platform.
 - Check "Is Trigger".

<img src="http://i.imgur.com/GyGCU4n.png" />

TODO update collider size screenshot

</details>

TODO how do you know what size to make the collider?

## Set tag to Ladder

Update all the ladder GameObjects to have the Ladder tag.

<details><summary>How</summary>

 - Create a tag for "Ladder".
 - Select all the ladder GameObjects and change their tag to Ladder.
   - You can select "Yes, change children" when prompted.

</details>

TODO why tag and not layer here?


## Change colliders to trigger

Create a script to change all colliders for a GameObject to triggers, and then allow undo later on.

<details><summary>How</summary>

 - Create a script Assets/Code/Utils/ChangeCollidersToTriggersCommand
 - Paste in the following:

```csharp
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This uses the 'Command pattern' to disable colliders on a 
/// gameObject (by changing them to triggers).
/// 
/// It stores the colliders modified so it may undo the change
/// later.
/// </summary>
public class ChangeCollidersToTriggersCommand
{
  /// <summary>
  /// The colliders which were modified.
  /// Saved to enable undo later on.
  /// </summary>
  List<Collider2D> impactedColliderList;

  /// <summary>
  /// Disables all colliders on the gameObject 
  /// and stores them allowing undo later.
  /// </summary>
  /// <param name="gameObject">
  /// The gameObject to disable colliders for.
  /// </param>
  public ChangeCollidersToTriggersCommand(
    GameObject gameObject)
  {
    impactedColliderList = new List<Collider2D>();
    Collider2D[] colliderList 
      = gameObject.GetComponentsInChildren<Collider2D>();
    for(int i = 0; i < colliderList.Length; i++)
    {
      Collider2D collider = colliderList[i];
      // Only modify colliders 
      // (vs anything that is already a trigger)
      if(collider.isTrigger == false)
      { 
        // Store this for undo later
        impactedColliderList.Add(collider);

        // Change to trigger, allowing this to pass-through 
        // obstacles
        collider.isTrigger = true;
      }
    }
  }

  /// <summary>
  /// Re-enable all colliders this command originally disabled.
  /// </summary>
  public void Undo()
  {
    for(int i = 0; i < impactedColliderList.Count; i++)
    {
      Collider2D collider = impactedColliderList[i];
      collider.isTrigger = false;
    }
  }
}
```

</details>

TODO What do you mean by command pattern?
TODO why?

## LadderMovement

LadderMovement, for character and spike ball.


<details><summary>How</summary>

 - Create "LadderMovement"
 - Add to character

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the motion up/down ladders.
/// 
/// Driven primarily by desiredClimbDirection.
/// 
/// When on a ladder, this component overwrites the 
/// rigidbody velocity.y (not the .x), preventing gravity.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class LadderMovement : MonoBehaviour
{
  /// <summary>
  /// Set by another component to attempt climbing a ladder 
  /// up/down.
  /// </summary>
  [NonSerialized]
  public float desiredClimbDirection;

  /// <summary>
  /// Called when the entity first gets on a ladder.
  /// </summary>
  public event Action onGettingOnLadder;

  /// <summary>
  /// Called when the entity gets off a ladder it was 
  /// previously climbing.
  /// </summary>
  public event Action onGettingOffLadder;

  /// <summary>
  /// True if the entity is currently on a ladder.
  /// </summary>
  public bool isOnLadder
  {
    get
    {
      return ladderWeAreOn != null;
    }
  }

  /// <summary>
  /// How quickly the entity moves up/down ladders.
  /// </summary>
  [SerializeField]
  float climbSpeed = 60;

  /// <summary>
  /// Used to turn off gravity while we are climbing.
  /// </summary>
  Rigidbody2D myBody;

  /// <summary>
  /// Used to determine the distance to the ground.
  /// </summary>
  FloorDetector floorDetector;

  /// <summary>
  /// Backs the ladderWeAreOn property.
  /// </summary>
  GameObject _ladderWeAreOn;

  /// <summary>
  /// The ladder we are currently climbing, if any.
  /// </summary>
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
        // No changes
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

  /// <summary>
  /// Used to turn off colliders when we get on a ladder,
  /// and then turn them back on when we get off a ladder.
  /// This allows us to walk through floors while climbing.
  /// </summary>
  ChangeCollidersToTriggersCommand triggerCommand;

  /// <summary>
  /// Via trigger enter/exit we maintain a list of all 
  /// the ladders the entity is currently standing on.
  /// </summary>
  List<GameObject> currentLadderList;

  /// <summary>
  /// A Unity event, called once before this GameObject
  /// is spawned in the world.
  /// </summary>
  protected void Awake()
  {
    currentLadderList = new List<GameObject>();
    myBody = GetComponent<Rigidbody2D>();
    floorDetector = GetComponentInChildren<FloorDetector>();

    Debug.Assert(myBody != null);
    Debug.Assert(floorDetector != null);
  }

  /// <summary>
  /// When we encounter a new ladder, add it to the list.
  /// </summary>
  /// <param name="collision">The gameObject we just encountered.</param>
  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    // Ignore anything which is not a ladder
    if(collision.CompareTag("Ladder") == false)
    {
      return;
    }

    // Add this to the list of ladders we are on top of
    currentLadderList.Add(collision.gameObject);
  }

  /// <summary>
  /// When we walk away from a ladder, remove it from the currentLadderList.
  /// </summary>
  /// <param name="collision">The gameObject we are walking away from.</param>
  protected void OnTriggerExit2D(
    Collider2D collision)
  {
    // If the ladder being removed is the currentLadder, force getting off.
    if(collision.gameObject == ladderWeAreOn)
    {
      GetOffLadder();
    }

    // Remove the ladder from the list
    currentLadderList.Remove(collision.gameObject);
  }

  /// <summary>
  /// Consider getting on/off a ladder given climbDirection. 
  /// When on a ladder, control the entity's y movement.
  /// </summary>
  protected void FixedUpdate()
  {
    GameObject ladder = ladderWeAreOn;

    if(ladder == null)
    {
      // If we are not on a ladder, check if we are near one.
      ladder = FindClosestLadder();
      if(ladder == null)
      {
        // If we are not near a ladder, there's nothing to do
        return;
      }
    }

    // Get the climbable region for the ladder
    Bounds bounds = ladder.GetComponent<Collider2D>().bounds;

    if(isOnLadder == false && Mathf.Abs(desiredClimbDirection) > 0.01)
    {
      // If the desiredClimbDirection is not zero, consider getting on
      if(((desiredClimbDirection > 0 && myBody.position.y < bounds.center.y)
        || (desiredClimbDirection < 0 && myBody.position.y > bounds.center.y)))
      {
        // Get on if moving up and on lower half or moving down and on upper half
        ladderWeAreOn = ladder;
      }
    }

    if(isOnLadder)
    {
      float currentVerticalVelocity = myBody.velocity.y;
      if(floorDetector.distanceToFloor > .1f && floorDetector.distanceToFloor < .3f
        && ((currentVerticalVelocity > 0 && myBody.position.y > bounds.center.y)
          || (currentVerticalVelocity < 0 && myBody.position.y < bounds.center.y)))
      {
        // If feet near ground and moving towards end of ladder
        GetOffLadder();
      }
      else
      {
        // Move up/down ladder or hold current location
        myBody.velocity = new Vector2(myBody.velocity.x,
          desiredClimbDirection * climbSpeed * Time.fixedDeltaTime);
      }
    }
  }

  /// <summary>
  /// Get off the ladder.
  /// </summary>
  public void GetOffLadder()
  {
    ladderWeAreOn = null;
  }

  /// <summary>
  /// Called when the entity first gets on a ladder.
  /// </summary>
  void OnGettingOnLadder()
  {
    Debug.Assert(triggerCommand == null);

    // When we first get on a ladder, disable physics to allow climbing through the floor
    triggerCommand = new ChangeCollidersToTriggersCommand(gameObject);
    myBody.gravityScale = 0;
    myBody.velocity = Vector2.zero;

    if(onGettingOnLadder != null)
    {
      // Fire event for other components
      onGettingOnLadder();
    }
  }

  /// <summary>
  /// Called when an entity gets off a ladder they are climbing.
  /// </summary>
  void OnGettingOffLadder()
  {
    // When we get off a ladder, re-enable physics
    triggerCommand.Undo();
    triggerCommand = null;
    desiredClimbDirection = 0;
    myBody.GetComponent<Collider2D>().isTrigger = false;
    myBody.gravityScale = 1;

    if(onGettingOffLadder != null)
    {
      // Fire event for other components
      onGettingOffLadder();
    }
  }

  /// <summary>
  /// The best fit ladder the entity is standing on/near, 
  /// if any.
  /// </summary>
  /// <returns>The closest ladder's GameObject, or null.</returns>
  GameObject FindClosestLadder()
  {
    if(currentLadderList.Count == 0)
    {
      // We are not near any ladder ATM
      return null;
    }

    // Select the closest ladder, if we are standing near several
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

## PlayerController to climb ladders



<details><summary>How</summary>

 - TODO

<details><summary>Existing code</summary>

```csharp
using UnityEngine;

/// <summary>
/// Wires up user input, allowing the user to 
/// control the player in game with a keyboard.
/// </summary>
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
  /// <summary>
  /// Used to cause the object to walk.
  /// </summary>
  WalkMovement walkMovement;

  /// <summary>
  /// Used to cause the object to jump.
  /// </summary>
  JumpMovement jumpMovement;
```

</details>

```csharp
  /// <summary>
  /// Used to cause the object to climb up or down.
  /// </summary>
  LadderMovement ladderMovement; 
```

<details><summary>Existing code</summary>

```csharp
  /// <summary>
  /// A Unity event, called once before the GameObject
  /// is instantiated.
  /// </summary>
  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    jumpMovement = GetComponent<JumpMovement>();
```

</details>

```csharp
    ladderMovement = GetComponent<LadderMovement>(); 
    Debug.Assert(ladderMovement != null); 
```

<details><summary>Existing code</summary>

```csharp

    Debug.Assert(walkMovement != null);
    Debug.Assert(jumpMovement != null);
  }

  /// <summary>
  /// A Unity event, called every x ms of game time.
  /// 
  /// Consider moving.
  /// </summary>
  /// <remarks>
  /// Moving uses an input state, and therefore may be captured 
  /// on Update or FixedUpdate, we use FixedUpdate since physics 
  /// also runs on FixedUpdate, so trying to do this on update would
  /// require an extra cache (w/o benefit).
  /// </remarks>
  protected void FixedUpdate()
  {
    // Consider moving left/right based off keyboard input.
    walkMovement.desiredWalkDirection
      = Input.GetAxis("Horizontal");
```

</details>

```csharp
    // Consider climbing a ladder
    ladderMovement.desiredClimbDirection 
      = Input.GetAxis("Vertical");
```

<details><summary>Existing code</summary>

```csharp
  }

  /// <summary>
  /// A Unity event, called once per frame.
  /// 
  /// Consider jumping.
  /// </summary>
  /// <remarks>
  /// Jumping uses an input event, and therefore must be
  /// captured on Update.
  /// </remarks>
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

</details>


## Make the fly guy climb



<details><summary>How</summary>


aoeu

```csharp

```

</details>

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
