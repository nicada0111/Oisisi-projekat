namespace SajamKnjiga.Core.Utils;

public static class DataPaths
{
    private const string SolutionFileName = "SajamKnjiga.sln";
    private const string DataFolderName = "Data";

    private static string? _dataFolder;

    public static string DataFolder
    {
        get
        {
            if (_dataFolder == null)
            {
                _dataFolder = PronadjiDataFolder();
                Directory.CreateDirectory(_dataFolder);
            }
            return _dataFolder;
        }
    }

    public static string GetFilePath(string fileName)
    {
        return DataFolder + Path.DirectorySeparatorChar + fileName;
    }

    private static string PronadjiDataFolder()
    {
        DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(dir.FullName + Path.DirectorySeparatorChar + SolutionFileName))
            {
                return dir.FullName + Path.DirectorySeparatorChar + DataFolderName;
            }
            dir = dir.Parent;
        }

        return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)
               + Path.DirectorySeparatorChar + DataFolderName;
    }
}
