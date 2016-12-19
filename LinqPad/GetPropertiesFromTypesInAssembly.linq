<Query Kind="Program" />

void Main()
{
  var file = FileIO.GetFile();
  
  var assembly = Assembly.LoadFile(file);
  
  var types = new Dictionary<int, dynamic>();
      
  int i = 1;
  
  var fileNameRemove =  assembly.GetName().Name;
    
  foreach(var t in assembly.GetTypes().OrderBy (o => o.FullName))
  {
    string name = t.FullName;
    
    var match = Regex.Match(name, fileNameRemove);
    
    if (match.Success)
    {
      var startIndex = 0;
      var endIndex = match.Index + match.Length+1;
    
      name = name.Remove(startIndex, endIndex);
    }
  
    types.Add(i, new { Type = t, Name = name});
    i++;
  }
  
  foreach (var kvp in types)
  {
    Console.WriteLine(string.Format("{0,-3}. {1}", kvp.Key, kvp.Value.Name));
  }
  
  Console.WriteLine();
  
  var selection = Console.ReadLine();
  
  var intSel = int.Parse(selection);
  
  var q = types[intSel].Type as Type;
  
  foreach (var x in q.FindMembers(MemberTypes.Property, 
                                  BindingFlags.Public | BindingFlags.Instance, 
                                  null, 
                                  null)
                      .OrderBy (o => o.Name))
  {
    Console.WriteLine(x.Name);
  }
  
  // This is a cheap work around to release the lock on the file so that it can be rebuilt.
  // We could potentially create another appdomain, load the assembly, access it's types, then do whatever we need, then kill the appdomain?
     AppDomain.Unload(AppDomain.CurrentDomain);
     
  }

// Define other methods and classes here
