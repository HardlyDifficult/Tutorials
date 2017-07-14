using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main FlyGuy enemy entity component.  Keeps count of total active fly guys in the world.
/// Drives random movement.
/// </summary>
public class FlyGuy : MonoBehaviour
{
  #region Data
  /// <summary>
  /// The total number of active fly guys in the world.
  /// </summary>
  public static int flyGuyCount;

  /// <summary>
  /// When selecting a new direction to walk, this is the odds of selecting up vs down (when on an incline).
  /// </summary>
  [SerializeField]
  float oddsOfGoingUpHill = .8f;

  /// <summary>
  /// How long to wait before picking a new random direction.
  /// </summary>
  [SerializeField]
  float timeBeforeFirstWander = 5;

  /// <summary>
  /// Used to determine if we are on the ground.
  /// </summary>
  Feet feet;

  /// <summary>
  /// Used to know when we are on a ladder.
  /// </summary>
  LadderMovement ladderMovement;

  /// <summary>
  /// Used to control the entity walk.
  /// </summary>
  WalkMovement walkMovement;

  /// <summary>
  /// If the starting direction should be random vs always walk to the right.
  /// </summary>
  [SerializeField]
  bool shouldRandomizeStartingMovement = false;
  #endregion

  #region Init
  /// <summary>
  /// On awake, populate variables and register for ladder events.
  /// Disable this component until fade completes (preventing movement).
  /// </summary>
  void Awake()
  {
    walkMovement = GetComponent<WalkMovement>();
    ladderMovement = GetComponent<LadderMovement>();
    feet = GetComponentInChildren<Feet>();

    ladderMovement.onGettingOffLadder += LadderMovement_onGettingOffLadder;

    AppearInSecondsAndFadeInSprite.DisableMeTillComplete(this);
  }

  /// <summary>
  /// On start, add to the total active flyGuy count and start random movement.
  /// </summary>
  void Start()
  {
    flyGuyCount++;

    StartCoroutine(Wander());
  }
  #endregion

  #region Events
  /// <summary>
  /// Anytime we get off a ladder, pick a new random direction.
  /// </summary>
  void LadderMovement_onGettingOffLadder()
  {
    SelectARandomWalkDirection();
  }
  #endregion

  #region Private helpers
  /// <summary>
  /// Executes the random movement script, periodically changing walk direction.
  /// </summary>
  /// <returns>Used by corountines to manage time.</returns>
  IEnumerator Wander()
  {
    if(shouldRandomizeStartingMovement == false)
    { // Walk to the right for the first few seconds
      walkMovement.inputWalkDirection = 1;
      yield return new WaitForSeconds(timeBeforeFirstWander);
    }

    while(true)
    { // Every so often, if not currently on a ladder, pick a new walk direction.
      if(ladderMovement.isOnLadder == false)
      {
        SelectARandomWalkDirection();
      }
      yield return new WaitForSeconds(UnityEngine.Random.Range(1, 10f));
    }
  }

  /// <summary>
  /// Sets a direction to walk, considering the edge of the screen and the angle of the platform we are on.
  /// </summary>
  void SelectARandomWalkDirection()
  {
    if(GameController.instance.screenBounds.min.x >= transform.position.x)
    { // Past left edge of screen, walk right
      walkMovement.inputWalkDirection = 1;
    }
    else if(GameController.instance.screenBounds.max.x <= transform.position.x)
    { // Past right edge of screen, walk left
      walkMovement.inputWalkDirection = -1;
    }
    else
    { // Not on edge of screen, check for inclines
      // Use dot to determine if the floor is has an incline facing the right (where 0 means a flat surface)
      float dot = Vector2.Dot(feet.floorUp, Vector2.right);
      if(dot < -.1f)
      { // Negative dot means that the slope is down and to the left
        // If odds say go up hill, travel right
        walkMovement.inputWalkDirection = UnityEngine.Random.value >= oddsOfGoingUpHill ? 1 : -1;
      }
      else if(dot > .1f)
      { // Positive dot means that the slot is down and to the right
        // If odds say go up hill, travel left
        walkMovement.inputWalkDirection = UnityEngine.Random.value >= oddsOfGoingUpHill ? -1 : 1;
      }
      else 
      { // On a flat surface, roll the dice to pick a direction
        walkMovement.inputWalkDirection = UnityEngine.Random.value >= .5f ? 1 : -1;
      }
    }
  }
  #endregion
}
