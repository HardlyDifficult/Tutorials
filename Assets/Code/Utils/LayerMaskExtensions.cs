using UnityEngine;

/// <summary>
/// Provides additional convenience methods for Unity's LayerMask.
/// </summary>
public static class LayerMaskExtensions
{
  /// <summary>
  /// Determines if the layer is part of this layerMask.
  /// </summary>
  /// <param name="mask">The layer mask defining which layers should be included.</param>
  /// <param name="layer">The layer to check against the mask.</param>
  /// <returns>True if the layer is part of the layerMask.</returns>
  /// <remarks>This method is used to wrap the bit logic below as it's not an intuitive read.</remarks>
  public static bool Includes(
    this LayerMask mask,
    int layer)
  {
    return (mask.value & 1 << layer) > 0;
  }
}
