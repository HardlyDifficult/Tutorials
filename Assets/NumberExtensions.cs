using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class NumberExtensions
{
  public static string ToStringWithCommas(
    this int number)
  {
    return number.ToString("N0");
  }
}
