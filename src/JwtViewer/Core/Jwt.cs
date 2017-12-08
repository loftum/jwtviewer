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
        public Exception HeaderError { get; }
        public JObject Payload { get; }
        public Exception PayloadError { get; }

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
            try
            {
                Header = ParseOrFail(parts[0]);
            }
            catch (Exception e)
            {
                HeaderError = e;
            }
            
            if (parts.Length < 2)
            {
                return;
            }
            RawPayload = parts[1];
            try
            {
                Payload = ParseOrFail(parts[1]);
            }
            catch (Exception e)
            {
                PayloadError = e;
            }
            if (parts.Length < 3)
            {
                return;
            }
            Signature = parts[2];
        }

        private static JObject ParseOrFail(string part)
        {
            return JObject.Parse(Base64.UrlDecodeToString(part));
        }
    }
}