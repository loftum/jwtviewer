using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class TokenValidator
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        static TokenValidator()
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
        }

        private readonly IList<Jwks> _keys;

        public bool ValidateSignature(string jwt)
        {
            try
            {
                var parts = jwt?.Split('.');
                if (parts?.Length != 3)
                {
                    return false;
                }

                var header = parts[0];
                var payload = parts[1];
                var signature = parts[2];

                var key = _keys.FirstOrDefault(k => k.CanSign);
                if (key == null)
                {
                    return false;
                }
                var expectedSignature = key.Sign(header, payload);

                return expectedSignature == signature;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TokenValidator(IList<Jwks> keys)
        {
            _keys = keys;
        }

        public static TokenValidator For(string authority)
        {
            var config = FromFile(authority) ?? FetchAndStore(authority);
            var keys = ((JArray) config["keys"]).Select(c => new Jwks(c)).ToList();
            return new TokenValidator(keys);
        }

        private static JObject FetchAndStore(string authority)
        {
            var config = Fetch(authority);
            Store(config, authority);
            return config;
        }

        private static void Store(JObject config, string authority)
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
            var request = WebRequest.CreateHttp($"https://{authority}/core/.well-known/jwks");
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
                        var config = JObject.Parse(content);
                        return config;
                    }
                }
            }
        }

        private static JObject FromFile(string authority)
        {
            var path = Path.Combine(FilePath, authority, "jwks.json");
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