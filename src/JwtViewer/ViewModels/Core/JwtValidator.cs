using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace JwtViewer.ViewModels.Core;

public class JwtValidator
{
    public static JwtValidator Instance { get; } = new();

    public TokenValidationParameters Parameters { get; } = new()
    {
        ValidateAudience = false,
    };
    
    public bool TryValidate(string jwt, out Exception exception)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            _ = handler.ValidateToken(jwt, Parameters, out _);
            exception = default;
            return true;
        }
        catch (Exception e)
        {
            exception = e;
            return false;
        }
    }
}
