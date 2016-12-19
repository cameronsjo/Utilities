<Query Kind="Program" />

void Main()
{
	var watcher = new FileSystemWatcher();
	
  watcher.Path = @"C:\Temp\";
	watcher.Filter = "*.txt";
	watcher.Changed += (sender, args) => DoWork();
	watcher.EnableRaisingEvents = true;
  
  // Keep this thread and the LinqPad script to keep running.
	Thread.Sleep(-1);
}

public static object @lock = new object();

public void DoWork()
{
	// Clear up the old results.
  Util.ClearResults();
  
	// Sleep for 100 ms so that whoever has the file handle will fully release it
  // before we steal it! Otherwise you'll get an Exception. :(
	Thread.Sleep(100);
	
  // If you double save, then you have problems.
  // Thus, the lock :)
	lock (@lock)
	{
		var data = new List<string>();

		using (StreamReader reader = new StreamReader(@"C:\Temp\Example.txt"))
		{
			var line = reader.ReadLine();

			while (line != null)
			{
				data.Add(line);
				line = reader.ReadLine();
			}
		}

		var hasData = true;

		var skip = 0;

		"var parameters = new []".Dump();
		"{".Dump();


		while (hasData)
		{
			var lines = data.Skip(skip).Take(3).ToArray();

			if (!lines.Any() || lines.Length != 3)
			{
				hasData = false;
				break;
			}

			var r1 = @"\s*var\s+(?<ParameterName>\w+)\s+=\s+(?<Variable>\w+)(?:(?:\s!=\snull)|(?:\.HasValue))";
			var r2 = @"\s+\?\s+new\s+\w+\(\""(?<ParameterName>\w+)\""\,.*\)\s*";
			var r3 = @"\s+\:\s+new\s+\w+\(.*,\s+typeof\((?<Type>\w+)\)\)\;";

			var m1 = Regex.Match(lines[0], r1);
			var m2 = Regex.Match(lines[1], r2);
			var m3 = Regex.Match(lines[2], r3);

			Debug.Assert(m1.Success, $"No match on {nameof(r1)}");
			Debug.Assert(m2.Success, $"No match on {nameof(r2)}");
			Debug.Assert(m3.Success, $"No match on {nameof(r3)}");

			var variable = m1.Groups["Variable"].Value;
			var parameterName = m2.Groups["ParameterName"].Value;
			var type = m3.Groups["Type"].Value;

			Debug.Assert(!string.IsNullOrWhiteSpace(variable), $"Invalid value: {nameof(variable)}");
			Debug.Assert(!string.IsNullOrWhiteSpace(parameterName), $"Invalid value: {nameof(parameterName)}");
			Debug.Assert(!string.IsNullOrWhiteSpace(type), $"Invalid value: {nameof(type)}");

			var dbType = "";

			if (type == "string")
				dbType = "VarChar";
			if (type == "int")
				dbType = "Int";
			if (type == "short")
				dbType = "SmallInt";
			if (type == "DateTime")
				dbType = "DateTime";
			if (type == "bool")
				dbType = "Bit";
			if (type == "decimal")
				dbType = "Decimal";

			if (type != "string")
				$"    CreateParameter(\"@{parameterName}\", {variable}, SqlDbType.{dbType}),".Dump();
			else
				$"    CreateParameter(\"@{parameterName}\", {variable}),".Dump();
			skip += 4;
		}
		"};".Dump();
	}
}

// Define other methods and classes here
