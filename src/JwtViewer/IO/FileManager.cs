using System.Text.Json;

namespace JwtViewer.IO;

internal class FileManager
{
    private static readonly string ParentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".convenient");
    
    private readonly string _basePath;

    public FileManager(string applicationName)
    {
        var basePath = Path.Combine(ParentPath, applicationName);
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
        _basePath = basePath;
    }

    public void SaveText(string text, string filename)
    {
        File.WriteAllText(GetPathFor(filename), text);
    }

    public string GetTextOrDefault(string filename)
    {
        var path = GetPathFor(filename);
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public void SaveJson(object item, string filename = null)
    {
        if (item == null)
        {
            return;
        }
        var path = filename == null ? GetPathFor(item.GetType()) : GetPathFor(filename);
        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(item));
    }

    public T LoadJson<T>(string filename = null)
    {
        var path = filename == null ? GetPathFor<T>() : GetPathFor(filename);
        return File.Exists(path)
            ? JsonSerializer.Deserialize<T>(File.ReadAllText(path))
            : default(T);
    }

    public T LoadJsonOrDefault<T>(T defaultValue = default)
    {
        try
        {
            return LoadJson<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    private string GetPathFor<T>()
    {
        return GetPathFor(typeof(T));
    }

    private string GetPathFor(Type type)
    {
        var filename = $"{type.Name}.json";
        return GetPathFor(filename);
    }

    private string GetPathFor(string filename)
    {
        var path = Path.Combine(_basePath, filename);
        return path;
    }
}