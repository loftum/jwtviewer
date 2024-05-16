using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JwtViewer.ViewModels;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }
    [JsonPropertyName("id_token")]
    public string IdToken { get; init; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    public static bool TryParse(string value, [MaybeNullWhen(false)] out TokenResponse tokenResponse)
    {
        try
        {
            tokenResponse = JsonSerializer.Deserialize<TokenResponse>(value);
            return true;
        }
        catch (Exception e)
        {
            tokenResponse = default;
            return false;
        }
    }
}