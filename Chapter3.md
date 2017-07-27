# 3) An item and another enemy

In chapter 3, TODO intro...

This assumes you completed chapter 2, or you can download the project so far. (TODO link)

TODO tutorial video link

TODO gif

demo build of level 3



## 3.1) Create a Hammer

Create a Hammer prefab and then layout several in the world.

<details><summary>How</summary>

 - Change the sprite's pivot to Bottom. We are using **Hammer**.
 - Add to the world and scale (to about .2).
 - Add a PolygonCollider2D.

<img src="http://i.imgur.com/mfrIum0.png" width=300px /> 

 - Check Is Trigger.
 - Create a prefab.
 - Add several Hammers and lay them out for the level.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>




## 3.2) Equipt the hammer

Add a script to the hammer and character, allowing the character to pickup the hammer and then kill enemies until it despawns.

<details><summary>How</summary>

 - Create script Code/Components/Weapons/**WeaponHolder**:

```csharp
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
  public GameObject currentWeapon;
}
```

 - Add it to the character.
 - Create script Code/Components/Weapon/**Hammer**:

```csharp
using UnityEngine;

public class Hammer : MonoBehaviour
{
  [SerializeField]
  Vector2 positionWhenEquipt = new Vector2(.214f, -.321f);

  [SerializeField]
  Vector3 rotationWhenEquiptInEuler = new Vector3(0, 0, -90);

  [SerializeField]
  MonoBehaviour[] componentListToEnableOnEquipt;

  WeaponHolder currentHolder;

  protected void OnDestroy()
  {
    if(currentHolder != null)
    {
      currentHolder.currentWeapon = null;
    }
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    WeaponHolder holder = collision.GetComponent<WeaponHolder>();
    if(holder != null && currentHolder == null && holder.currentWeapon == null)
    {
      currentHolder = holder;
      currentHolder.currentWeapon = gameObject;

      transform.SetParent(currentHolder.transform);
      transform.localPosition = positionWhenEquipt;
      transform.localRotation = Quaternion.Euler(rotationWhenEquiptInEuler);

      for(int i = 0; i < componentListToEnableOnEquipt.Length; i++)
      {
        MonoBehaviour component = componentListToEnableOnEquipt[i];
        component.enabled = true;
      }
    }
  }
}
```

 - Add it to the Hammer prefab.
 - Add SuicideIn, disable the component.
 - Add KillOnContactWith configured for layer 'Enemy', disable the component.
 - Under Hammer components to enable, add SuicideIn and KillOnContactWith.
 - Select the spike ball prefab, add DeathEffectSpawn and configure it to use the explosion prefab.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 3.3) Hammer blinks red before despawning

Add a script to the hammer to flash red before it's gone.


<details><summary>How</summary>

 - Create script Code/Components/Death/**DeathEffectFlash**:

```csharp
using System.Collections;
using UnityEngine;

public class DeathEffectFlash : DeathEffect
{
  [SerializeField]
  float lengthBeforeFlash = 7;

  [SerializeField]
  float lengthToFlashFor = 5;

  [SerializeField]
  float timePerColorChange = .75f;

  [SerializeField]
  float colorChangeTimeFactorPerFlash = .85f;

  public override float timeUntilObjectMayBeDestroyed
  {
    get
    {
      return lengthBeforeFlash + lengthToFlashFor;
    }
  }

  public override void PlayDeathEffects()
  {
    StartCoroutine(FlashToDeath());
  }

  IEnumerator FlashToDeath()
  {
    yield return new WaitForSeconds(lengthBeforeFlash);

    SpriteRenderer[] spriteList = GetComponentsInChildren<SpriteRenderer>();
    float timePassed = 0;
    bool isRed = false;
    while(timePassed < lengthToFlashFor)
    {
      spriteList.SetColor(isRed ? Color.red : Color.white);
      isRed = !isRed;

      yield return new WaitForSeconds(timePerColorChange);
      timePerColorChange = Mathf.Max(Time.deltaTime, timePerColorChange);
      timePassed += timePerColorChange;
      timePerColorChange *= colorChangeTimeFactorPerFlash;
    }
  }
}
```

 - Add it to the hammer prefab (will automatically add a DeathEffectManager).

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## 3.4) Create a flying enemy

Create a GameObject for the fly guy reusing components from the spike ball and character.  

<details><summary>How</summary>

 - Select **spritesheet_jumper_30**, **84**, and **90** and drag them into the Hierarchy, creating Assets/Animations/**FlyGuyWalk**.
 - Set Order in Layer to 1.
 - Add it to a parent GameObject named "FlyGuy".
 - Set the Layer for FlyGuy to 'Enemy'.
 - Add a Rigidbody2D and freeze the Z rotation.
 - Add a CapsuleCollider2D and adjust the size.

<img src="http://i.imgur.com/d1lxoEj.png" width=150px />

 - Add WalkMovement.
 - Add DeathEffectSpawn and configure it to use the explosion prefab.
 - Add KillOnContactWith and set the layermask to Player.

</details>


## 3.5) Make the fly guy walk

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
  float timeBeforeFirstWander = 10;

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
      = UnityEngine.Random.value <= .5f ? 1 : -1;
  }
}
```

 - Add WanderWalkController to the FlyGuy.

</details>


## 3.6) Make the fly guy float above the ground

Add a second collider so that the body of this entity is above the ground but does not kill a character walking underneath.

<details><summary>How</summary>

 - Add an empty GameObject as a child under the FlyGuy.  Name it "Feet".
 - Create a Layer for "Feet" and assign it to the Feet GameObject.
 - Update the Physics 2D collision matrix to disable Feet / Player, Feet / Enemy, and Feet / Feet collisions.
 - Add a CircleCollider2D to the Feet and size and position it below the body.

<img src="http://i.imgur.com/VMPqiFE.png" width=300px />


<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## 3.7) Add a spawner for fly guys

Create a second spawner at the bottom for fly guys.


<details><summary>How</summary>

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

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 3.8) Restrict movement to stay on screen

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

## 3.9) Fly guy turns around when reaching the edge

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

## 3.10) Fade in entities

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
 - Add FadeInThenEnable to the hammer prefab, nothing needed in the to enable list.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 3.11) Create a GameController script

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


## 3.12) Decrement lives when the character dies

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


## 3.13) Respawn on death

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


## 3.14) Clear and restart the level on death

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








## 3.15) Prevent enemies spawning on top of the character

Update the door so that it does not spawn if the character is too close.

<details><summary>How</summary>

 - Add a BoxCollider2D and size it to cover the entrance area.

<img src="http://i.imgur.com/Jq4rU93.png" width=300px />

 - Check 'Is Trigger'.
 - Update the 'Spawner' script with the following (or copy/paste TODO link):

<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour, ICareWhenPlayerDies
{
  [SerializeField]
  GameObject thingToSpawn;

  [SerializeField]
  float initialWaitTime = 2;

  [SerializeField]
  float minTimeBetweenSpawns = .5f;

  [SerializeField]
  float maxTimeBetweenSpawns = 10;
```

</details>

```csharp  
  Collider2D safeZoneCollider;

  ContactFilter2D contactFilter;

  Collider2D[] tempColliderList = new Collider2D[1];
  
  protected void Awake()
  {
    safeZoneCollider = GetComponent<Collider2D>();
    contactFilter = new ContactFilter2D()
    {
      layerMask = LayerMask.GetMask(new[] { "Player" }),
      useLayerMask = true
    };
  }
```

<details><summary>Existing code</summary>

```csharp
  protected void Start()
  {
    StartCoroutine(SpawnEnemies());
  }

  void ICareWhenPlayerDies.OnPlayerDeath()
  {
    StopAllCoroutines();
    StartCoroutine(SpawnEnemies());
  }

  IEnumerator SpawnEnemies()
  {
    yield return new WaitForSeconds(initialWaitTime);

    while(true)
    {
```

</details>

```csharp
      if(safeZoneCollider == null 
        || safeZoneCollider.OverlapCollider(contactFilter, tempColliderList) == 0)
      {
```

<details><summary>Existing code</summary>

```csharp
        Instantiate(
          thingToSpawn,
          transform.position,
          Quaternion.identity);
```

</details>

```csharp
      }
```

<details><summary>Existing code</summary>

```csharp
      float sleepTime = UnityEngine.Random.Range(
        minTimeBetweenSpawns,
        maxTimeBetweenSpawns);
      yield return new WaitForSeconds(sleepTime);
    }
  }
}
```

</details>


 - Set the Layers to Consider to Player.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 3.16) Add points for jumping over enemies

Add a collider and script to award points anytime the character jumps over an enemy.


<details><summary>How</summary>

 - Create a new Layer for "Points" and disable everything except for Points / Player.

<img src="http://i.imgur.com/5sxuf2I.png" width=150px />

 - Add the fly guy and spike ball to scene.
 - For each, add a new empty GameObject as a child named "Points" and then:
   - Assign the Points layer to the Points GameObject.
   - Add a Rigidbody2D and change the Body Type to 'Kinematic'.
   - Add a BoxCollider2D and check Is Trigger.
   - Size the collider to capture the area above.

<img src="http://i.imgur.com/gmMDJlD.png" width=150px />

 - Create script Code/Components/Effects/**AwardPointsOnJumpOver**:

```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AwardPointsOnJumpOver : MonoBehaviour
{
  [SerializeField]
  int pointsToAward = 100;

  [SerializeField]
  LayerMask playerAndAllPossibleObstacles;

  BoxCollider2D myCollider;

  ContactFilter2D contactFilter;

  RaycastHit2D[] tempHitList = new RaycastHit2D[1];

  LayerMask myLayerMask;

  protected void Awake()
  {
    myCollider = GetComponent<BoxCollider2D>();

    contactFilter = new ContactFilter2D()
    {
      layerMask = playerAndAllPossibleObstacles,
      useLayerMask = true,
      useTriggers = true
    };

    myLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

    Debug.Assert(myCollider != null);
  }

  protected void OnTriggerStay2D(
    Collider2D collision)
  {
    if(myLayerMask.Includes(collision.gameObject.layer) == false)
    {
      return;
    }

    Physics2D.Raycast(
      transform.parent.position, 
      Vector2.up, 
      contactFilter, 
      tempHitList);

    if(tempHitList[0].collider == collision)
    {
      GameController.instance.points += pointsToAward;

      Destroy(this);
    }
  }
}
```

 - Add it to both Points GameObjects.
 - Set layer mask to Player and Floor.
 - Apply changes to the prefabs and delete the GameObjects.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## 3.17) Hold rotation on the point collider

Create a script to hold the child GameObjects rotation while the parent spins.

<details><summary>How</summary>

 - Create script Code/Components/Movement/**HoldRotation**:

```csharp
using UnityEngine;

public class HoldRotation : MonoBehaviour
{
  Quaternion originalRotation;

  protected void Awake()
  {
    originalRotation = transform.rotation;
  }

  protected void Update()
  {
    transform.rotation = originalRotation;
  }
}
```

 - Add it to the Points GameObject under the spike ball prefab.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>




## 3.18) Test

GG

# Next chapter

[Chapter 4](https://github.com/hardlydifficult/Platformer/blob/master/Chapter4.md).
