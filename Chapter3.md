# 3) Triggers, collisions, and raycasts

Triggers, singleton game controller, OverlapCollider, Raycast

In chapter 3, we add hammers (an item/weapon), HoverGuy (a second enemy), lives (and respawn), and points. 

TODO intro...

This assumes you completed chapter 2, or you can download the project so far. (TODO link)

TODO tutorial video link

TODO gif

demo build of level 3



## 3.1) Create a Hammer

Create a Hammer prefab and then layout several in the world.

<details><summary>How</summary>

 - Change the sprite's pivot to Bottom. We are using **Hammer**.
 - Add to the world and scale (to about .2).
 - Add a **PolygonCollider2D**:
   - Check Is Trigger.
 - Create a prefab.
 - Add several Hammers and lay them out for the level.

<br>Hammer blinks red before despawning:

 - Create script Components/Death/**DeathEffectFlash**:

```csharp
using System.Collections;
using UnityEngine;

public class DeathEffectFlash : DeathEffect
{
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
      return lengthToFlashFor;
    }
  }

  public override void PlayDeathEffects()
  {
    StartCoroutine(FlashToDeath());
  }

  IEnumerator FlashToDeath()
  {
    SpriteRenderer[] spriteList 
      = GetComponentsInChildren<SpriteRenderer>();
    float timePassed = 0;
    bool isRed = false;
    while(timePassed < lengthToFlashFor)
    {
      SetColor(spriteList, isRed ? Color.red : Color.white);
      isRed = !isRed;

      yield return new WaitForSeconds(timePerColorChange);
      timePerColorChange = Mathf.Max(Time.deltaTime, timePerColorChange);
      timePassed += timePerColorChange;
      timePerColorChange *= colorChangeTimeFactorPerFlash;
    }
  }

  void SetColor(
    SpriteRenderer[] spriteList,
    Color color)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      sprite.color = color;
    }
  }
}
```

 - Add **DeathEffectFlash** to the hammer prefab, (it should automatically add DeathEffectManager as well).


<hr></details><br>
<details><summary>What did that do?</summary>

We sized the hammer to be about as large as the character.  You could go larger or smaller if you think that looks better.  

However we are using a polygon collider, which outlines the sprite art. In order for the hammer to kill an enemy later on, the hammer needs to make contact with the enemy before the character's body does.  If the hammer is too small, the character may start dying instead.

<img src="http://i.imgur.com/mfrIum0.png" width=300px /> 

3 were placed around the level.  Add as many as you'd like, but when positioning be sure that the hammer is in a location the player can get too.  It's frustrating for players if they see a hammer but can't ever reach it.

Picking up the hammer and killing enemies with it is covered in the next sections.


<br>Hammer blinks red before despawning:

DeathEffectFlash will start when another component triggers death on the GameObject (using DeathEffectManager).  Over a period of time the sprite will flash red faster and faster until the object dies.  

This is added to the hammer and the effect begins when SuicideIn's time completes.  When configuring the length of time a player has the hammer for, sum the SuicideIn time as well as the length to flash for configured in DeathEffectFlash.

The other fields in DeathEffectFlash may be used to control how quickly flash occurs as well as how it accelerates over time.  You could play with these values or modify the formula use in FlashToDeath to create your own effect.
<hr></details>
<details><summary>Why use pivot bottom?</summary>

We will be equipping the hammer on the character and have him swing.  Moving the pivot point to bottom sets it to approximately where the character will grip the hammer.  

When rotating the hammer for a swing, the bottom pivot causes the bottom of the handle to keep its position while the hammer's head swings.  The default middle pivot would create equal motion at the hammer's head and the base of the hammer's handle.

<img src="http://i.imgur.com/UUoyqJ3.gif" width=300px />

<hr></details>
<details><summary>Why use a polygon collider and not a box or capsule?</summary>

You could.  

The hammer's shape does not match either a Box or Capsule collider.  If you were to use one of those, the difference between the collider and the sprite art could be great enough that collisions in the game feel wrong.  e.g. you may miss picking up a hammer you thought you got or not kill an enemy you clearly hit.

The hammer's shape could be approximated well by using 2 box colliders.  A polygon collider does require more processing time, although not a significant difference, so this may be a potential optimization worth the tradeoff sacrificing some precision on collisions.  

<hr></details>
<details><summary>Why use Is Trigger?</summary>

When the character jumps for the hammer to pick it up, we do not want the character to bounce off of it.  The collider used on the hammer when the hammer is a pick up item shouldn't respond to anything expect equipping when the character touches it.  This is best achieved with 'Is Trigger'.

<hr></details>

<details><summary>Why not simply sum the time used in WaitForSeconds instead of max with deltaTime?</summary>

In the following example, we are requesting the coroutine sleep for a period of time:

```csharp
yield return new WaitForSeconds(timePerColorChange);
timePerColorChange = Mathf.Max(Time.deltaTime, timePerColorChange);
```

Unity does not make any guarantee that the amount of time before the coroutine resumes aligns with the wait time requested.  If we request a near zero time to wait, Unity will wait for a single frame -- we want to ensure that the effect progresses by at least that amount of time as well.

Additionally, this simplistic algorithm may drive the variable timePerColorChange to zero.  If that number got small enough, the loop would never terminate.  Ensuring that we progress by at least deltaTime each frame ensures that the loop will end.

Alternatively this method could be rewritten to use Time.timeSinceLevelLoaded.  With that we do not need to sum each iteration but instead can make decisions based off of the current time vs the time the effect began.

<hr></details>
<details><summary>Why use GetComponentsInChildren instead of a single sprite?</summary>

Flexibility.  Some use cases would work with GetComponent or GetComponentInChildren.  We get all the sprites in this GameObject and its children, and then update all so if something is composed of multiple sprites this script just works. 

<hr></details>

## 3.2) Equip the hammer

Add a script to the hammer and character, allowing the character to pickup the hammer and then kill enemies until it despawns.

<details><summary>How</summary>

 - Create script Components/Weapons/**WeaponHolder**:

```csharp
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
  public GameObject currentWeapon;
}
```

 - Add **WeaponHolder** to the Character.
 - Create script Components/Weapons/**Hammer**:

```csharp
using UnityEngine;

public class Hammer : MonoBehaviour
{
  [SerializeField]
  Vector2 positionWhenEquip = new Vector2(.214f, .17f);

  [SerializeField]
  Vector3 rotationWhenEquipInEuler = new Vector3(0, 0, -90);

  [SerializeField]
  MonoBehaviour[] componentListToEnableOnEquip;

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
      transform.localPosition = positionWhenEquip;
      transform.localRotation = Quaternion.Euler(rotationWhenEquipInEuler);

      for(int i = 0; i < componentListToEnableOnEquip.Length; i++)
      {
        MonoBehaviour component = componentListToEnableOnEquip[i];
        component.enabled = true;
      }
    }
  }
}
```

 - Select the Hammer prefab:
   - Add **SuicideIn**:
     - Time Till Death: 10
     - Disable the component.
   - Add **KillOnContactWith**:
     - Layers to kill: Enemy
     - Disable the component.
   - Add **Hammer**:
     - Add SuicideIn and KillOnContactWith to the list 'To Enable On Equip'.
 - Select the SpikeBall prefab:
   - Add **DeathEffectSpawn**:
     - GameObject to spawn: the Explosion prefab

<hr></details><br>
<details><summary>What did that do?</summary>

We create a weapon holder component to ensure we don't hold more than one weapon at a time.  When the weapon despawns (i.e. OnDestroy), we free up the Character's weapon holder so it can pick up another.

When the character picks up a hammer, the hammer becomes a child of the Character GameObject.  The hammer is then given a local position and rotation which represents where to grip the hammer relative to the character's feet (because the character has a bottom pivot point).  

When the hammer is equip, a list of components are enabled.  We use use this to make the necessary changes to switch this from a pickup item to a limited time killing machine.  

 - SuicideIn creates a timer till despawn.
 - KillOnContactWith enables killing enemies, previously disabled because it would be usual for the hammer as a pickup item to kill passers by.

<hr></details>
<details><summary>Could we reset the timer instead of preventing a second pickup?</summary>

Yes, in fact that would better match how most games would implement this feature.  There are various ways, as always, to achieve this. For example when the character touches a second hammer, you could:

 - Destroy the first and then simply allow the second to play out.  However the animation of the hammer swing may visibly skip.
 - Reset the SuicideIn countdown and Destroy the second hammer.

<hr></details>
<details><summary>Why serialize the rotation as Vector3 instead of Quaternion?</summary>

Quaternions are confusing for people.  This is why the Transform rotation is modified in the Inspector as an Euler.  Unfortunately when you ask Unity to expose a Quaternion in the Inspector it appears as X, Y, Z, W and not the Euler X, Y, Z like they did for the Transform.

You could switch to Quaternion, and it would be slightly more performant that way.  But I recommend using Euler, in case you ever want to modify the rotation used.

<hr></details>
<details><summary>What's localPosition / localRotation and how do they differ from position / rotation?</summary>

When modifying the Transform position - you can do so with either .position or .localPosition.  When the GameObject is a child of another GameObject these methods differ; they do the same thing when the GameObject has no parent.

 - .position: Sets the Transform position so that the GameObject appears at that location after considering the parent's Transform (position, rotation, and scale).
 - .localPosition: Sets the Transform position to the value specified.  If the GameObject has a parent, the parent's Transform will impact the final position you see in the scene.

<hr></details>


## 3.11) Create a GameController 

Create a singleton GameController to track points, lives, and hold global data such as the world size.

<details><summary>How</summary>

 - Create script Components/Controllers/**GameController**:

```csharp
using System;
using UnityEngine;

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

    DontDestroyOnLoad(gameObject);

    CalcScreenSize();
  }

  protected void Update()
  {
    CalcScreenSize();
  }

  void CalcScreenSize()
  {
    Vector2 screenSize = new Vector2(
          (float)Screen.width / Screen.height,
          1);
    screenSize *= Camera.main.orthographicSize * 2;
    screenBounds = new Bounds(
      (Vector2)Camera.main.transform.position,
      screenSize);
  }
}
```

  - Create a new GameObject named "GameController":
    - Add the **GameController** component.
    - Create a prefab for the GameController at Prefabs/**GameController**.

<br>Decrement lives when the character dies:

 - Create script Components/Death/**DeathEffectDecrementLives**:

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

 - Add **DeathEffectDecrementLives** to the Character.

<hr></details><br>
<details><summary>What did that do?</summary>

GameController holds the player's life count and points.  It uses DontDestroyOnLoad to maintain data between scenes (e.g. level 1 to level 2). And it's a singleton for easy access by other components.

We store the original life count so that we can add a feature later to reset when the player starts a new game.

An event, onLifeCounterChange, allows other components to react to the number of lives changing.

ScreenBounds was included in this class for other components to leverage without having to calculate the value multiple times.

<br>Decrement lives when the character dies:

When the character dies, the life count goes down by one. You can test this by looking at the life count go down in the GameController component (the value in the Inspector will update in real-time).

<hr></details>
<details><summary>What does DontDestroyOnLoad do?</summary>

DontDestroyOnLoad is a Unity method which marks a GameObject as independent from the scene you are in.  This means when we change scenes, the GameObject is not destroyed like everything else in the scene.

While in play mode, Unity moves the GameObject to a DontDestroyOnLoad section in the Hierarchy.

In order to simplify development, we will be putting a GameController GameObject in every scene -- as opposed to defining one in the world, maybe at the Main Menu or in Level 1 only.  This way when we test a specific scene, such as level 2, the GameController is available.  

To ensure only one GameController at a time, in Awake we destroy the extra GameController if one is already available.

<hr></details>
<details><summary>What's a singleton and why use it?</summary>

Singleton is a common design pattern.  When there is only going to be one of something, the singleton pattern provides an easy way of accessing that object from other scripts -- a public static 'instance'.

You could have used GameObject.Find (or one of its variations) instead.  Since several components will be accessing the GameController, using singleton here simplifies the code and improves performance a bit.

Here's a [good article about singleton from dotnetperls](https://www.dotnetperls.com/singleton).

<hr></details>



## 3.8) Restrict movement to stay on screen

<!-- Create a script which ensures entities can not walk off screen. -->


<details><summary>How</summary>

 - Create script Components/Movement/**KeepOnScreen**:

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
  }

  protected void FixedUpdate()
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

 - Add **KeepOnScreen** to both the Character and HoverGuy prefabs.

<hr></details>
<details><summary>What</summary>

When an entity attempts to move off screen, KeepOnScreen will position them back to the nearest on screen location.

When positioned by KeepOnScreen, an event is fired. This allows other components to add additional logic to be executed when an entity attempts to leave the screen. 

<hr></details><br>
<details><summary>Does setting the position cause the entity to pop on-screen?</summary>

Since this is checked every FixedUpdate, the teleporting effect does not cause popping on the screen.  Typically this has the impact of undoing the move which would have occurred if not for this script.

TODO

<hr></details>
<details><summary>Why use bounds for these checks?</summary>

There are a few ways you could check for an entity walking off the edge of the screen.  I choose to use the Unity bounds struct because it has methods which make the rest of this component easy.  Specifically:

 - Contains: Check if the current position is on the screen.
 - ClosestPoint: Return the closest point on screen for the entity, used when it is off-screen to teleport it back.

<hr></details>
<details><summary>What's the difference between setting transform.position and using myBody.MovePosition?</summary>

Updates to the Transform directly will teleport your character immediately and bypass all physics logic.  

Using the rigidbody.MovePosition method will interpolate (i.e. smoothly transition) the object to its new position and give consideration to other forces on that object.  It's very fast, but if you try and watch closely, MovePosition may animate a few frames on the way to the target position instead of going there immediately.

We are not suggesting one approach should always be used over the other - consider the use case and how you want your game to feel, sometimes teleporting is exactly the feature you're looking for.  

Be careful when you change position using either of these methods as opposed to using forces on the rigidbody.  It's possible that you teleport right into the middle of another object.  The next frame, Unity will try to react to that collision state and this may result in objects popping out in strange ways.

In this component we are setting transform.position for the teleport effect.  If rigidbody.MovePosition was used instead, occasionally issues may arise as MovePosition competes with other forces on the object.

</details>
<details><summary>Why use an event when another component could check screen bounds again?</summary>

TODO

When a GameObject is teleported by this script, an event is fired.  This event allows other components to add additional logic to be executed when an entity attempts to leave the screen.  For example, in the next section we will be asking the HoverGuy to turn around and start walking the other way.

2 reasons.

Encourage reuse.  If our definition of leaving the screen changes, it would be best if that was contained in a single script.  For example, ATM half of the entity's body goes off screen before we consider it to be out of bounds.  We may want to change that in the future to use the entity's collider bounds to ensure that the entire body stays visible.

It may not work reliably.  If both components checked screen bounds independently, the result may differ depending on which of those components executed first.  For example, KeepOnScreen may teleport you back on screen and then BounceOffScreenEdges would not consider you out of bounds (and therefore not turn you around.)  You could make this work by modifying the 'Script Execution Order', but I prefer reusing the KeepOnScreen component.

<hr></details>

## 3.9) HoverGuy turns around when reaching the edge

Create a script to have the HoverGuy bounce off the edge of the screen and never stop walking.

<details><summary>How</summary>

 - Create script Components/Movement/**BounceOffScreenEdges**:

```csharp
using UnityEngine;

[RequireComponent(typeof(KeepOnScreen))]
[RequireComponent(typeof(WalkMovement))]
public class BounceOffScreenEdges : MonoBehaviour
{
  WalkMovement walkMovement;

  protected void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
  }

  protected void Start()
  {
    KeepOnScreen keepOnScreen = GetComponent<KeepOnScreen>();
    keepOnScreen.onAttemptToLeaveScreen 
      += KeepOnScreen_onAttemptToLeaveScreen;
  }

  void KeepOnScreen_onAttemptToLeaveScreen()
  {
    TODO check direction , instead of flipping we go towards origin
    walkMovement.desiredWalkDirection
      = -walkMovement.desiredWalkDirection;
  }
}
```

 - Add **BounceOffScreenEdges** to the HoverGuy prefab.
 - Open menu Edit -> Project Settings -> Script Execution Order TODO -- NOO
   - Add **WalkMovement** and position it at the bottom of the list (positive number / below Default Time).

<hr></details><br>
<details><summary>What did that do?</summary>

The DestroyWhenPlayerDies component can be added to any GameObject to have it destroy itself when the player dies.  We use this on the HoverGuy and SpikeBall to clear enemies from the screen before respawning the character.

DestroyWhenPlayerDies uses Destroy, bypassing any DeathEffects.  We do this instead of using the DeathEffect pattern because we don't want a bunch of explosions spawning.

The spawner also inherits from PlayerDeathMonoBehaviour, restarting the SpawnEnemies coroutine.  We restart the spawner so that any initial wait time is executed again as well.  Additionally we may want to extend the spawner logic to do something like spawn faster the longer the player has been alive, which can also easily be reset by restarting the coroutine.



This component leverages the KeepOnScreen component to know when the entity attempts to walk off screen.  When hitting the edge, this will flip the entities desired walk direction causing it to start walking the opposite way.

<hr></details>


## 3.13) Clear and restart the level on death

Add scripts to respawn the character when he dies.

<details><summary>How</summary>

Respawn on Death:

 - Create script Components/Death/**PlayerDeathMonoBehaviour**:

```csharp
using UnityEngine;

public abstract class PlayerDeathMonoBehaviour : MonoBehaviour
{
  public abstract void OnPlayerDeath();
}
```

 - Create script Components/Controllers/**LevelController**:

```csharp
using UnityEngine;

public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;

  protected void OnEnable()
  {
    GameController.instance.onLifeCounterChange
      += Instance_onLifeCounterChange;

    StartLevel();
  }

  protected void OnDisable()
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

    BroadcastEndOfLevel();
 
    if(GameController.instance.lifeCounter <= 0)
    {
      isGameOver = true;
      YouLose();
    }
    else
    {
      StartLevel();
    }
  }

  public void YouWin()
  {
    if(isGameOver == true)
    { 
      return;
    }
    isGameOver = true;

    // TODO
  }

  void StartLevel()
  {
    Instantiate(playerPrefab);
  }

  void BroadcastEndOfLevel()
  {
    PlayerDeathMonoBehaviour[] gameObjectList 
      = GameObject.FindObjectsOfType<PlayerDeathMonoBehaviour>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      PlayerDeathMonoBehaviour playerDeath = gameObjectList[i];
      playerDeath.OnPlayerDeath();
    }

  }

  void YouLose()
  {
    // TODO
  }
}
```

 - Select the Character GameObject:
   - Position it over the door.
   - Create a prefab for the Character.
   - Delete the GameObject.
 - Add a GameObject named "LevelController":
   - Assign the Character prefab.

<br>Clear and restart the level on death:

 - Create script Components/Death/**DestroyWhenPlayerDies**:

```csharp
public class DestroyWhenPlayerDies : PlayerDeathMonoBehaviour
{
  public override void OnPlayerDeath()
  {
    Destroy(gameObject);
  }
}
```

 - Add **DestroyWhenPlayerDies** to the HoverGuy and the SpikeBall prefabs.
 - Update Components/Controllers/**Spawner**:

<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections;
using UnityEngine;

```

</details>

```csharp
public class Spawner : PlayerDeathMonoBehaviour
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
  public override void OnPlayerDeath()
  {
    StopAllCoroutines();
    StartCoroutine(SpawnEnemies());
  }
```

<details><summary>Existing code</summary>

```csharp
  IEnumerator SpawnEnemies()
  {
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
<details><summary>What did that do?</summary>

The LevelController is going to be responsible for starting and restarting a level.  It does this by instantiating a player and then broadcasting to all components which inherit from PlayerDeathMonoBehaviour when the level restarts.

The LevelController knows when a player dies by subscribing the life count in the GameController.  When the lives go down, we push the event to all components which inherit from PlayerDeathMonoBehaviour.

Any component may inherit from PlayerDeathMonoBehaviour to receive this event and perform whatever action is appropriate.  For example, enemies should die so we can have a clear level when the player respawns.

The LevelController also has placeholders for completing the level as well as for when the player is out of lives.

At the moment we do not a sequence which ends the game, so if the life count goes negative you stop spawning but the game never ends.

<br>Clear and restart the level on death:

The DestroyWhenPlayerDies component can be added to any GameObject to have it destroy itself when the player dies. We use this on the HoverGuy and SpikeBall to clear enemies from the screen before respawning the character.

DestroyWhenPlayerDies uses Destroy, bypassing any DeathEffects. We do this instead of using the DeathEffect pattern because we don't want a bunch of explosions spawning.

The spawner also inherits from PlayerDeathMonoBehaviour, restarting the SpawnEnemies coroutine. We restart the spawner so that any initial wait time is executed again as well. Additionally we may want to extend the spawner logic to do something like spawn faster the longer the player has been alive, which can also easily be reset by restarting the coroutine.

<hr></details>
<details><summary>Why does position before saving the prefab matter?</summary>

As a simplification, when the GameController spawns in the Character, we reuse the prefabs Transform position (and rotation/scale).  This is the default behaviour when you Instantiate from a prefab.

To be more flexible, we could have a default position for the Character defined somewhere for that level - allowing the spawn location to vary level to level.  

<hr></details>
<details><summary>Why not use an interface instead of abstract?</summary>

An interface would have been appropriate to use in this use case.  However Unity currently does not have an API for FindObjectsOfType for an interface.  You can work around this by getting all the GameObjects and then calling GetComponents, which does work with interfaces - but that is not an efficient solution.

<hr></details>
<details><summary>What does FindObjectsOfType do?</summary>

Unity offers a few similar calls allowing you to find all components attached to any GameObject in the scene.  

We are using FindObjectsOfType to get an array of every component which inherited from PlayerDeathMonoBehaviour.  This call won't return components on an inactive GameObject but you could use FindObjectsOfTypeAll if you needed that.

Unity's Find* calls are very slow.  You should not use this frequently, such as every Update.  Depending on the use case, you may be able to collect the information just once OnEnable, or only periodically like we do here only when the player dies.  

If you find the need to call Find* frequently, look for an alternative solution.  For example you may be able to create a static list of relevant references and have objects add/remove themselves as appropriate.

<hr></details>
<details><summary>Why not have all objects subscribe to life count changes instead or this new pattern?</summary>

There is a performance consideration, but this game likely would work fine either way.  I wanted to introduce another pattern for the tutorial to expose you to multiple possible solutions.

There is some overhead with subscribing and unsubscribing to events.  And as more and more objects subscribe to the same event, each sub and unsub is slower.  We are removing this overhead from the gameplay entirely by using this approach.  

Find* is much slower overall, but in this use case it does not happen until after gameplay has ended - so losing a frames would not be as impactful.

<hr></details>




## 3.15) Prevent enemies spawning on top of the character

Update the door so that it does not spawn if the character is too close.

<details><summary>How</summary>

 - Select the Door:
   - Add a **BoxCollider2D**:
     - Check 'Is Trigger'.
     - Size it to cover the entrance area.

<img src="http://i.imgur.com/Jq4rU93.png" width=300px />

 - Update Components/Controllers/**Spawner**:

<details><summary>Existing code</summary>

```csharp
using System;
using System.Collections;
using UnityEngine;

public class Spawner : PlayerDeathMonoBehaviour
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
  [SerializeField]
  ContactFilter2D contactFilter;

  Collider2D safeZoneCollider;

  Collider2D[] tempColliderList = new Collider2D[1];
  
  protected void Awake()
  {
    safeZoneCollider = GetComponent<Collider2D>();
  }
```

<details><summary>Existing code</summary>

```csharp
  protected void Start()
  {
    StartCoroutine(SpawnEnemies());
  }

  public override void OnPlayerDeath()
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

 - Select the EvilCloud and under the Spawner component:
   - Check Use Layer Mask
   - Layer Mask: Player

<img src="http://i.imgur.com/9oHr63R.png" width=150px />

<hr></details><br>
<details><summary>What did that do?</summary>

The collider we added defines the area to check for the character before allowing a spawn to happen.  We make this large enough to cover the entire entrance area so that there is never a HoverGuy which spawns in and instantly kills the character - leaving the player feeling cheated.

<hr></details>
<details><summary>What does OverlapCollider do?</summary>

In script, we check for the character by using OverlapCollider.  This is an on-demand way to check for colliders in the area.  The contact filter filters results to only consider the character, so another HoverGuy in the area does not stop the spawner as well.  

We could have chosen to use OnTriggerEnter and OnTriggerExit instead - this approach was chosen both because it's simple and works well for this use case, and because it exposes us to multiple different techniques during this tutorial.

</details>
<details><summary>Why use a contact filter instead of a tag or a layermask?</summary>

You could but it may change how we interact with Unity here.  OverlapCollider answers our question of if the character is in the area, and it accepts a ContactFilter2D.

ContactFilter2D may be used to filter results on various dimensions when making calls such as OverlapCollider.  LayerMask is the only one we are interested in here.

<hr></details>
<details><summary>Does the Collision Matrix impact anything when using OverlapCollider?</summary>

No.  The collision matrix as defined under the Physics 2D settings only impacts the real-time collisions from Unity.  Calls such as OverlapCollider do not assume the same restrictions that may have been applied in the collision matrix.  This provides a lot of flexibility for different mechanics.

If you do want to use the same LayerMask as defined in the collision matrix, you can ask Unity for that with the following:

```csharp
LayerMask myLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
```

<hr></details>
<details><summary>Why use a temp collider list?</summary>

For performance reasons, the OverlapCollider method from Unity takes an array and then adds data to it -- as opposed to returning an array with the data requested (as they do for calls such as GetComponents).  They do this because calls like this may occur frequently and the overhead of creating a new array each time may become a bottleneck.

We create the array once and then pass the same one every time we make the call to OverlapCollider.

For this component, we don't actually need the data itself.  We only want to know if any objects overlap or not.  For this reason, we never read anything from the tempColliderList -- we only consider the number of results (the return value for that method).

Unity also uses the array we pass in to define the max number of results it should discover.  That is why our temp array has a length of 1 and not 0.

<hr></details>


## 3.16) Add points for jumping over enemies

Add a collider and script to award points anytime the character jumps over an enemy.

<details><summary>How</summary>

 - Create a new Layer for "Points" and disable everything except for Points / Player collisions.

<img src="http://i.imgur.com/5sxuf2I.png" width=150px />

 - Create script Code/Components/Effects/**AwardPointsOnJumpOver**:

```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AwardPointsOnJumpOver : MonoBehaviour
{
  [SerializeField]
  int pointsToAward = 100;

  [SerializeField]
  float cooldownTime = 3;

  BoxCollider2D myCollider;

  [SerializeField]
  ContactFilter2D contactFilter;

  RaycastHit2D[] tempHitList = new RaycastHit2D[1];

  float lastPickupTime;

  int playerLayer;

  protected void Awake()
  {
    myCollider = GetComponent<BoxCollider2D>();
    playerLayer = LayerMask.NameToLayer("Player");
  }

  protected void OnTriggerStay2D(
    Collider2D collision)
  {
    if(Time.timeSinceLevelLoad - lastPickupTime < cooldownTime)
    {
      return;
    }

    int count = Physics2D.Raycast(
      transform.parent.position, 
      Vector2.up, 
      contactFilter, 
      tempHitList);

    if(count > 0
      && tempHitList[0].collider.gameObject.layer == playerLayer)
    {
      GameController.instance.points += pointsToAward;

      lastPickupTime = Time.timeSinceLevelLoad;
    }
  }
}
```

- Add the HoverGuy and SpikeBall to scene and for each:
  - Add a new empty GameObject as a child:
    - Name it "Points".
    - Add **AwardPointsOnJumpOver**:
      - Check 'Use Triggers'
      - Check 'Use LayerMask'
      - LayerMask: 'Player' and 'Floor'
    - Assign it the Points layer.
    - Add a **Rigidbody2D**:
      - Change the Body Type to 'Kinematic'.
   - Add a **BoxCollider2D**
     - Check Is Trigger
     - Size the collider to capture the area above the entity.

<img src="http://i.imgur.com/gmMDJlD.png" width=150px />

 - Apply changes to the prefabs and delete the GameObjects.

<br>Hold rotation on the point collider:

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

  protected void FixedUpdate()
  {
    transform.rotation = originalRotation;
  }
}
```

 - Add **HoldRotation** to the Points GameObject under the SpikeBall prefab.


<hr></details><br>
<details><summary>What did that do?</summary>

We added a large collider above the enemy to detect when the player is above us.  The script AwardPointsOnJumpOver awards points if the player is directly above vs having a platform between them.  A cooldown to prevents the player from doubling up on points with a single jump.

<br>Hold rotation on the point collider:

Each FixedUpdate, we set the rotation back to the original. We add this to the points child on the SpikeBall to ensure we are always checking for the player straight up.

Without this, the points collider would spin with the parent ball.

<hr></details>
<details><summary>What's Raycast do?</summary>

Raycast projects a line and returns colliders intersecting with it (in order, closest first).  There are other 'cast' calls to project different shapes when needed, e.g. BoxCast.

When Raycasting, there are various options available.  Here we provide an origin point for the line and the direction its pointing.  The contact filter defines which objects to include in the results - when using Raycast, it does not consider your configuration in the collision matrix.

<hr></details>
<details><summary>Why Trigger AND Raycast?</summary>

The trigger informs us when there is a player above the enemy.  However, this does not consider any platforms which are also above us.  The raycast is used to determine what is directly above the enemy, and we only award points if it's the player.

Ultimately the raycast here answers the question of when to award points.  We could raycast each frame in an update loop, but instead leverage the trigger to improve performance by only checking when the player is near.

<hr></details>
<details><summary>Why add another Rigidbody2D / why check the collision layer manually?</summary>

When you are using a child GameObject, adding another Rigidbody2D will ensure that physics events from the child do not reach the parent.  i.e. any scripts on the parent would not get an OnTriggerEnter or OnCollisionStay call for a collider on the child this way -- in this tutorial the KillOnContact script may trigger much too soon without the second Rigidbody2D.

The second Rigidbody2D does not prevent events on the parent from reaching any scripts on the child GameObject.  In AwardPointsOnJumpOver, after a trigger we will raycast to confirm the player is directly above us - with this the additional events from the parent do not impact gameplay.

<hr></details>
<details><summary>Do we need a cooldown?</summary>

Yes, as the code is currently written.  Removing the cooldown would result in huge payouts as the player jumped over.  

This could be addressed other ways.  Consider exactly when you would want to award more points for jumping over an enemy. e.g. we allow you to move back and forth while in the air - if I did this over an enemy, should I get paid twice?

<hr></details>
<details><summary>Why FixedUpdate instead of Update here?</summary>

Update runs each frame.  Changing the Transform each Update may be appropriate when you are making changes the player will see.  

FixedUpdate runs every x ms of game time.  Changing the Transform each FixedUpdate can be used to impact the physics, such as collision detection. 

It is possible for FixedUpdate to happen twice between Updates.  For this use case, we are only interested in freezing the position for the purpose of trigger enter events.  If we were to change the transform each Update, we would be checking for collisions with some rotation.  That said, this probably would not be noticeable for this use case - just noting that using Update instead FixedUpdate is a tiny bit incorrect.

<hr></details>


## 3.18) Test

GG

# Next chapter

[Chapter 4](https://github.com/hardlydifficult/Platformer/blob/master/Chapter4.md).
