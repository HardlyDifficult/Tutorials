using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Ladder : MonoBehaviour
{
  #region Data
  [SerializeField]
  bool isBrokenLadder;

  public Bounds bounds
  {
    get; private set;
  }
  #endregion

  #region Properties
  public bool isBroken
  {
    get
    {
      return isBrokenLadder;
    }
  }
  #endregion

  #region Init
  protected void Awake()
  {
    bounds = GetComponent<Collider2D>().bounds;
  }
  #endregion
}
