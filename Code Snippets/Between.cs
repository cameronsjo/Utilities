/// <summary>
    ///   Compares an object against two other objects (upper and lower bound objects) to see if the value is between them.
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <param name="obj">Object for value to be compared against.</param>
    /// <param name="lower">Lower bound of the comparison</param>
    /// <param name="upper">Upper bound of the comparison</param>
    /// <param name="options">Options for performing the between</param>
    /// <returns>Returns true if the value is between the two bounds.</returns>
    public static bool Between<T>(this T obj, T lower, T upper, BetweenOptions options = BetweenOptions.Default) where T : IComparable<T>
    {
      switch (options)
      {
        case BetweenOptions.Default:
          return obj.CompareTo(lower) >= 0 && obj.CompareTo(upper) <= 0;

        case BetweenOptions.ExcludeLower:
          return obj.CompareTo(lower) > 0 && obj.CompareTo(upper) <= 0;

        case BetweenOptions.ExcludeUpper:
          return obj.CompareTo(lower) >= 0 && obj.CompareTo(upper) < 0;

        case BetweenOptions.ExcludeUpperAndLower:
          return obj.CompareTo(lower) > 0 && obj.CompareTo(upper) < 0;

        default:
          throw new Exception("Unable to process between. Invalid BetweenOptions value.");
      }
      
  public enum BetweenOptions
  {
    /// <summary>
    ///   Includes both upper and lower bounds.
    /// </summary>
    Default,

    /// <summary>
    ///   Exclude the upper bound
    /// </summary>
    ExcludeUpper,

    /// <summary>
    ///   Excludes the lower bound
    /// </summary>
    ExcludeLower,

    /// <summary>
    ///   Excludes both upper and lower bound.
    /// </summary>
    ExcludeUpperAndLower
  }
