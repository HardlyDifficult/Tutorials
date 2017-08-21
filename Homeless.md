Include an ETA per chapter.


Major changes:
 
 - Sprite sheets
 - Ladders change layer instead of disable 
   - Layers: Enemy, EnemyOnLadder, Player, PlayerLadder, Feet, FeetOnLadder



TODO      - Disable Feet / Player.

 - Select HoverGuy:
   - Add **DeathEffectSpawn**:
     - GameObject to Spawn: the Explosion prefab
   - Add **KillOnContactWith**:
     - Layers to kill: Player



[YouTube]() | [Source before]() | [Source after]()

Code changes
 - Delete SpriteExtensions
 - RotateOvertimeToOriginal renamed rotationFactor
   - Add FAQ about yield break;
   - Add FAQ on Quaternion lerp
 - Rename LevelManager to LevelController
 - Rename Suicide to Destroy WhenPlayerDies and OutOfBounds
  - FAQ on why a sub class TimelineEventPlayable.EventType
  - Configure ladderFilter on LadderMovement

 Add a 'Rigidbody2D' component to the spike ball.
 - Create a script **Spawner** under Assets/Code/Components/Life and paste the following:


Existing code sections need to remove HR.

Add it to an empty parent GameObject named "Character".

TODO: Long sections with more than x bullets need subtitles to split things up
TODO: End of script sections may be shortened a bit
TODO: Remove all comments and Asserts inline, link to file with comments.
 - ToString("N0")

Concepts to include on first mention
 - Class vs struct
 - Auto properties
 - Smart properties

TODO: No sprite sheets!  just sprites.

Image sizes: 50, 150, 300, 500, 700.  Prefer: 300 

'Script'


 - Update **PlayerController**.cs by adding the code below (or copy/paste the full version - TODO link):

How:
<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>What did that do?</summary>

TODO

<hr></details>
<details><summary>TODO</summary>

TODO

<hr></details>


 - Create script Code/Components/Movement/**TODO**:

```csharp
TODO
```


<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>

```csharp
```

<details><summary>Existing code</summary>

```csharp
```

<hr></details>





TODO  - Why do some scripts have a check to disable and others do not. (add to first instance)


TODO paypal, patreon, and twitch links.
Check for HRs
TODO
 - Image size is all over the place
 - say links are included when relevant er something.
 - Target character count on scripts is 65
 - Table of contents with anchors
 - Maybe copy paste for a fully expanded view all in one page (but note gifs make it not print friendly)
 - Lots of FAQs along the way, please consider these questions as you go, we try not to be redundant later on.
 - Stop saying cached for perf.
 - Post questions/comments under the youtube video (since git doesn't do discussions well). Maybe survey at end?
 - Brief bio / why i did this
 - Filenames in bold
 - Maybe always have a 'Why' or 'What' question after how.














Familiarize yourself with the Unity Editor a bit.  This [guide from Unity](https://docs.unity3d.com/Manual/LearningtheInterface.html) is a nice, quick overview.







More about special folders:

Everything under a folder named "Editor" is an editor script, including files in any subdirectories.  e.g., this script could be saved as Assets/Editor/AutoSave.cs or Assets/Code/Editor/Utils/AutoSave.cs  There are two special folder names that work anywhere in your folder hierarchy like this:
 - "[Editor](https://docs.unity3d.com/Manual/ExtendingTheEditor.html)": These scripts are only executed when in the Unity Editor (vs built into the game).
 - "[Resources](https://docs.unity3d.com/ScriptReference/Resources.html)": Assets bundled with the game, available for loading via code.  Note Unity [recommends using resources only for prototypes](https://unity3d.com/learn/tutorials/temas/best-practices/resources-folder) - the preferred solution is [AssetBundles](https://docs.unity3d.com/Manual/AssetBundlesIntro.html). 
 
The following are additional special folder names only considered when in the root Assets directory. e.g., Assets/Gizmos is a special directory but Assets/Code/Gizmos could hold anything.
 - "[Standard Assets](https://docs.unity3d.com/Manual/HOWTO-InstallStandardAssets.html)": These are optional assets and scripts available from Unity that you can add to your project anytime by right clicking in the Assets directory and selecting from 'Import Package'.
 - "[Editor Default Resources](https://docs.unity3d.com/ScriptReference/EditorGUIUtility.Load.html)": A resources directory only available to Editor scripts so you can have assets appear in the editor for debugging without increasing the game's built size.
 - "[Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html)": Editor-only visualizations to aid level design and debugging in the 'Scene' view.
 - "[Plugins](https://docs.unity3d.com/Manual/Plugins.html)": Dlls to include, typically from 3rd party libraries.
 - "[StreamingAssets](https://docs.unity3d.com/Manual/StreamingAssets.html)": Videos or other archives for your game to stream.

Be sure you do not use folders with these names anywhere in your project unless specifically for that Unity use case. e.g., Assets/Code/Editor/InGameMapEditor.cs may be intended to be part of the game but would be flagged as an Editor only script instead.







<details><summary>How might you play mulitple audio clips at the same time?</summary>

Each AudioSource can be configured for one clip at a time.  To play multiple clips in parallel, you could use multiple AudioSources by placing multiple on a single GameObject or instantiating multiple GameObjects.  You can also use the following API to play a clip in parallel:

```csharp
GetComponent<AudioSource>().PlayOneShot(clip);
```

This will start playing another clip, re-using an existing AudioSource component (and its GameObject's position as well as the audio configuration options such as pitch).

<hr></details>


<details><summary>Why FindObjectsOfType<GameObject> followed by GetComponents instead of FindObjectsOfType<ICareWhenPlayerDies>?</summary>

Unity does not support FindObjectsOfType by interface.  This is unexpected because they do support GetComponentsInChildren by interface (and similiar methods).  

As a workaround, we are getting every GameObject and then checking for components on each that implement ICareWhenPlayerDies.

This is not a performant solution.  However the use case is also one which does not occur often, so the performance hit should be fine.  

If we needed to do something similiar frequently (e.g., every frame), we would want to add caching or take a completely different approach.

<hr></details>




## 1.3) Slice sprite sheets

Slice each of the sprite sheets in order to access the individual sprites within.


<details><summary>How</summary>

- Select a sprite sheet in the 'Project' (such as Assets/Art/**spritesheet_ground**).
- In the 'Inspector', set 'Sprite Mode' to 'Multiple'.

<img src="https://i.imgur.com/duYuVMy.png" width=300px />

- Click 'Sprite Editor' and apply changes when prompted.
- Click the 'Slice' menu item (see below for the type to use per sprite).
  - If all the sprites in the sheet are the same size, use Grid By Cell Count and enter the number of sprites in the sheet horizontally (columns) and vertically (rows).
  - If the sprites in the sheet are various sizes, use Automatic.

<img src="https://i.imgur.com/3wLWBZG.png" width=300px />

<img src="https://i.imgur.com/d3XzhRU.png" width=300px />

- Click 'Slice' button.
- Close the Sprite Editor and apply changes when prompted.
- Repeat for each sprite sheet.

We are using:
 - **spritesheet_ground**: Cell Count (8 x 16)
 - **adventurer_tilesheet**: Cell Count (9 x 3)
 - **spritesheet_jumper**: Automatic
 - **spritesheet_tiles**: Cell Count (8 x 16)

<hr></details><br>
<details><summary>What did that do?</summary>

Slicing is the process of defining each individual sprite in a sprite sheet.  Once sliced, you can access each sprite as if it were a unique asset.

After you have sliced, white lines appear in the 'Sprite Editor'.  These lines show you how the sprite sheet is cut, boxing in each individual sprite.  Any whitespace as shown in this example is ignored (i.e., it does not generate blank sprites as well).

<img src="https://i.imgur.com/NawupLS.png" width=50% />

After closing the 'Sprite Editor' and applying changes you can expand the sprite sheet in Assets to see each sprite it created.

<img src="https://i.imgur.com/Qq0nn2B.png" width=50% />

<hr></details>
<details><summary>Why not always use Automatic?</summary>

Automatic does not always provide the desired results.  

One issue may be consistency between sprites in a sprite sheet.  Often we want each sprite to be treated the same. For example with the character, using automatic will create different sized sprites for each pose.  This can make animating a challenge.

<img src="https://i.imgur.com/lKfaiMj.png" width=300px />

Other issues may arise as well, such as different objects in a sprite sheet being combined into a single sprite.

Use the white lines in the 'Sprite Editor' to confirm the results.  There is also an option to manually slice if you need more control.

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

<hr></details>




<br>**Update Spawner**:

 - Update Code/Controllers/**Spawner**:

<details><summary>Existing code</summary>

```csharp
using System.Collections;
using UnityEngine;
```

<hr></details>

```csharp
public class Spawner : PlayerDeathMonoBehaviour
```

<details><summary>Existing code</summary>

```csharp
{
  [SerializeField]
  GameObject thingToSpawn;
  
  [SerializeField]
  float minTimeBetweenSpawns = .5f;

  [SerializeField]
  float maxTimeBetweenSpawns = 10;

  [SerializeField]
  ContactFilter2D contactFilter;

  Collider2D safeZoneCollider;

  static Collider2D[] tempColliderList = new Collider2D[1];

  protected void Awake()
  {
    safeZoneCollider = GetComponent<Collider2D>();
  }

  protected void Start()
  {
    StartCoroutine(SpawnEnemiesCoroutine());
  }
```

</details>

```csharp
  public override void OnPlayerDeath()
  {
    StopAllCoroutines();
    StartCoroutine(SpawnEnemiesCoroutine());
  }
```

<details><summary>Existing code</summary>

```csharp
  IEnumerator SpawnEnemiesCoroutine()
  {
    while(true)
    {
      if(safeZoneCollider == null
        || safeZoneCollider.OverlapCollider(
          contactFilter, tempColliderList) == 0)
      {
        Instantiate(
        thingToSpawn,
        transform.position,
        Quaternion.identity);
      }

      float sleepTime = UnityEngine.Random.Range(
        minTimeBetweenSpawns,
        maxTimeBetweenSpawns);
      yield return new WaitForSeconds(sleepTime);
    }
  }
}
```
</details>





## 1.4) Disable Anti Aliasing

[YouTube]() | [Source before](https://github.com/hardlydifficult/2DUnityTutorial/archive/cb9527525820b72f3a8dbff786153b92a6c2ebc4.zip) | [Source after](https://github.com/hardlydifficult/2DUnityTutorial/archive/0c2993c651a60b56e583c80d1006f232c93539b3.zip)

Disable anti aliasing for each quality level.

<details><summary>How</summary>

**Disable anti aliasing**:

 - Open menu Edit -> Project Settings -> Quality.
 - In the Inspector:
   - Anti Aliasing: Disabled

<img src="https://i.imgur.com/auHPjbi.png" width=300px />

<br>**Repeat for each quality 'Level'**:

   - Click on the row to modify (e.g., 'Very High').
   - Update Anti Aliasing if needed.

<img src="https://i.imgur.com/KYym6V0.png" width=300px />

 - Click 'Ultra' to resume testing with the best settings.

<hr></details><br>
<details><summary>What's anti aliasing?</summary>

Anti aliasing is a technique used to smooth jagged edges as shown here:

<img src="https://qph.ec.quoracdn.net/main-qimg-10856ecbea4f439fb9fb751d41ff704a" width=150px />

Disabling anti aliasing gets us closer to pixel-perfect sprites and prevents some visual glitches, particularly when using sprite sheets. Like changing the filter mode to Point, we do this when working with sprites because we often want control over images down to the pixel.

<hr></details>
<details><summary>Why do we need to change this setting multiple times?</summary>

The highlighted 'Level' is what you are testing with at the moment.  It will default to Ultra.  The green checkboxes represent the default quality level for different build types.  To avoid artifacts, we disable anti aliasing in every level and then switch back to Ultra so that we are testing with the best settings.

<hr></details>
<details><summary>Why not update the camera instead?</summary>

The camera in your scene has an option to disable 'Allow MSAA'.  Disabling this will turn off Anti Aliasing, as we did above.  Since Anti Aliasing is disabled in the project settings, this checkbox has no effect.

You could opt to disable Anti Aliasing in the camera and not in the project settings; however, if you do, be sure that the cameras you use in other scenes have the same settings.

<hr></details>