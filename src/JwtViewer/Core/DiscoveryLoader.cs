using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class DiscoveryLoader
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        static DiscoveryLoader()
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
        }

        public static JObject Load(string authority)
        {
            return FromFile(authority) ?? Refresh(authority);
        }

        public static JObject Refresh(string authority)
        {
            var config = Fetch(authority);
            Store(config, authority);
            return config;
        }

        private static void Store(object config, string authority)
        {
            var path = Path.Combine(FilePath, authority, "jwks.json");
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, config.ToString());
        }

        private static JObject Fetch(string authority)
        {
            var config = new JObject
            {
                ["openid-configuration"] = Request($"https://{authority}/core/.well-known/openid-configuration"),
                ["jwks"] = Request($"https://{authority}/core/.well-known/jwks")
            };
            return config;
        }

        private static JObject Request(string url)
        {
            var request = WebRequest.CreateHttp(url);
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        throw new ApplicationException("Stream is null. Wat");
                    }
                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();
                        return JObject.Parse(content);
                    }
                }
            }
        }

        private static JObject FromFile(string authority)
        {
            var path = Path.Combine(FilePath, authority, "discovery.json");
            if (!File.Exists(path))
            {
                return null;
            }
            using (var stream = File.OpenRead(path))
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    return JObject.Parse(content);
                }
            }
        }
    }
}