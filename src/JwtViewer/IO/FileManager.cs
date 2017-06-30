using System;
using System.IO;
using Newtonsoft.Json;

namespace JwtViewer.IO
{
    public class FileManager
    {
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

        static FileManager()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }

        public void SaveJson(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var path = GetFullPath($"{item.GetType().Name}.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(item));
        }

        public T LoadJson<T>()
        {
            var path = GetFullPath($"{typeof(T).Name}.json");
            return File.Exists(path) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path)) : default(T);
        }

        private static string GetFullPath(string filename)
        {
            return Path.Combine(BasePath, filename);
        }
    }
}