using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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

        public static async Task<JObject> LoadConfigurationAsync(string authority)
        {
            return await FromFile(authority) ?? await RefreshConfigurationAsync(authority);
        }

        public static async Task<JObject> RefreshConfigurationAsync(string authority)
        {
            var config = await FromInternet(authority);
            Store(config, authority);
            return config;
        }

        private static void Store(object config, string authority)
        {
            var part = authority.Replace("https://", "").Replace("http://", "");
            var path = Path.Combine(FilePath, part, "jwks.json");
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, config.ToString());
        }

        private static async Task<JObject> FromInternet(string authority)
        {
            var openIdConfig = await Request($"{authority}/.well-known/openid-configuration");
            var jwks = await Request(openIdConfig["jwks_uri"].ToString());
            var config = new JObject
            {
                ["openid-configuration"] = openIdConfig,
                ["jwks"] = jwks
            };
            return config;
        }

        private static async Task<JObject> Request(string url)
        {
            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";
                request.Accept = "application/json";
                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            throw new ApplicationException("Stream is null. Wat");
                        }
                        using (var reader = new StreamReader(stream))
                        {
                            var content = await reader.ReadToEndAsync();
                            return JObject.Parse(content);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Could not reach {url}", e);
            }
        }

        private static async Task<JObject> FromFile(string authority)
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
                    var content = await reader.ReadToEndAsync();
                    return JObject.Parse(content);
                }
            }
        }
    }
}