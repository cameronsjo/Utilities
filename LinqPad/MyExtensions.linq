<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

void Main()
{
	// Write code to test your extensions here. Press F5 to compile and run.
}

public static class FileIO
{
  private static string LastDirectoryKey  = "extensions.fileio.lastdirectory";
 
    public static IEnumerable<string> GetFilesRecursivelyFromDirectory(string directory)
    {
      return Directory.EnumerateFiles(directory).Concat(
             Directory.EnumerateDirectories(directory)
                      .SelectMany(subdir => GetFilesRecursivelyFromDirectory(subdir)));
    }
    
    // This doesn't work, and I haven't taken the time to figure out why...
    public static IEnumerable<string> GetDirectoriesRecursively(string directory)
    {
     return Directory.EnumerateDirectories(directory).SelectMany (subDir => GetDirectoriesRecursively(subDir));
    }
	
	public static IEnumerable<string> GetFolders()
	{
		var initialDirectory = Util.GetPassword(LastDirectoryKey);
    
    	if (string.IsNullOrWhiteSpace(initialDirectory))
    		initialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
    
        var ofd = new System.Windows.Forms.OpenFileDialog
        {
            InitialDirectory = initialDirectory,
			Multiselect = true,			
        };
		
		var result = ofd.ShowDialog();
    
        if (result != System.Windows.Forms.DialogResult.OK)
	    {
        	throw new Exception("Open file was cancelled.");
    	}
		
		return ofd.FileNames;
	
	}
 
    public static string GetFile()
    {
    var initialDirectory = Util.GetPassword(LastDirectoryKey);
    
    if (string.IsNullOrWhiteSpace(initialDirectory))
    initialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
    
        var ofd = new System.Windows.Forms.OpenFileDialog()
        {
            CheckFileExists = true,
            InitialDirectory = initialDirectory
        };
	
        var result = ofd.ShowDialog();
    
        if (result != System.Windows.Forms.DialogResult.OK)
	      {
            throw new Exception("Open file was cancelled.");
    	  }
    
        var fileName = ofd.FileName;
        
        var directory = System.IO.Directory.GetParent(fileName);
        
        Util.SetPassword(LastDirectoryKey, directory.FullName);
    
        if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
        {
            throw new Exception("Unable to get file: " + fileName);
        }
        
        Trace.WriteLine("File found: " + fileName);
    
        return fileName;
    }
    
    public static IEnumerable<string> GetFiles(bool log = false)
    {
        var ofd = new System.Windows.Forms.OpenFileDialog()
        {
            CheckFileExists = true,
            Multiselect = true,
            InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)
        };
	
        var result = ofd.ShowDialog();
    
        if (result != System.Windows.Forms.DialogResult.OK)
	      {
            throw new Exception("Open file was cancelled.");
    	  }
    
        var fileNames = ofd.FileNames;
        
        for (int i = 0; i < fileNames.Count(); i++ )
        {
          var fileName = fileNames[i];
         
          if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
          {
            throw new Exception("Unable to get file: " + fileName);
          }
          
        
          Trace.WriteLine("File found: " + fileName);
        }
    
        return fileNames;
    
    }
    
  public static async Task ProcessLinesAsync(string fileName, Action<string> action)
  {
    using (var s = new StreamReader(fileName))
    {
      string line;
    
      while((line = await s.ReadLineAsync()) != null)
      {
        action.Invoke(line);
      }
    }
  }
 
  public static async Task ProcessLinesAsync(string fileName, Func<string, Task> action)
  {
    using (var s = new StreamReader(fileName))
    {
      string line;
    
      while((line = await s.ReadLineAsync()) != null)
      {
        await action.Invoke(line);
      }
    }
  }
}

public static class StringIO
{
public static string GetText() 
{
            var window = new Form
            {
                AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F),
                AutoScaleMode = AutoScaleMode.Font,
                ClientSize = new System.Drawing.Size(1541, 898),
                Text = "Enter text"
            };

            var textBox1 = new TextBox
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(0, 0),
                Multiline = true,
                Name = "textBox1",
                Size = new System.Drawing.Size(1541, 898),
                TabIndex = 0
            };
			
            var button1 = new Button
            {
                Dock = DockStyle.Bottom,
                Location = new System.Drawing.Point(0, 861),
                Name = "button1",
                Size = new System.Drawing.Size(1541, 37),
                TabIndex = 1,
                Text = "Process",
                UseVisualStyleBackColor = true
            };
			
			button1.Click += (o,e) => { window.Close(); };
            
            window.Controls.Add(button1);
            window.Controls.Add(textBox1);

            window.ShowDialog();

            return textBox1.Text;

}

}

public static class MyExtensions
{


  /*
   * I didn't write this, and I can't remember where I got it.
   * But boy is it useful when doing analytics.
   */
  
  public static IList<TR> FullOuterGroupJoin<TA, TB, TK, TR>(
    this IEnumerable<TA> a,
    IEnumerable<TB> b,
    Func<TA, TK> selectKeyA, 
    Func<TB, TK> selectKeyB,
    Func<IEnumerable<TA>, IEnumerable<TB>, TK, TR> projection,
    IEqualityComparer<TK> cmp = null)
  {
    cmp = cmp?? EqualityComparer<TK>.Default;
    var alookup = a.ToLookup(selectKeyA, cmp);
    var blookup = b.ToLookup(selectKeyB, cmp);
 
    var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
    keys.UnionWith(blookup.Select(p => p.Key));
 
    var join = from key in keys
               let xa = alookup[key]
               let xb = blookup[key]
               select projection(xa, xb, key);
 
    return join.ToList();
  }
 
  public static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
    this IEnumerable<TA> a,
    IEnumerable<TB> b,
    Func<TA, TK> selectKeyA, 
    Func<TB, TK> selectKeyB,
    Func<TA, TB, TK, TR> projection,
    TA defaultA = default(TA), 
    TB defaultB = default(TB),
    IEqualityComparer<TK> cmp = null)
  {
    cmp = cmp?? EqualityComparer<TK>.Default;
    var alookup = a.ToLookup(selectKeyA, cmp);
    var blookup = b.ToLookup(selectKeyB, cmp);
 
    var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
    keys.UnionWith(blookup.Select(p => p.Key));
 
    var join = from key in keys
               from xa in alookup[key].DefaultIfEmpty(defaultA)
               from xb in blookup[key].DefaultIfEmpty(defaultB)
               select projection(xa, xb, key);
 
    return join.ToList();
  }
	
}

// You can also define non-static classes, enums, etc.
