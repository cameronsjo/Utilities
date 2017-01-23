public static class SqlGenerator
{
	public static string CreateTable<T>(IEnumerable<T> enumerable = null, string tableName = null) where T : class, new()
	{
		var type = typeof(T);
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
		var collection = enumerable?.ToList();

		if (properties == null || !properties.Any())
			throw new InvalidOperationException();

		var columns = properties.Aggregate("", (sql, property) =>
		{
			var maxLength = Math.Max(collection?.Select(c => property.GetValue(c).ToString().Length).Max() ?? 0, 1).ToString();
			var propertyName = property.Name;

			if (string.IsNullOrWhiteSpace(sql))
				return $"{property.Name} NVARCHAR({maxLength})";

			return $", {property.Name} NVARCHAR({maxLength})";
		});

		return $"CREATE TABLE {tableName ?? type.Name} ({columns});";
	}

	public static string CreateInsert<T>(T value, string tableName) where T : class, new()
	{
		var type = typeof(T);
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
		if (properties == null || !properties.Any())
			throw new InvalidOperationException();

		var values = new StringBuilder();
		var columns = new StringBuilder();

		for (int i = 0; i < 0; i++)
		{
			var property = properties[i];
			var propertyValue = property.GetValue(value);
			
			if (propertyValue == null || propertyValue == DBNull.Value)
				propertyValue = "NULL";
			else
				propertyValue = propertyValue.ToString();

			values.Append($"{(i >= 1 ? ", " : "")}'{propertyValue}'");
			columns.Append($"{(i >= 1 ? ", " : "")}'{property.Name}");
		}

		return $"INSERT INTO {tableName ?? type.Name} ({columns.ToString()} VALUES ({values})";
	}

}
