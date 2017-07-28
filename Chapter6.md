# 6) UI and Scene transitions

TODO intro

## Add a win condition

<details><summary>How</summary>

 - Create an empty GameObject named "WinArea".
 - Add a BoxCollider2D sized to cover the area that when entered will end the level.
 - Check Is Trigger.
 - Add a sprite to lure the character to the win area.  Make it a child of the WinArea.  We are using **spritesheet_jumper_26** with Order in Layer -3.

<img src="http://i.imgur.com/WuW9hPk.png" width=300px />

 - Create script Code/Components/Effects/**TouchMeToWin**:

```csharp
using UnityEngine;

public class TouchMeToWin : MonoBehaviour
{
  static int totalNumberActive;

  [SerializeField]
  LayerMask layerToTriggerOn;

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
    CheckForWin(collision.gameObject);
  }

  protected void OnCollisionEnter2D(
    Collision2D collision)
  {
    CheckForWin(collision.gameObject);
  }

  void CheckForWin(
    GameObject gameObject)
  {
    if(enabled == false)
    {
      return;
    }

    if(layerToTriggerOn.Includes(gameObject.layer))
    {
      enabled = false;
      if(totalNumberActive == 0)
      {
        GameObject.FindObjectOfType<LevelManager>().YouWin();
      }
    }
  }
}
```

 - Add to the WinArea and configure for the Player layer.

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## Win animation

When the character reaches the win area, 

<details><summary>How</summary>

 - Create another animation for the evil cloud, Animations/**CloudLevel1Exit** to play when the player wins.
   - You may not be able to record if the Timeline window is open.
 - Select CloudLevel1Exit and disable Loop Time.
 - Right click in Assets/Animations -> Create -> Timeline named "Level2Exit".
 - Select the evil cloud's sprite GameObject and in the Inspector change the Playable Director's 'Playable' to Level2Exit.

<img src="http://i.imgur.com/Jsah6Ll.png" width=150px />

 - In the Timeline window, click 'Add' then 'Animation Track' and select youself.
 - Right click in the timeline and 'Add Animation From Clip'.

<img src="http://i.imgur.com/xcR7HWr.gif" width=300px />

TODO

<hr></details><br>
<details><summary>TODO</summary>

Character stops moving, everyone else dies

<hr></details>

## Scene transition

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## UI for points

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## UI for lives

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## Main menu

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## Settings panel

<details><summary>How</summary>

TODO
Volume slider and keyboard remapping?

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


## Volume

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>

## Scene between level

<details><summary>How</summary>

TODO

<hr></details><br>
<details><summary>TODO</summary>

TODO

<hr></details>


# Next chapter

[Chapter 7](https://github.com/hardlydifficult/Platformer/blob/master/Chapter7.md).
