using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class IntExtensions
{
  public static string ToStringWithCommas(
    this int number)
  {
    return number.ToString("N0");
  }
}
