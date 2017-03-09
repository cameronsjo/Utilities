<Query Kind="Program">
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\3txss4b0.sod\EnvDTE.dll</Reference>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>EnvDTE</Namespace>
  <Namespace>System.Runtime.InteropServices.ComTypes</Namespace>
</Query>


// This will be the actual version of Visual Studio that you're trying to hook into.
// 2010 => 10
// 2012 => 11
// 2013 => 12
// 2015 => 13
// 2017 => 15
const string VisualStudioVersion = "15";

void Main()
{
    const string SourceDirectory = @"";

    var dte = GetDteFromMoniker();

    foreach (var file in GetFilesRecursivelyFromDirectory(SourceDirectory).Where(file => file.EndsWith("csproj") || file.EndsWith("vbproj")))
    {
        file.Dump();

        dte.Solution.AddFromFile(file);
        
        // Arbitrary time to see if loading the file causes any dialog boxes in Visual Studio
        // Also to give time to Visual Studio to load the project.
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
    }
}

public static DTE GetDteFromProcesses()
{
    /* 
     * References:
     * - http://stackoverflow.com/a/34782950/2607840
     * - https://blogs.msdn.microsoft.com/kirillosenkov/2011/08/10/how-to-get-dte-from-visual-studio-process-id/
     */
    
    var processes = System.Diagnostics.Process.GetProcesses()
       .Where(proc => proc.ProcessName == "devenv")
       .Select(proc => proc.Id).ToArray()
       ;
    if (processes.Length == 0)
    {
        throw new Exception("No devenv processes running.");
    }

    else if (processes.Length > 1)
    {

        throw new Exception("Too many devenv processes.");
    }

    return GetDTE(processes.First());
}
public static DTE GetDteFromMoniker()
{
    return (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.15.0");
}

public static void FindDllsWithDte()
{
    // Visual Studio 2017 actually installs to a different location
    // C:\Program Files (x86)\Microsoft Visual Studio\2017\{Version}\
    // I wasn't able to find an assembly with DTE in it for VS 2017, but
    // a random EnvDTE.dll  for VS 2015 worked just fine.
    // 
    // I found it here, so your mileage may vary.
    // C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\3txss4b0.sod
    var Path = $@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\";

    foreach (var file in GetFilesRecursivelyFromDirectory(Path)
            .Where(file => file.EndsWith(".dll"))
            .Where(file => file.Contains("DTE"))
            .Distinct()
            .OrderBy(file => file))
    {
        file.Replace(Path, @"..\").Dump();
    }
}

public static IEnumerable<string> GetFilesRecursivelyFromDirectory(string directory)
{
    return Directory.EnumerateFiles(directory)
        .Concat(Directory.EnumerateDirectories(directory)
        .SelectMany(subdir => GetFilesRecursivelyFromDirectory(subdir)));
}

[DllImport("ole32.dll")]
private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

public static DTE GetDTE(int processId)
{
    string progId = $"!VisualStudio.DTE.{VisualStudioVersion}.0:" + processId.ToString();
    object runningObject = null;

    IBindCtx bindCtx = null;
    IRunningObjectTable rot = null;
    IEnumMoniker enumMonikers = null;

    try
    {
        Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
        bindCtx.GetRunningObjectTable(out rot);
        rot.EnumRunning(out enumMonikers);

        IMoniker[] moniker = new IMoniker[1];
        IntPtr numberFetched = IntPtr.Zero;
        while (enumMonikers.Next(1, moniker, numberFetched) == 0)
        {
            IMoniker runningObjectMoniker = moniker[0];

            string name = null;

            try
            {
                if (runningObjectMoniker != null)
                {
                    runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Do nothing, there is something in the ROT that we do not have access to.
            }
            //name.Dump();
            if (!string.IsNullOrEmpty(name) && string.Equals(name, progId, StringComparison.Ordinal))
            {
                Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                break;
            }
        }
    }
    finally
    {
        if (enumMonikers != null)
        {
            Marshal.ReleaseComObject(enumMonikers);
        }

        if (rot != null)
        {
            Marshal.ReleaseComObject(rot);
        }

        if (bindCtx != null)
        {
            Marshal.ReleaseComObject(bindCtx);
        }
    }

    return (DTE)runningObject;
}

// Define other methods and classes here
