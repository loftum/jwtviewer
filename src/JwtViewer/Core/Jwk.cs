using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace JwtViewer.Core
{
    public class Jwk
    {
        public string KeyType { get; }
        public string Use { get; }
        public string KeyId { get; }
        public string PemExponent { get; }
        public string PemModulus { get; }
        public string CertificateThumbprint { get; }
        public List<string> CertificateChain { get; }

        public List<X509Certificate2> Certificates { get; }
        public bool CanSign => Use == "sig";

        public Jwk(JToken config)
        {
            KeyType = config["kty"]?.ToString();
            KeyId = config["kid"]?.ToString();
            Use = config["use"]?.ToString();
            PemExponent = config["e"]?.ToString();
            PemModulus = config["n"]?.ToString();
            CertificateThumbprint = config["x5t"]?.ToString();
            CertificateChain = (config["x5c"] as JArray ?? new JArray()).Select(t => t.ToString()).ToList();


            Certificates = CertificateChain.Select(c => new X509Certificate2(Convert.FromBase64String(c))).ToList();
            
        }

        public void VerifySignature(byte[] text, string alg, byte[] signature)
        {
            
            var csp = (RSACryptoServiceProvider)Certificates.First().PublicKey.Key;
            using (var algorithm = HashAlgorithm.Create(alg))
            {
                if (algorithm == null)
                {
                    throw new InvalidOperationException($"Invalid alg {alg}");
                }
                var hash = algorithm.ComputeHash(text);
                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(csp);
                rsaDeformatter.SetHashAlgorithm(alg);
                if (!rsaDeformatter.VerifySignature(hash, signature))
                {
                    throw new Exception("Invalid signature");
                }
            }
        }
    }
}