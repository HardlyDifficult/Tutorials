# 6) UI and Scene transitions

TODO intro

## Add a win condition

The goal of the game is to save the beautiful mushroom.  For level 1, that means getting close - but before you actually reach it the evil cloud is going to carry the shroom up to level 2.  

Here we detect the end of the game, but the cloud animation will be added later in the tutorial.

<details open><summary>How</summary>

 - Create an empty GameObject named "WinArea".
   - Add a **BoxCollider2D** sized to cover the area that when entered will end the level.
     - Check Is Trigger.
   - Create a Layer "WinArea":
     - Configure the collision matrix to only support WinArea <-> Player collisions.
     - Assign the layer to the WinArea GameObject.
   - Add a sprite to lure the character to the win area.  We are using **spritesheet_jumper_26** with Order in Layer -3.
     - Make it a child of the WinArea. 

<img src="http://i.imgur.com/WuW9hPk.png" width=300px />

 - Create script Code/Components/Effects/**TouchMeToWin**:

```csharp
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  static int totalNumberActive;

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
    if(enabled == false)
    {
      return;
    }

    enabled = false;
    if(totalNumberActive == 0)
    {
      GameObject.FindObjectOfType<LevelManager>().YouWin();
    }
  }
}
```

 - Add **TouchMeToWin** to the WinArea.

<hr></details><br>
<details open><summary>What did that do?</summary>

We put a large trigger collider around the mushroom.  When the character enters this area, TouchMeToWin will end the level.  The collider is configured to use a layer which only interacts with the player so enemies cannot accidentally end the level.

TouchMeToWin counts the total number of these special zones in the world.  For level 1 we are only using one but for level 2 there will be more.  When the last one is disabled (by the character entering that area), we call YouWin on the LevelManager which will own starting the end sequence / switching to level 2.

We check if the TouchMeToWin component is enabled before processing the trigger enter so that an area does not call YouWin multiple times.

<hr></details>


## Win animation

When the character reaches the win area, play the animation which

<details open><summary>How</summary>

 - Create another animation for the evil cloud, Animations/**CloudLevel1Exit** to play when the player wins.
   - You may not be able to record if the Timeline window is open.
   - Select Animations/CloudLevel1Exit and disable Loop Time.
 - Right click in Assets/Animations -> Create -> Timeline named **Level2Exit**.
   - Select the evil cloud's sprite GameObject and in the Inspector change the Playable Director's 'Playable' to Level2Exit.

<img src="http://i.imgur.com/Jsah6Ll.png" width=300px />

 - In the Timeline window, click 'Add' then 'Animation Track' and select the evil cloud's child GameObject with the animator.
 - Right click in the timeline and 'Add Animation From Clip' and select the CloudLevel1Exit animation.

<img src="http://i.imgur.com/xcR7HWr.gif" width=300px />

 - Select the box which appeared for the animation, and in the Inspector modify the speed.
   - Hit play in the Timeline to preview the speed.  The value is going to depend on how you created the animation.
 - Select the mushroom GameObject and drag it into the timeline.
   - Adjust the timeframe so that it starts at the beginning of the timeline and ends when you want the mushroom to disappear.
   - Select the track's row and in the Inspector change the 'Post-playback state' to 'Inactive'.

<img src="http://i.imgur.com/W9lejAB.png" width=300px />

 - Drag in the evil cloud's child GameObject and create another Activation track.
   - Size this track to fit the entire timeline.
   - Change the Post-playback state to Inactive.
 - Select the evil cloud's sprite GameObject and in the Inspector change the Playable Director's Playable back to Level1Entrance.
 - Update LevelManager:

<details open><summary>Existing code</summary>

```csharp
using UnityEngine;
```

</details>

```csharp
using UnityEngine.Playables; 
```

<details open><summary>Existing code</summary>

```csharp
public class LevelManager : MonoBehaviour
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
  PlayableAsset endOfLevelPlayable; 
```

<details open><summary>Existing code</summary>

```csharp
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
    director.Play(endOfLevelPlayable); 
```

<details open><summary>Existing code</summary>

```csharp
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
  }

  void RestartLevel()
  {
    PlayerDeathMonoBehaviour[] gameObjectList 
      = GameObject.FindObjectsOfType<PlayerDeathMonoBehaviour>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      PlayerDeathMonoBehaviour playerDeath = gameObjectList[i];
      playerDeath.OnPlayerDeath();
    }
    Instantiate(playerPrefab);
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
<details open><summary>What did that do?</summary>

When the Character reaches the win area, the Evil Cloud plays its end of level animation.  

<hr></details>
<details open><summary>Why switch the Playable when editing Timelines?</summary>

Unity 2017 is the first release of Timeline, it's still a work in progress.  

At the moment you cannot edit Timelines unless they are active in the scene.  You can only partially view the Timeline by selecting the file.  So anytime you want to modify the Level1Exit Timeline, you need to change the Playable Director and then when you are complete change it back.

On a related note, you can't edit an animation if the Timeline window is open.  When working with Animations and Timelines, it seems to work best if you only have one open at a time.

<hr></details>

## Stop everything when the level is over

When the level is over, stop the spawners and freeze the character and enemies while the evil cloud animation plays.

<details open><summary>How</summary>

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

 - Select the Character prefab.
   - Add **DisableComponentsOnEndOfLevel** and to the components list, add:
     - Rigidbody2D.
     - PlayerController.
     - The character's animator.  You can do this by:
       - Open a second Inspector by right click on the Inspector tab and select Add Tab -> Inspector.
       - With the Character's parent GameObject selected, hit the lock symbol in one of the Inspectors.
       - Select the character's child sprite, then drag the Animator from one Inspector into the other.

<img src="http://i.imgur.com/UOEJNyx.gif" width=500px />

 - Unlock the Inspector.
 - Select the Fly Guy prefab.
   - Add **DisableComponentsOnEndOfLevel** and add the rigidbody and animator.
 - Select the Spike Ball prefab.
   - Add **DisableComponentsOnEndOfLevel** and add the rigidbody.
 - For each the Evil cloud and Door:
   - Add **DisableComponentsOnEndOfLevel** and add the spawner.
 - Update LevelManager to call DisableComponentsOnEndOfLevel:


<details open><summary>Existing code</summary>

```csharp
using UnityEngine;

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
    DisableComponentsOnEndOfLevel[] disableComponentList 
      = GameObject.FindObjectsOfType<DisableComponentsOnEndOfLevel>();  
    for(int i = 0; i < disableComponentList.Length; i++)
    {
      DisableComponentsOnEndOfLevel disableComponent = disableComponentList[i];
      disableComponent.OnEndOfLevel();
    }
```

<details open><summary>Existing code</summary>

```csharp
  }

  void RestartLevel()
  {
    PlayerDeathMonoBehaviour[] gameObjectList 
      = GameObject.FindObjectsOfType<PlayerDeathMonoBehaviour>();
    for(int i = 0; i < gameObjectList.Length; i++)
    {
      PlayerDeathMonoBehaviour playerDeath = gameObjectList[i];
      playerDeath.OnPlayerDeath();
    }
    Instantiate(playerPrefab);
  }

  void YouLose()
  {
    // TODO
  }
}
```

</details>


<hr></details><br>
<details open><summary>What did that do?</summary>

At the end of the level, the LevelManager calls each DisableComponentsOnEndOfLevel component. This component then disables other components to make the game freeze during our end of level animation.

 - Entities disable their rigidbody to stop gravity and the animator to stop playback.
 - The Character also disables the PlayerController so that input does not cause the sprite to flip facing direction.
 - Spawners stop the spawn coroutine so no more enemies appear.

<hr></details>
<details open><summary>Why not just set timeScale to 0?</summary>

You could, but some things would need to change a bit.

We don't want everything to pause.  The evil cloud animation needs to progress.  If you change the timeScale, you will need to modify the Animators to use Unscaled time -- otherwise the animations would not play until time resumed.

<hr></details>

## Create a new empty scene

Create a new scene which will be used for level 2.  Add both levels to the build settings.

<details open><summary>How</summary>

 - Add scene to build settings with menu File -> Build Settings:
   - Click "Add Open Scenes" to add the current scene (level 1).
 - Create a new scene with File -> New Scene.
   - Save it as Assets/Scenes/**Level2**.
   - Add level 2 to the Build Settings.
 - Double click Assets/Scenes/Level1 to return to that scene.

<hr></details><br>
<details open><summary>What did that do?</summary>

We have a separate scene to manage each level.  By adding these to the build settings, we are informing Unity that these scenes should be made available -- allowing us to transition to one either by name or by index (their position in the build settings list).

<hr></details>
<details open><summary>Why not use just one scene for the game?</summary>

You could.  But I would not advise it.

Using multiple scenes, one for each level, makes it easy to customize the layout and behaviour for the level.  Technically this could all be achieved in a single scene but that could make level design confusing.

GameObjects which are shared between levels can use a prefab so that they have a common definition.  With a prefab, you can make a modification and have that change impact every instance.  You can also override a setting from a prefab for a specific use case, such as making enemies move faster in level 2.

<hr></details>

## Scene transition

After the level ends, load level 2.

<details open><summary>How</summary>

 - Create script Code/Components/Controllers/**ChangeScenePlayable**:

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
<details open><summary>What's SceneManager.LoadScene do?</summary>

Calling LoadScene will Destroy every GameObject in the scene, except for any which are DontDestroyOnLoad like our GameController, and then load the requested scene.

The scenes available to load are defined in Build Settings.  You must add scenes you want to load there.  Once in Build Settings you can load a scene by its filename, as we do here ('Level2'), or you can load by index (the order of the scene in build settings.)


<hr></details>

## UI for points

Display the number of points in the top right.

<details open><summary>How</summary>

 - In the Heirarchy, right click select UI -> Text.
   - This creates a Canvas and a Text GameObject.
 - Select the "Text" GameObject:
   - Name it "Points".
   - Change the anchor top right (you may need to zoom out a lot).

<img src="http://i.imgur.com/xPFe8kV.png" width=300px />   

 - Change the Paragraph Alignment to Right.
 - Use the move tool to position the text in the top right.
 
<img src="http://i.imgur.com/r7g1W7y.png" width=500px />

 - Change the color to white.
 - Change the font.  We are using kenpixel_future.
 - Increase the font size to 32 (text may disapear).  
 - Increase the height to 40 (text should be too large at this point).
 - Increase the width to 500.
 - Use the scale tool to scale down until its a good size.
 - Use the move tool to reposition the text.
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
  }

  protected void Update()
  {
    int currentPoints = GameController.instance.points;
    int deltaPoints = currentPoints - lastPointsDisplayed;
    if(deltaPoints > 0)
    {
      float pointsTarget =
        Mathf.Lerp(lastPointsDisplayed, currentPoints, scrollSpeed * Time.deltaTime);
      int pointsToDisplay = Mathf.CeilToInt(pointsTarget);
      text.text = pointsToDisplay.ToString("N0");
      lastPointsDisplayed = pointsToDisplay;
    }
  }
}
```

 - Add **TextPoints** to the Points GameObject.

<hr></details><br>
<details open><summary>What did that do?</summary>

A canvas was created to hold the text for points, we'll add more to this canvas soon.  We anchor the text to the top right and position it in the corner.  We set the font size too large and then scale down to size to get a crisp display.

<hr></details>
<details open><summary>What's a canvas do?</summary>

The Canvas is a container holding UI.  It allows Unity to manage features such as automatically scaling UI to fit the current resolution.  Unity offers components such as the VerticalLayoutGroup which help in getting positioning and sizing correct.

<hr></details>
<details open><summary>Why size the font too large and then scale it down?</summary>

Fonts by default may look blurry.  We size the font too large and then scale it down via the RectTransform to fit in order to make the rendering more clear for users.

Here is an example, the top is sized only using font size while the bottom is oversized and then scaled down:

<img src="http://i.imgur.com/qLqSeRV.png" width=300px />

<hr></details>
<details open><summary>What is a RectTransform, how does it differ from a Transform?</summary>

A RectTransform is the UI version of the Transform used for GameObjects.  RectTransform inherhits from Transform, adding features specifically for UI positioning such as pivot points and an anchor.

<hr></details>
<details open><summary>Why use ceiling here?</summary>

We need to ensure that each iteration of Update increases the points displayed by at least one, if we are not already displaying the final value.  Without this, it's possible each Update would calculate less than 1 - if we simply cast that means that each update would progress by 0 and therfore never actually display the correct amount.

<hr></details>
<details open><summary>What does setting the anchor point on UI do?</summary>

Setting the anchor changes how the position for the Rect

Pivot point - defined in %.

<hr></details>

## UI for lives

<details open><summary>How</summary>

TODO

<hr></details><br>
<details open><summary>TODO</summary>

TODO

<hr></details>

## Main menu

<details open><summary>How</summary>

TODO

<hr></details><br>
<details open><summary>TODO</summary>

TODO

<hr></details>

## Settings panel

<details open><summary>How</summary>

TODO
Volume slider and keyboard remapping?

<hr></details><br>
<details open><summary>TODO</summary>

TODO

<hr></details>


## Volume

<details open><summary>How</summary>

TODO

<hr></details><br>
<details open><summary>TODO</summary>

TODO

<hr></details>

## Scene between level

<details open><summary>How</summary>

TODO

<hr></details><br>
<details open><summary>TODO</summary>

TODO

<hr></details>


# Next chapter

[Chapter 7](https://github.com/hardlydifficult/Platformer/blob/master/Chapter7.md).
