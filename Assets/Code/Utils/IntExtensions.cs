/// <summary>
/// Provides additional convenience methods for int.
/// </summary>
public static class IntExtensions
{
  /// <summary>
  /// Returns the number as a string with commas (or periods depending on culture).
  /// </summary>
  /// <param name="number">The number to format.</param>
  /// <returns>The number as a string.</returns>
  /// <remarks>I use this because I have a hard time remembering the format codes c# uses.</remarks>
  public static string ToStringWithCommas(
    this int number)
  {
    return number.ToString("N0");
  }
}
