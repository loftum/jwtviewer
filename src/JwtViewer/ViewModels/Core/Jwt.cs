using JwtViewer.Conversion;
using ReactiveUI;

namespace JwtViewer.ViewModels.Core;

public delegate void PropertyValueChanged<in T>(object sender, T oldValue, T newValue);

public class Jwt : ReactiveObject
{
    public event PropertyValueChanged<string> RawChanged;
    
    private JwtObject _header;
    private JwtObject _payload;
    private string _signature;
    private string _raw;
    private string _title;

    public string Raw
    {
        get => _raw;
        private set
        {
            if (_raw == value)
            {
                return;
            }

            var old = _raw;
            _raw = value;
            RawChanged?.Invoke(this, old, value);
        }
    }

    public JwtObject Header
    {
        get => _header;
        set
        {
            this.RaiseAndSetIfChanged(ref _header, value);
            Raw = CalculateRaw();
        }
    }

    private string CalculateRaw()
    {
        var parts = new List<string>();
        if (_header != null)
        {
            parts.Add(Base64.UrlEncode(_header.ToJson()));
        }

        if (_payload != null)
        {
            parts.Add(Base64.UrlEncode(_payload.ToJson()));
        }

        if (_signature != null)
        {
            parts.Add(_signature);
        }

        return string.Join('.', parts);
    }

    public JwtObject Payload
    {
        get => _payload;
        set
        {
            this.RaiseAndSetIfChanged(ref _payload, value);
            Raw = CalculateRaw();
        }
    }

    public string Signature
    {
        get => _signature;
        set
        {
            this.RaiseAndSetIfChanged(ref _signature, value);
            Raw = CalculateRaw();
        }
    }

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public static bool TryParse(string raw, out Jwt jwt)
    {
        jwt = default;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        
        var parts = raw.Split('.', 3);
        if (parts.Length < 3)
        {
            return false;
        }

        JwtObject.TryParse(parts[0], out var header);
        JwtObject.TryParse(parts[1], out var payload);
        
        jwt = new Jwt
        {
            _raw = raw,
            _header = header,
            _payload = payload,
            _signature = parts[2]
        };
        return true;
    }

    public override string ToString()
    {
        return Raw;
    }
}
