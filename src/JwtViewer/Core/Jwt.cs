using System;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class Jwt
    {
        public string Raw { get; }
        public string RawHeader { get; }
        public string RawPayload { get; }
        public string Signature { get; }

        public JObject Header { get; }
        public JObject Payload { get; }

        public Jwt(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }
            Raw = raw;
            var parts = raw.Split('.');
            if (parts.Length < 1)
            {
                return;
            }
            RawHeader = parts[0];
            Header = JObject.Parse(Base64.UrlDecodeToString(parts[0]));
            if (parts.Length < 2)
            {
                return;
            }
            RawPayload = parts[1];
            Payload = JObject.Parse(Base64.UrlDecodeToString(parts[1]));
            if (parts.Length < 3)
            {
                return;
            }
            Signature = parts[2];
        }
    }
}