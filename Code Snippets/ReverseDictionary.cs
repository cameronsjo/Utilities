    public static Dictionary<TKey, TValue> ToReverseDictionary<TModel, TKey, TValue>(this IEnumerable<TModel> collection,
      Func<TModel, IEnumerable<TKey>> keySelector, 
      Func<TModel, TValue> valueSelector)
    {
      var dictionary = new Dictionary<TKey,TValue>();

      foreach (var item in collection)
      {
        var value = valueSelector.Invoke(item);

        foreach (var key in keySelector.Invoke(item))
          dictionary.Add(key, value);
      }

      return dictionary;
    }
