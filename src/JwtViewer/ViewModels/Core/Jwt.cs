namespace JwtViewer.ViewModels.Core;

public class Jwt
{
    public string Raw { get; private set; }
    public JwtObject Header { get; private init; }
    public JwtObject Payload { get; private init; }
    public string Signature { get; private init; }

    public int Position => 0;

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

        JwtObject.TryParse(parts[0], 0, out var header);
        JwtObject.TryParse(parts[1], parts[0].Length + 1, out var payload);
        
        jwt = new Jwt
        {
            Raw = raw,
            Header = header,
            Payload = payload,
            Signature = parts[2]
        };
        return true;
    }

    public override string ToString()
    {
        return Raw;
    }
}
