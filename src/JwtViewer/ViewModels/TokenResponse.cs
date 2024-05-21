using System.Diagnostics.CodeAnalysis;
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

    public static bool TryParse(string value, [MaybeNullWhen(false)] out TokenResponse tokenResponse)
    {
        try
        {
            tokenResponse = JsonSerializer.Deserialize<TokenResponse>(value);
            return true;
        }
        catch (Exception)
        {
            tokenResponse = default;
            return false;
        }
    }


    private static readonly JsonSerializerOptions Pretty = new()
    {
        WriteIndented = true
    };
    
    public string ToPrettyJson()
    {
        return JsonSerializer.Serialize(this, Pretty);
    }
}