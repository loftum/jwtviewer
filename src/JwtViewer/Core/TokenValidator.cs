using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class TokenValidator
    {
        private static readonly Dictionary<string, string> HashAlgorithmMap = new Dictionary<string, string>
        {
            ["RS256"] = "SHA256"
        };

        private readonly string _issuer;
        private readonly IList<Jwk> _keys;
        private readonly JToken _openidConfig;
        private readonly JToken _jwks;

        public TokenValidator()
        {
            _keys = new List<Jwk>();
        }

        public TokenValidator(JToken config)
        {
            var openidConfig = config["openid-configuration"];
            
            var jwks = config["jwks"];

            var keys = (JArray) jwks["keys"];

            _keys = keys.Select(d => new Jwk(d)).ToList();
            _issuer = openidConfig["issuer"]?.ToString();
            _jwks = jwks;
            _openidConfig = openidConfig;
        }

        public void ValidateToken(Jwt jwt)
        {
            var parameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = jwt.Payload?["aud"]?.ToString(),
                IssuerSigningKeys = _keys.Select(k => new RsaSecurityKey(new RSAParameters{Exponent = Base64.UrlDecode(k.PemExponent), Modulus = Base64.UrlDecode(k.PemModulus)})
                    {
                        KeyId = k.KeyId,
                        
                    })
            };
            
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(jwt.Raw, parameters, out var validatedToken);
            
        }

        private static string GetHashAlgorithm(string alg)
        {
            return HashAlgorithmMap.TryGetValue(alg, out var val) ? val : alg;
        }
    }
}