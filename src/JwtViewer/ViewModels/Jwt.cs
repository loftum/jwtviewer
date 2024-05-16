using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JwtViewer.Conversion;

namespace JwtViewer.ViewModels;

public class Jwt
{
    private static JsonSerializerOptions Pretty = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public Dictionary<string, object> Header { get; private init; }
    public Dictionary<string, object> Payload { get; private init; }
    public string Signature { get; private init; }
    public string DecodedJwt { get; private set; } 

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

        var header = Base64.UrlDecode(parts[0]);
        var payload = Base64.UrlDecode(parts[1]);

        jwt = new Jwt
        {
            Header = JsonSerializer.Deserialize<Dictionary<string, object>>(header),
            Payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payload),
            Signature = parts[2],
             
        };
        jwt.DecodedJwt = new StringBuilder()
            .AppendLine(JsonSerializer.Serialize(jwt.Header, Pretty))
            .AppendLine(".")
            .AppendLine(JsonSerializer.Serialize(jwt.Payload, Pretty))
            .AppendLine(".")
            .AppendLine(jwt.Signature)
            .ToString();
        return true;
    }
}