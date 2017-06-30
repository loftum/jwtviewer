using System;
using JwtViewer.Core;
using Newtonsoft.Json.Linq;

namespace JwtViewer.ViewModels
{
    public class JwtViewModel : PropertyChangedNotifier
    {
        private string _header;
        private string _payload;
        
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public string Payload
        {
            get { return _payload; }
            set
            {
                _payload = value;
                OnPropertyChanged();
            }
        }

        public void Parse(string raw)
        {
            Header = CalculateHeader(raw);
            Payload = CalculatePayload(raw);
        }

        private static string CalculateHeader(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            try
            {
                var base64 = value.Split('.')[0];
                return JObject.Parse(Base64.UrlDecode(base64)).ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static readonly string[] KnownTimeProperties =
        {
            "auth_time",
            "exp",
            "nbf"
        };

        private static string CalculatePayload(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            try
            {
                var base64 = value.Split('.')[1];
                var payload = JObject.Parse(Base64.UrlDecode(base64));

                var help = new JObject();
                foreach (var knownTimeProperty in KnownTimeProperties)
                {
                    JToken val;
                    long seconds;
                    if (payload.TryGetValue(knownTimeProperty, out val) && long.TryParse(val.Value<string>(), out seconds))
                    {
                        help[knownTimeProperty] = Utc.Epoch.AddSeconds(seconds);
                    }
                }
                payload["friendly"] = help;

                return payload.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}