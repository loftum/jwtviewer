using JwtViewer.Core;
using Newtonsoft.Json.Linq;

namespace JwtViewer.ViewModels
{
    public class JwtViewModel : PropertyChangedNotifier
    {
        private static readonly string[] KnownTimeProperties =
        {
            "auth_time",
            "exp",
            "nbf"
        };

        private string _raw;
        private string _header;
        private string _payload;
        private string _signature;

        public string Raw
        {
            get => _raw;
            set
            {
                _raw = value;
                OnPropertyChanged();
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public string Payload
        {
            get => _payload;
            set
            {
                _payload = value;
                OnPropertyChanged();
            }
        }

        public string Signature
        {
            get => _signature;
            set
            {
                _signature = value;
                OnPropertyChanged();
            }
        }

        public void Show(Jwt jwt)
        {
            Raw = jwt.Raw;
            Header = jwt.Header?.ToString();
            Payload = CalculatePayload(jwt);
            Signature = jwt.Signature;
        }

        private static string CalculatePayload(Jwt jwt)
        {
            if (jwt.Payload == null)
            {
                return null;
            }
            var payload = (JObject) jwt.Payload.DeepClone();
            var help = new JObject();
            foreach (var knownTimeProperty in KnownTimeProperties)
            {
                if (payload.TryGetValue(knownTimeProperty, out var val) && long.TryParse(val.Value<string>(), out var seconds))
                {
                    help[knownTimeProperty] = Utc.Epoch.AddSeconds(seconds);
                }
            }
            payload["friendly"] = help;
            return payload.ToString();
        }
    }
}