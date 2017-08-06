# 6) UI and Scene transitions

TODO intro

## Add a win condition

The goal of the game is to save the beautiful mushroom.  For level 1, that means getting close - but before you actually reach it the evil cloud is going to carry the shroom up to level 2.  

Here we detect the end of the game, the cloud animation will be added later in the tutorial.

<details><summary>How</summary>

Design the win area:

 - Create an empty GameObject named "WinArea".
   - Add a **BoxCollider2D** sized to cover the area that when entered will end the level.
     - Check Is Trigger.
   - Create a Layer "WinArea":
     - Configure the collision matrix to only support WinArea <-> Player collisions.
     - Assign the layer to the WinArea GameObject.
   - Add a sprite to lure the character to the win area.  We are using **spritesheet_jumper_26** with Order in Layer -3.
     - Make it a child of the WinArea. 

<img src="http://i.imgur.com/WuW9hPk.png" width=300px />

<br>Inform the LevelController when the player won:

 - Create script Components/Effects/**TouchMeToWin**:

```csharp
using System;
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  static int totalNumberActive;

  [SerializeField]
  MonoBehaviour componentToEnableOnTouch;

  int playerLayer;

  protected void Awake()
  {
    playerLayer = LayerMask.NameToLayer("Player");
  }

  protected void OnEnable()
  {
    totalNumberActive++;
  }

  protected void OnDisable()
  {
    totalNumberActive--;
  }

  protected void OnTriggerEnter2D(
    Collider2D collision)
  {
    if(enabled == false 
      || collision.gameObject.layer != playerLayer)
    {
      return;
    }

    if(componentToEnableOnTouch != null)
    {
      componentToEnableOnTouch.enabled = true;
    }

    enabled = false;
    if(totalNumberActive == 0)
    {
      GameObject.FindObjectOfType<LevelController>().YouWin();
    }
  }
}
```

 - Add **TouchMeToWin** to the WinArea.

<hr></details><br>
<details><summary>What did that do?</summary>

Design the win area:

We put a large trigger collider around the mushroom.  When the character enters this area, it will trigger the end the level.  The collider is configured to use a layer which only interacts with the player so enemies cannot accidentally end the level.

<br>Inform the LevelController when the player won:

TouchMeToWin counts the total number of these special zones in the world.  For level 1 we are only using one but for level 2 there will be more.  When the last one is disabled (by the character entering that area), we call YouWin on the LevelController which will own starting the end sequence / switching to level 2.

An enabled check is included to ensure we an area does not call YouWin multiple times.

<hr></details>


## Win animation

When the character reaches the win area, play a Timeline to animate the end of the level.

<details><summary>How</summary>

Create a win animation:

 - Create another animation for the evil cloud, Animations/**CloudLevel1Exit** to play when the player wins.
   - You may not be able to record if the Timeline Editor window is open.
   - Select Animations/CloudLevel1Exit and disable Loop Time.

<br>Create a win Timeline:

 - Right click in Assets/Animations -> Create -> Timeline named **Level2Exit**.
   - Select the evil cloud's sprite GameObject and in the Inspector change the Playable Director's 'Playable' to Level2Exit.

<img src="http://i.imgur.com/Jsah6Ll.png" width=300px />

 - In the Timeline Editor window, click 'Add' then 'Animation Track' and select the evil cloud's child GameObject with the animator.
 - Right click in the timeline and 'Add Animation From Clip' and select the CloudLevel1Exit animation.

<img src="http://i.imgur.com/xcR7HWr.gif" width=300px />

 - Select the box which appeared for the animation, and in the Inspector modify the speed.
   - Hit play in the Timeline Editor to preview the speed.  The value is going to depend on how you created the animation.

<br>Hide the mushroom during the animation:

 - Select the mushroom GameObject and drag it into the timeline.
   - Adjust the timeframe so that it starts at the beginning of the timeline and ends when you want the mushroom to disappear.
   - Select the track's row and in the Inspector change the 'Post-playback state' to 'Inactive'.

<img src="http://i.imgur.com/W9lejAB.png" width=300px />

 - Select the evil cloud's sprite GameObject and in the Inspector change the Playable Director's Playable back to Level1Entrance.

<br>Start the Timeline at the end of the level:

 - Update **LevelController**:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;
```

</details>

```csharp
using UnityEngine.Playables; 
```

<details><summary>Existing code</summary>

```csharp
public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;
```

</details>

```csharp
  [SerializeField]
  PlayableDirector director; 

  [SerializeField]
  PlayableAsset TimelineEventPlayable; 
```

<details><summary>Existing code</summary>

```csharp
  [SerializeField]
  int levelNumber = 1; 

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
```

</details>

```csharp
    director.Play(TimelineEventPlayable); 
```

<details><summary>Existing code</summary>

```csharp
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
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

</details>

 - Configure the director and set the end of level playable to Level1Exit.

<hr></details><br>
<details><summary>What did that do?</summary>

Create a win animation:

Another animation was created to play when the player wins.  We leave it up to you what this looks like and how long the animation plays for.  

<br>Create a win Timeline:

A new Timeline is created for the win sequence.  We add the animation just created and adjust the speed as needed.

<br>Hide the mushroom during the animation:

An Activation Track is used to hide the mushroom when the animation is nearly complete.  Setting the post-playback state to inactive ensures that the mushroom does not return when the Timeline completes.

<br>Start the Timeline at the end of the level:

When the win condition is triggered, the LevelController changes the Evil Cloud's Playable Director to play the end of level Timeline just created.

<hr></details>
<details><summary>Why switch the Playable when editing Timelines?</summary>

Unity 2017 is the first release of Timeline, it's still a work in progress.  

At the moment you cannot edit Timelines unless they are active in the scene.  You can only partially view the Timeline by selecting the file.  So anytime you want to modify the Level1Exit Timeline, you need to change the Playable Director and then when you are complete change it back.

On a related note, you can't edit an animation if the Timeline window is open.  When working with Animations and Timelines, it seems to work best if you only have one open at a time.

<hr></details>

## Stop everything when the level is over

When the level is over, stop the spawners and freeze the character and enemies while the evil cloud animation plays.

<details><summary>How</summary>

Create a script to disable certain mechanics:

 - Create script Components/Controllers/**DisableComponentsOnEndOfLevel**:

```csharp
using UnityEngine;

public class DisableComponentsOnEndOfLevel : MonoBehaviour
{
  [SerializeField]
  Component[] componentsToDisable;

  public void OnEndOfLevel()
  {
    for(int i = 0; i < componentsToDisable.Length; i++)
    {
      Component component = componentsToDisable[i];
      if(component is Rigidbody2D)
      {
        Rigidbody2D myBody = (Rigidbody2D)component;
        myBody.simulated = false;
      }
      else if(component is Behaviour)
      {
        Behaviour behaviour = (Behaviour)component;
        behaviour.enabled = false;
        if(behaviour is MonoBehaviour)
        {
          MonoBehaviour monoBehaviour = (MonoBehaviour)behaviour;
          monoBehaviour.StopAllCoroutines();
        }
      }
      else
      {
        Destroy(component);
      }
    }
  }
}
```

<br>Configure disabling for GameObjects:

 - Select the Character prefab.
   - Add **DisableComponentsOnEndOfLevel** and to the components list, add 3 items:
     - Its Rigidbody2D.
     - Its PlayerController.
     - The character's animator (which is on the child GameObject).  You can do this by:
       - Open a second Inspector by right click on the Inspector tab and select Add Tab -> Inspector.
       - With the Character's parent GameObject selected, hit the lock symbol in one of the Inspectors.
       - Select the character's child sprite, then drag the Animator from one Inspector into the other.

<img src="http://i.imgur.com/UOEJNyx.gif" width=500px />

 - Unlock the Inspector.
 - Select the Fly Guy prefab.
   - Add **DisableComponentsOnEndOfLevel**, and add its Rigidbody2D and Animator.
 - Select the Spike Ball prefab.
   - Add **DisableComponentsOnEndOfLevel** and add its Rigidbody2D.
 - For the Evil Cloud and the Door:
   - Add **DisableComponentsOnEndOfLevel** and add its Spawner.

<br>Call scripts at the end of the level:

 - Update Components/Controllers/**LevelController**:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;

public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;

  [SerializeField]
  int levelNumber = 1; 

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

    director.Play(TimelineEventPlayable);
```

</details>

```csharp
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
```

<details><summary>Existing code</summary>

```csharp
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

</details>

<hr></details><br>
<details><summary>What did that do?</summary>

Create a script to disable certain mechanics:

This script exposes a public method to be called when the level ends.  It will disable a list of components, typically on the same GameObject or a child GameObject.

Depending on the type of component, our approach to 'disabling' differs.

<br>Configure disabling for GameObjects:

At the end of the level, the LevelController will call each DisableComponentsOnEndOfLevel component. This component then disables other components on the GameObject to make the game freeze during our end of level animation.

 - Entities disable their rigidbody to stop gravity and the animator to stop playback.
 - The Character also disables the PlayerController so that input does not cause the sprite to flip facing direction.
 - Spawners stop the spawn coroutine so no more enemies appear.

<br>Call scripts at the end of the level:

When the LevelController detects the win condition, it's updated to call each of the DisableComponentsOnEndOfLevel components in the scene.

<hr></details>
<details><summary>Why not just set timeScale to 0?</summary>

You could, but some things would need to change a bit.

We don't want everything to pause.  The evil cloud animation needs to progress.  If you change the timeScale, you will need to modify the Animators to use Unscaled time -- otherwise the animations would not play until time resumed.

<hr></details>
<details><summary>Why not just destroy all the components instead?</summary>

Destroying a component is an option.  Once destroyed, that component stops but the rest of the GameObject is still in-tact.

Errors occur if we attempt to destroy the components mentioned above due to other components requiring the ones we removed.  If we wanted to switch to destroying components instead, we would need to be more selective in which components are included to avoid dependancy issues.  Because of this, it's simplier to disable than destroy.

<hr></details>
<details><summary>What's rigidbody simulated=false do?</summary>

Setting simulated to false on the rigidbody effectively disables the component.  The rigidbody does not support an 'enabled' flag like scripts do - 'simulated' is their equivalent.

<hr></details>
<details><summary>What's the lock symbol do?</summary>

Many of the windows in Unity have a lock symbol in the top right.  Clicking this will freeze the selection for that window.  So if you select a GameObject you can freeze the Inspector, allowing you to continue navigating other files while still having that same GameObject's properties displayed in the Inpsector.

This is handy for various things such as above where we want one GameObject to reference another GameObject's component.  Open two Inspectors, select the first GameObject and lock one of the Inspector windows... now you can select the other GameObject and you have one Inspector for each.

<hr></details>

## Create a new empty scene

Create a new scene which will be used for level 2.  Add both levels to the build settings.

<details><summary>How</summary>

 - Add scene to build settings with menu File -> Build Settings.
   - Click "Add Open Scenes" to add the current scene (level 1).
 - Create a new scene with File -> New Scene.
   - Save it as Assets/Scenes/**Level2**.
   - Add level 2 to the Build Settings.
 - Double click Assets/Scenes/Level1 to return to that scene.

<hr></details><br>
<details><summary>What did that do?</summary>

We have a separate scene to manage each level.  By adding these to the build settings, we are informing Unity that these scenes should be included in the build -- allowing us to transition to one either by name or by index (their position in the build settings list).

<hr></details>
<details><summary>Why not use just one scene for the game?</summary>

You could.  But I would not advise it.

Using multiple scenes, one for each level, makes it easy to customize the layout and behaviour for the level.  Technically this could all be achieved in a single scene but that could make level design confusing.

GameObjects which are shared between levels can use a prefab so that they have a common definition.  With a prefab, you can make a modification and have that change impact every instance.  You can also override a setting from a prefab for a specific use case, such as making enemies move faster in level 2.

<hr></details>

## Scene transition

After the level ends, load level 2.

<details><summary>How</summary>

 - Create script Playables/**ChangeScenePlayable**:

```csharp
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class ChangeScenePlayable : BasicPlayableBehaviour
{
  [SerializeField]
  string sceneNameToLoad;

  public override void OnBehaviourPlay(
    Playable playable, 
    FrameData info)
  {
    base.OnBehaviourPlay(playable, info);

    SceneManager.LoadScene(sceneNameToLoad);
  }
}
```

 - Change the Evil Cloud Director to Level1Exit and open the Timeline.
   - Drag the **ChangeScenePlayable** script into the Timeline.
   - Position it to start after the animation completes.  The size of the box does not matter.
 - Change the Evil Cloud Director back to Level1Entrance.

<hr></details><br>
<details><summary>What did that do?</summary>

ChangeScenePlayable allows us to define when to load the next scene in the Timeline directly.  This is handy as we are designing the end sequence so that we don't need to manage a countdown that aligns with our animations.

</details>
<details><summary>What's SceneManager.LoadScene do?</summary>

Calling LoadScene will Destroy every GameObject in the scene, except for any which are DontDestroyOnLoad like our GameController, and then loads the requested scene.

The scenes available to load are defined in Build Settings.  You must add scenes you want to load there.  Once in Build Settings you can load a scene by its filename, as we do here ('Level2'), or you can load by index (the order of the scene in build settings.)

<hr></details>

## UI for points

Display the number of points in the top right.

<details><summary>How</summary>

Create and position the points text:

 - In the Heirarchy, right click create UI -> **Text**.
   - This creates a Canvas and a Text GameObject.
 - Select the "Text" GameObject:
   - Name it "Points".
   - Pivot: (1, 1)
   - Paragraph Alignment: Right
   - Anchor: Top right

<img src="http://i.imgur.com/xPFe8kV.png" width=300px />   

 - Use the move tool to position the text in the top right (you may need to zoom out a lot).
 
<img src="http://i.imgur.com/r7g1W7y.png" width=500px />

<br>Style the text:

 - Select the Text GameObject:
   - Color: white
   - Font: kenpixel_future
   - Font size: 32 (text may disapear)
   - Height: 40 (text should be too large)
   - Width: 500
   - Use the scale tool to scale down until its a good size.

<br>Update the text when the player earns points:

 - Create script Components/UI/**TextPoints**:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class TextPoints : MonoBehaviour
{
  [SerializeField]
  float scrollSpeed = .1f;

  Text text;

  int lastPointsDisplayed;

  protected void Awake()
  {
    text = GetComponent<Text>();

    Debug.Assert(text != null);
  }

  protected void Update()
  {
    int currentPoints = GameController.instance.points;
    int deltaPoints = currentPoints - lastPointsDisplayed;
    if(deltaPoints > 0)
    {
      float speed = scrollSpeed * Time.deltaTime;
      float pointsTarget =
        Mathf.Lerp(lastPointsDisplayed, currentPoints, speed);
      int pointsToDisplay = Mathf.CeilToInt(pointsTarget);
      text.text = pointsToDisplay.ToString("N0");
      lastPointsDisplayed = pointsToDisplay;
    }
  }
}
```

 - Add **TextPoints** to the Points GameObject.

<hr></details><br>
<details><summary>What did that do?</summary>

Create and position the points text:

A canvas was created to hold the text for points, we'll add more to this canvas soon.  We set the anchor and pivot to the top right and position the text in the corner of the canvas.

<br>Style the text:

Kenpixel_future is a fixed width font, which makes the points look a little better as the values are changing.  We set the font size too large and then scale down to size to get a crisp display.

<br>Update the text when the player earns points:

TextPoints uses Lerp to scroll the number of points displayed up until reaching the current value.  This means if the player earns 100 points, we may see 10 the first frame and 17 the second frame, 20 the third, etc where the number of points increasing each frame slows down as it approaches the actual value.

<hr></details>
<details><summary>What's a canvas do and why is our level so small in comparision?</summary>

The Canvas is a container holding UI.  It allows Unity to manage features such as automatically scaling UI to fit the current resolution.  Unity offers components such as the VerticalLayoutGroup which help in getting positioning and sizing correct.

Canvas appears in the Scene window along side other objects in the game.  It's huge, and overlaps the world center a little.  This is an arbitrary decision from Unity - the Canvas is actually completely separate from the rest of the game.  I believe they choose to display this way as a simplification so you don't need another window for editing.

You can use the Layers button in the editor to hide UI if you prefer, allowing you to just look at the game or level design.

<img src="http://i.imgur.com/ewCoCiB.png" width=300px />

<hr></details>
<details><summary>Why size the font too large and then scale it down?</summary>

Fonts by default may look blurry.  We size the font too large and then scale it down via the RectTransform to fit in order to make the rendering more clear for users.

Here is an example, the top is sized only using font size while the bottom is oversized and then scaled down:

<img src="http://i.imgur.com/qLqSeRV.png" width=300px />

<hr></details>
<details><summary>What is a RectTransform, how does it differ from a Transform?</summary>

A RectTransform is the UI version of the Transform used for GameObjects.  RectTransform inherhits from Transform, adding features specifically for UI positioning such as pivot points and an anchor.  Anything displayed in a Canvas must use a RectTransform... as that is how Canvas does layout and positioning.

<hr></details>
<details><summary>Why use ceiling here?</summary>

We need to ensure that each iteration of Update increases the points displayed by at least one, if we are not already displaying the final value.  Without this, it's possible each Update would calculate less than 1 - if we simply cast that means that each update would progress by 0 and therfore never actually display the correct amount.

<hr></details>
<details><summary>What does setting the anchor point / pivot on UI do?</summary>

Setting the anchor changes how the position for the Rect is determined.  The default is center, which means places (0, 0) at the center of the screen.  The unit for these coordinates is pixels.  

As the screen size changes, the offset from the anchor point is still defined in pixels.  If we positioned the points with a center anchor, it would not be position correctly when the resolution changed.

Pivot point is the spot in the GameObject which is used for positioning against the anchor.  It is defined in percent of the object's size, 0 to 1.  So if we have an anchor point of top right and the pivot is center (.5, .5) than the position (0, 0) will center the object in the corner, causing half of it to be offscreen.  Switch the pivot point to (1, 1) and the entire object is visible.

Unity also offers the Canvas Scaler component on the Canvas GameObject which can be used to automatically update position and sizing when the resolution changes.

<hr></details>
<details><summary>What's C# ToString("N0") do?</summary>

ToString is available on all types in C#.  When using ToString to convert a number, you may optionally include format codes like this.  "N0" is a common one.

 - "N" states it should formatted as a number, with commas in the states and periods in Europe, etc (e.g. 12,000,000).
 - "0" means any decimal places should not be included (e.g. 1000.234 would display as 1,000).

There are a lot of options when it comes to generating strings.  Read [more from Microsoft here](https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types).

<hr></details>

## UI for lives

Add sprites to display how many lives remain.

<details><summary>How</summary>

Add sprites for lives:

 - Add an Empty GameObject as a child to the Canvas, named "Lives".
   - Add **HorizontalLayoutGroup**:
     - Spacing: 30
     - Child Alignment: Upper Right
     - Uncheck Child Force Expand Width
 - Add an **Image** to the Canvas as well, named "Life".
   - Change the Source Image.  We are using **spritesheet_jumper_62**.
   - Copy / paste Life so that there are 3.
 - Position the Lives GameObject under the Points.

<img src="http://i.imgur.com/yZXrKUG.png" width=150px />

<br>Animate hiding the life sprite on death:

 - Create script Components/UI/**LifeLine**:

```csharp
using System;
using UnityEngine;

public class LifeLine : PlayerDeathMonoBehaviour
{
  [SerializeField]
  int lifeCount = 1;

  public override void OnPlayerDeath()
  {
    if(GameController.instance.lifeCounter < lifeCount)
    {
      DeathEffectManager.PlayDeathEffectsThenDestroy(gameObject);
    }
  }
}
```

 - Select each of the Life GameObjects (all 3).
   - Add **LifeLine**:
     - Change the lifeCount for each so that the first is 3, the second 2, and the last 1.
   - Add **DeathEffectThrob**.

<hr></details><br>
<details><summary>What did that do?</summary>

Add sprites for lives:

3 sprites were added to represent the number of lives remaining.  The  HorizontalLayoutGroup is used to position the sprites -- this approach is optional, there are other ways you could have achieved the same layout.

Animate hiding the life sprite on death:

When the player dies, LifeLine triggers DeathEffects on itself if the player just lost the life point that sprite represents.  DeathEffectThrob causes the sprite to animate its death by scaling up and down and getting smaller until its gone.

<hr></details>
<details><summary>How does the HorizontalLayoutGroup work?</summary>

The Horizontal Layout Group places its child GameObjects next to each other, side by side. There are various options for controlling the layout, such as:

 - Spacing: Adds padding between each of the child GameObjects.
 - Child Alignment: Defines if the child GameObjects should appear in the center, left, or right, etc of this GameObject.
 - Child Force Expand: Causes the child GameObjects to get wider, filling the entire parent GameObject.  This appears as whitespace between objects.

<hr></details>
<details><summary>Why an Image and not a Sprite?</summary>

Image is essentially a special kind of sprite with a RectTransform, to be used with a Canvas.  The Canvas and its associated components, such as the HorizontalLayoutGroup, only work with GameObjects that have a RectTransform.

<hr></details>
<details><summary>What does Child Force Expand Width?</summary>

Force Expand Width will automatically increase the Spacing so that the Images fill the entire container.  If we were to use this, and get things positioned correctly by modifying the RectTransform width - it may look correct at the start but once one of the lives is destroyed, the others would re-layout to fill that gap... and that would look wrong.

<hr></details>


## Main menu

Create a main menu to show at the start of the game.  Allow the player to start Level 1 and when the game is over, return to the menu.

<details><summary>How</summary>

Create the Mene scene:

 - Create a new Scene, save it as Scenes/**Menu**.
   - Add the Scene to Build Settings.
     - Drag and drop it so that it is the first scene in the list.
 - Add the GameController prefab.

<br>Design the scene:

 - Add a Platform sprite to the bottom.
   - Add **BoxCollider2D**.
   - Layer: **Floor**
 - Add the Character prefab.
   - Add **WanderWalkController**.
   - Add **BounceOffScreenEdges**.
   - Remove the **PlayerController**.

<img src="http://i.imgur.com/QCrcf66.png" width=150px />

 - Add the Evil Cloud sprite
   - Create an animation to loop, named Animations/**MenuCloud**.
   - Adjust the playback speed in the Animator Controller.

<img src="http://i.imgur.com/dM4LFPk.png" width=300px />

<br>Add a play button:

 - Create UI -> Button, named "Play".
 - Select the Canvas GameObject:
   - Canvas Scaler UI Scale Mode: **Scale with Screen Size**
 - Select the Play GameObject:
   - Change the Source Image.  We are using **spritesheet_tiles_22**.
   - Position the button on the menu screen.
 - Select the Text GameObject under Play.
   - Text: "Play"
   - Color: black
   - Font Size: 50
   - RectTransform Top: about -22 so the text is positioned well on the sign.
    
<img src="http://i.imgur.com/bDZ5dr5.png" width=150px />

 - Create script Components/UI/**ButtonChangeScene**:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonChangeScene : MonoBehaviour
{
  [SerializeField]
  string sceneName;

  public void OnClickLoadScene()
  {
    SceneManager.LoadScene(sceneName);
  }
}
```

 - Select the Play GameObject:
   - Add **ButtonChangeScene** and enter "Level1" for the scene name.
   - Under the button component, create a new OnClick event.

<img src="http://i.imgur.com/bGdqYZK.png" width=150px />

 - Drag and drop the ButtonChangeScene component onto the click event object box and then select the OnClickLoadScene event.

<img src="http://i.imgur.com/8EHUfAd.gif" width=300px />

<br>Return to the menu after losing:

 - Update Components/Controllers/**LevelController**:

<details><summary>Existing code</summary>

```csharp
using UnityEngine;
using UnityEngine.Playables;
```

</details>

```csharp
using UnityEngine.SceneManagement;
```

<details><summary>Existing code</summary>

```csharp
using UnityEngine;
using UnityEngine.Playables;
```

<details><summary>Existing code</summary>

```csharp
using UnityEngine.SceneManagement;
```

</details>

```csharp

public class LevelController : MonoBehaviour
{
  [SerializeField]
  GameObject playerPrefab;

  protected bool isGameOver;

  [SerializeField]
  PlayableDirector director; 

  [SerializeField]
  PlayableAsset TimelineEventPlayable;

  [SerializeField]
  int levelNumber = 1; 

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

    director.Play(TimelineEventPlayable);

    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
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
```

</details>

```csharp
    SceneManager.LoadScene("Menu"); 
```

<details><summary>Existing code</summary>

```csharp
  }
}
```

</details>

<hr></details><br>
<details><summary>What did that do?</summary>

Create the Mene scene:

A scene for the Menu was added as the first scene in build settings so that it's what you see first when starting the game.  

<br>Design the scene:

A simple platform was added the bottom for the character to walk on.  The character prefab is reused but we modify the configuration, swapping the PlayerController for the random movement components we used on FlyGuy.

<br>Add a play button:

When the button was added, a Canvas was automatically created.  Canvas was configured to Scale with Screen Size so that the button looks the same at all resolutions.

ButtonChangeScene exposes a public method that we wire up to be called by Unity's Button component when the button is clicked.

<br>Return to the menu after losing:

The LevelController was updated, leveraging the YouLose placeholder created earlier to return to the menu once the player is out of lives.

<hr></details>
<details><summary>Does order matter for scenes in the Build Settings?</summary>

The first enabled scene in Build Settings list is what appears first when playing the game.  Drag and drop scenes to change their order in that list.

You can disable scenes in Build Settings by unchecking the box, this excludes that scene from the build.  You can also select and hit Delete.

The order beyond the first does not matter for anything except for the index ID they are assigned.  When loading a scene you can either load by name or by index.  

I prefer using the name, as code is easier to follow.  You might also consider using an enum to define each scene in the correct order.  This way it's easier to maintain code if scene names or the order changes.

<hr></details>
<details><summary>Why Remove Component instead of disable it?</summary>

Either way should work.  I find it more clear to remove the component instead of just leaving it disabled as it's easier to understand what's happening with that GameObject.  Several times in this tutorial we have GameObjects with components which are disabled by default - all of them may be enabled if the right use case triggers it.  So removing the component clearly indicates there is no PlayerController in the menu, vs maybe there is a hidden way of enabling it.

<hr></details>
<details><summary>How does the Canvas Scaler / Scale with Screen Size work?</summary>

The Canvas Scaler controls the size of UI elements on the screen.  The default is constant pixel size which means that as the resolution gets larger, the relative size of UI is smaller (i.e. it does not scale up).  We are using Scale with Screen Size with makes UI elements bigger the bigger the screen is.

<hr></details>
<details><summary>How do UI events / button OnClick events work?</summary>

When an event occurs, such as OnClick for buttons, you can execute any number of methods.  Hit plus to add another event to call.  

To call an event, you first select the GameObject you want to operate on.  Once selected, each of the components on the GameObject are selectable from the event list.

Often you will be calling an event on the same object like we did here.

<hr></details>

## Level 2

Create level 2 reusing a lot from level 1 but change various configurations such as having the cloud spawn Fly Guy enemies.  

The win condition for this level, to be added later, will be to jump over each of the breakaway platforms we add here.  

<details><summary>How</summary>

Create prefabs from Level 1 to reuse:

 - Open Level1 and create prefabs for:
   - Main Camera
   - Canvas
   - EventSystem
   - 1 Platform (any is fine, we will use this as a starting point in Level2).
   - 1 Ladder
   - Evil Cloud
   - LevelController

<br>Start to design level 2 with prefabs from level 1:

 - Open Level2.
 - Delete the Main Camera.
 - Drag in the following prefabs:
   - Main Camera
   - Canvas
   - EventSystem
   - Evil Cloud
   - LevelController
     - Level Number: 2
     - Select the Director
 - Add the Platform, Ladder, Hammer prefabs and any new art you would like to include.  
   - Copy / paste as needed to layout the level.
   - Note that it's okay to 'Break the prefab instance' while making changes.
   - Add a **Rigidbody2D** to each of the center platforms.
     - Freeze the Position and Rotation.
 - Add the Mushroom as well.  We are using **spritesheet_jumper_26**.
   - Add **PolygonCollider2D**.
   - Add **Rigidbody2D**:
     - Freeze the Position (X and Y) and Rotation.
    
<img src="http://i.imgur.com/7UiA4df.png" width=300px />

<br>Add a section at the top where the Character cannot enter:

 - Add a Block at the top, we are using **spritesheet_ground_9**.
   - Add **Rigidbody2D**.
     - Freeze the Position and Rotation.
   - Add **BoxCollider2D**.
 - Create a layer 'CharacterOnly':
   - Disable CharacterOnly / Enemy and CharacterOnly / Feet collisions.
   - Assign it to all the blocks at the top and the mushroom.

<img src="http://i.imgur.com/af8Jpj0.png" width=300px />

<br>Add the breakaway sections:

  - Add a Breakaway GameObject and sprite, we are using **spritesheet_jumper_69**.
    - Set to Layer **Floor**.
    - Add **TouchMeToWin**.
    - Add **PolygonCollider2D** for collisions.
    - Add **Rigidbody2D**.
    - Add **BoxCollider2D**, set it as a trigger and size it to capture the area above.

<img src="http://i.imgur.com/vttLU0g.png" width=300px />

<br>Configure the enemy:

 - Drag the FlyGuy prefab into the scene.
   - Rename it "FlyGuy2".
   - Remove the **FadeInThenEnable** component.
   - Enable the **WanderWalkController**:
     - Time Before First Wander: 0
   - Change the **RandomClimbController**
     - Odds of going up: .1
     - Odds of going down: .9
   - Create a new prefab for FlyGuy2 and delete the GameObject.
 - Select the Evil Cloud and change the Thing To Spawn to FlyGuy2.

<br>Create the intro Timeline:

 - Select the Evil Cloud's sprite GameObject and create a new animation  Animations/**CloudLevel2Entrance**.
   - Record any sequence you'd like.
   - Select Animations/CloudLevel2Entrance and disable looping.
 - Create a 'Timeline' file at Animations/**Level2Entrance**.
 - Select the Evil Cloud's sprite GameObject and change the Playable Director's Playable to Level2Entrance.
 - Open the Timeline Editor window:
   - Add an Animation Track for the Evil Cloud 
     - Add an Animation Clip for CloudLevel2Entrance.
     - Update the speed if needed.
   - Add Activation Tracks for the Hammers, Ladders, and LevelController.  
     - Time them to start near the end of the animation.
     - And end at the end of the timeline.
   - Then disable the Hammers, Ladders, and LevelController.

<br>Create the outro Timeline:

 - Create a new Scene named Scenes/**YouWin**:
   - Add it to Build Settings.
   - Return to Level2.
 - Create a new animation on the Evil Cloud for the end of the game, named Animations/**CloudLevel2Exit**.
 - Create a new Timeline Animations/**Level2Exit** and select it in the Playable Director.
 - Open the Timeline Editor:
   - Create an **Animation Track** for the Evil Cloud's CloudLevel2Exit clip.
     - Adjust the speed.
   - Add **TimelineEventPlayable**:
     - Position it to start about half way through the animation.
     - Change the Event Type to End.
   - Add **ChangeScenePlayable**:
      - Position it to start a few seconds after the TimelineEventPlayable began.
      - Change the Scene Name to "YouWin".
 - Select the LevelController and change the End of Level Playable to Level2Exit.
 - Switch the Playable Director back to Level2Entrance.

<hr></details><br>
<details><summary>What did that do?</summary>

Create prefabs from Level 1 to reuse:

We create prefabs to save time creating Level 2. Some of these will be used with the same configurations as used in Level 1, others will be modified specifically for level 2.  

<br>Start to design level 2 with prefabs from level 1:

We construct most of level 2 by reusing GameObjects created for level 1, making customizations where needed. The layout is a lot different from level 1 but we are reusing the same core Platforms.  Level 2 will have a unique win condition, which is why we did not copy that from level 1.

Rigidbody was added to the center platforms and the mushroom.  We then constrain the body, effectively disabling it by default.  This will allow us to turn off the constraints when the player beats the level, causing them to fall to the ground.

<br>Add a section at the top where the Character cannot enter:

The win condition for this level is breaking each of the breakaway blocks.  We don't want you to be able to reach the mushroom as you did in level 1.  Blocks are added to gaurd the mushroom and a layer is used to allow enemies to pass through but block the player.

The rigidbody is added so this may also fall at the level's end.

<br>Add the breakaway sections:

To beat level 2 you need to jump or walk over each of the breakaway platforms.  

 - The layer floor allows the FloorDetector to work while standing on these platforms.
 - TouchMeToWin counts down the number of breakaway platforms remaining in order to trigger the end of the level.  
 - The polygon collider is used for collisions as entities walk over and for when it's falling.
 - The rigidbody is added so this may fall at the level's end.
 - The box collider is used to detect when the Character is jumping or walking over.

<br>Configure the enemy:

A new prefab was created specifically for level 2.  It's a slight modification to the settings on the Fly Guy we used in level 1.

 - FadeInThenEnable is removed so that the enemy starts moving as soon as it's dropped from the cloud.
 - WanderWalkController removes the initial sleep so it does not always walk right in the beginning.
 - RandomClimbController updates odds so that enemies travel down more often then up.

<br>Create the intro Timeline:

A new Timeline was created for the start of the level.  It's modeled after the Timeline used with level 1. 

 - The cloud is given a new animation to start with for this level.
 - We disable the Hammers, Ladders, and LevelController until the animation is near complete, like we had done with level 1.

<br>Create the outro Timeline:

The Timeline for the end of the level is also modeled after level 1.  

 - The cloud is given a new animation to end with.
 - TimelineEventPlayable broadcasts the end of the level to other interested components.
 - The Timeline ends with ChangeScene, taking us to the YouWin scene.

<hr></details>
<details><summary>What does it mean to 'Break the prefab instance'?</summary>

This dialog sounds more serious than it is.  Breaking the prefab instance means that Unity will no longer tie this GameObject to a prefab - so if the prefab were to change the GameObject will not receive the update.  

The prefab itself is still in-tact and may be used for other objects or scenes.

<hr></details>
<details><summary>When freezing the rigidbody a warning appears, why do it this way?</summary>

When you freeze all constraints on the rigidbody, Unity presents a warning that this may not be an efficient way to achieve your goal of preventing the object from moving.  We have a bit of a unique use case for this case -- we will be removing these constraints once the end of the level is reached, allowing them to fall to the ground.

Alternatively you could have not included the rigidbody at all until the end of the level.  This is style preference, as well as a bit of a performance consideration as there is overhead to having a frozen rigidbody and there is overhead with adding a new component to a GameObject.

<hr></details>


## Level 2 breakaway sequence

Once the character has touched each of the Breakaway platforms, make the level collapse.

<details><summary>How</summary>

Enable physics, causing the level to collapse:

 - Create script Components/Effects/**UnfreezeAndDisablePlatformers**:

```csharp
using UnityEngine;

public class UnfreezeAndDisablePlatformers : MonoBehaviour
{
  protected void OnEnable()
  {
    Rigidbody2D myBody = GetComponent<Rigidbody2D>();
    myBody.constraints = RigidbodyConstraints2D.None;

    PlatformEffector2D effector 
      = GetComponent<PlatformEffector2D>();
    if(effector != null)
    {
      effector.enabled = false;
    }
  }
}
```

 - For each Block gaurding the mushroom, each platform in the center (except the bottom platform), and the mushroom:
   - Add **UnfreezeAndDisablePlatformers**.
     - Disable the component.
   - Add **EnableComponentsOnTimelineEvent**:
     - Event: End
     - Component list: UnfreezeAndDisablePlatformers

<br>Breakaway blocks fall when touched:

 - For each breakaway block:
   - Add **UnfreezeAndDisablePlatformers**.
   - Update TouchMeToWin to enable the Unfreeze component.

<hr></details><br>
<details><summary>What did that do?</summary>

Enable physics, causing the level to collapse:

The Timeline which plays when the level ends will enable UnfreezeAndDisablePlatformers.  That component will then:

 - Remove all constraints on the rigidbody, allowing it to fall.
 - Disable the platform effector, if there is one.  This ensures there is no weird behaviour due to the one-way collisions from the platformer effect while the platform itself is falling and spinning.

<br>Breakaway blocks fall when touched:

When the player jumps over one of the breakaway blocks, UnfreezeAndDisablePlatformers is used to cause it to fall.

<hr></details>

## GG screen

Design a scene to display when the player wins the game.  Here we drop a bunch of random "GG"s from the sky.

<details><summary>How</summary>

Configure scene:

 - Open the YouWin scene.
 - Configure the Camera color.
 - Add the GameController prefab.

<br>Create a GG object:

 - Create an Empty GameObject named "GG".
   - Add **TextMesh**:
     - Text: "GG"
     - Font Size: 36
     - Anchor: Middle Center
     - Alignment: Center
   - Add **BoxCollider2D**:
     - Size it tightly around the GG letters.
   - Add **Rigidbody2D**.
   - Add **SuicideIn** and set the time to 30.

<br>Randomize the GG:

 - Create script Components/Effects/**RandomGG**:

```csharp
using UnityEngine;

public class RandomGG : MonoBehaviour
{
  [SerializeField]
  float minScale = .1f;

  [SerializeField]
  float maxScale = .7f;

  protected void OnEnable()
  {
    transform.localScale 
      = Vector3.one * UnityEngine.Random.Range(minScale, maxScale);

    TextMesh text = GetComponent<TextMesh>();
    text.color = UnityEngine.Random.ColorHSV();

    Bounds screenBounds = GameController.instance.screenBounds;
    transform.position = new Vector3(
      UnityEngine.Random.Range(screenBounds.min.x, screenBounds.max.x),
      screenBounds.max.y + 10,
      0);
  }
}
```

 - Add **RandomGG** to the GG GameObject.

<br>Keep the GG in bounds:

 - Create an Empty GameObject and add a **BoxCollider2D**
   - Size and position multiple to gaurd the screen edges.

<img src="http://i.imgur.com/KI8JXHK.png" width=300px />

<br>Spawn GGs:

- Create Prefabs/**GG** and delete the GameObject.
- Create an Empty GameObject named Spawner.
  - Add **Spawner** component
    - Initial wait time: 0
    - Min time: .1
    - Max time: 1
    - Thing to spawn: GG

<br>Press any key to return to the menu:

 - Create script Components/UI/**AnyKeyToLoadScene**:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyToLoadScene : MonoBehaviour
{
  [SerializeField]
  string sceneName = "Menu";

  protected void Update()
  {
    if(Input.anyKeyDown)
    {
      SceneManager.LoadScene(sceneName);
    }
  }
}
```

 - Add **AnyKeyToLoadScene** to the Spawner.

<hr></details><br>
<details><summary>What did that do?</summary>

Configure scene:

The YouWin scene is given a black background.  The GameController is added so that components can get the screen bounds from it, it's a DontDestroyOnLoad component but added here to ease testing.  

<br>Create a GG object:

A GameObject is created to display "GG".  We size it too large to start and will scale it down.  A box collider surrounds the letters so we can drop a bunch and have them bounce off eachout.  The rigidbody enables gravity.  Suicide in destroys the GG after 30 seconds, ensuring there is always some movement on the screen.

<br>Randomize the GG:

RandomGG will, when the GameObject is first added to the scene:

  - Pick a new random size, always smaller than the original (to ensure a crisp font).
  - Pick a random color.
  - Pick a random position which is a bit above the screen.

<br>Keep the GG in bounds:

Invisible bumpers were added off screen in order to get the GG objects to collect on screen.  They will bounce around a bit and some GGs will be completely off screen, but most should be visible.

<br>Spawn GGs:

The Spawner component used in our levels was reused here to spawn GGs.  The position of the Spawner does not matter as the RandomGG script will override the GameObject's position.  The time between spawns was greatly reduced, giving a pretty steady stream of GGs falling.

<br>Press any key to return to the menu:

Each update, the AnyKeyToLoadScene script checks if any key was pressed that frame.  We check AnyKeyDown instead of AnyKey so that the player does not acidentally skip the GG scene entirely.

This script could have been added to any GameObject in the world, using the Spawner was an arbitrary choice.

<hr></details>
<details><summary>Why a TextMesh instead of UI Text?</summary>

UI Text does not work with physics.  It's intended to be used on a Canvas and not have any interaction with objects in the world.  

Text Mesh can be added to a GameObject, allowing you to add a rigidbody for gravity and a collider to get them bouncing around.  Note that features built for the UI Text component, for example the Outline component, are not compatible with the Text Mesh.

<hr></details>


# The End.

TODO words.