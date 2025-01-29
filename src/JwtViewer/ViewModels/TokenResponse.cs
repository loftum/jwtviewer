using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JwtViewer.ViewModels;

public class TokenResponse : Dictionary<string, object>
{
    [JsonIgnore]
    public string AccessToken
    {
        get => GetString("access_token");
        set => this["access_token"] = value;
    }

    [JsonIgnore]
    public string IdToken
    {
        get => GetString("id_token");
        set => this["id_token"] = value;
    }
    
    public string GetRefreshToken() => GetString("refresh_token");

    private string GetString(string key)
    {
        if (TryGetValue(key, out var v))
        {
            return v switch
            {
                JsonElement e => e.GetString(),
                JsonNode e => e.GetValue<string>(),
                string s => s,
                _ => null
            };
        }

        return null;
    }

    private static readonly JsonSerializerOptions PermissiveRead = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
    
    public static bool TryParse(string value, [MaybeNullWhen(false)] out TokenResponse tokenResponse)
    {
        try
        {
            tokenResponse = JsonSerializer.Deserialize<TokenResponse>(value, PermissiveRead);
            return true;
        }
        catch (Exception)
        {
            tokenResponse = null;
            return false;
        }
    }

    private static readonly JsonSerializerOptions Pretty = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public string ToPrettyJson()
    {
        return JsonSerializer.Serialize(this, Pretty);
    }
}