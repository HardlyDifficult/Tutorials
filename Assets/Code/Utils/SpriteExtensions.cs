using UnityEngine;

/// <summary>
/// Provides additional convenience methods for Unity's SpriteRenderer.
/// </summary>
public static class SpriteExtensions
{
  /// <summary>
  /// Sets the color for each of the sprites provided.
  /// </summary>
  /// <param name="spriteList">The list of sprites to color.</param>
  /// <param name="color">The color to set.</param>
  public static void SetColor(
    this SpriteRenderer[] spriteList,
    Color color)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      sprite.color = color;
    }
  }

  /// <summary>
  /// Sets the alpha, preserving any color the sprite may have, for each of the sprites provided.
  /// </summary>
  /// <param name="spriteList">The list of sprites to color.</param>
  /// <param name="alpha">The alpha to set.</param>
  public static void SetAlpha(
    this SpriteRenderer[] spriteList, 
    float alpha)
  {
    for(int i = 0; i < spriteList.Length; i++)
    {
      SpriteRenderer sprite = spriteList[i];
      Color originalColor = sprite.color;
      sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }
  }
}
