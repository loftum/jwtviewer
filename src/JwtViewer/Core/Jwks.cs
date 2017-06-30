using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class Jwks
    {
        public JToken Config { get; }

        public string KeyType { get; }
        public string Use { get; }
        public string KeyId { get; }
        public string CertificateThumbprint { get; }
        public List<string> CertificateChain { get; }
         
        public List<X509Certificate2> Certificates { get; }

        public Jwks(JToken config)
        {
            Config = config;
            KeyType = config["kty"]?.ToString();
            KeyId = config["kid"]?.ToString();
            Use = config["use"]?.ToString();
            CertificateThumbprint = config["x5t"]?.ToString();
            CertificateChain = (config["x5c"] as JArray ?? new JArray()).Select(t => t.ToString()).ToList();
            Certificates = CertificateChain.Select(c => new X509Certificate2(Convert.FromBase64String(c))).ToList();
            var key = Certificates.First().PublicKey;
            
        }


        public bool CanSign => Use == "sig";

        public string Sign(string header, string payload)
        {
            return null;
        }
    }
}