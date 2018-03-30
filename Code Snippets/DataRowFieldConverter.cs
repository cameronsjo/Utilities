public static T Convert<T>(System.Data.DataRow row, string field)
{
  try
  {
    var value = row[field];
    if (value == System.DBNull || value == null)
    {
      return default(T);
    }
    
    var type = typeof(T);
    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
    {
      type = type.GetGenericArguments()[0];
    }
    
    if (type.IsValueType)
    {
      var tryParse = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
      if (tryParse == null)
      {
        throw new InvalidOperationException($"Unable to find TryParse method on type. Type: {typeof(T).Name}");
      }
      
      var t = default(T);
      var parameters = new[] { (object) value.ToString(), t };
      var success = tryParse.Invoke(null, parameters);
      if ((bool) success)
      {
        return (T) parameters[1];
      }
    }
    else
    {
      return (T) Convert.ChangeType(value, type);
    }
  }
  catch(Exception e)
  {
    System.Diagnostics.Debug.WriteLine($"Failed to convert {field}. Value: {row[field]}, Source: {row.Table.Columns[field].DataType.Name}, Target: {typeof(T).Name}, Message: {e.Message}");
    throw;
  }
}
