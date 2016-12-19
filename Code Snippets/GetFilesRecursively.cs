    public static IEnumerable<string> GetFilesRecursivelyFromDirectory(string directory)
    {
      return Directory
        .EnumerateFiles(directory)
        .Concat(Directory.EnumerateDirectories(directory)
          .SelectMany(subdir => GetFilesRecursivelyFromDirectory(subdir)));
    }
