﻿using System;
using System.Text;

namespace JwtViewer.Core
{
    public static class Base64
    {
        public static string Encode(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] ToBytes(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        public static string DecodeToString(string base64)
        {
            var bytes = ToBytes(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string UrlEncode(string value)
        {
            return new StringBuilder(Encode(value))
                .Replace("=", string.Empty)
                .Replace('+', '-')
                .Replace('/', '_')
                .ToString();
        }

        public static byte[] UrlDecode(string base64Url)
        {
            var padLength = (4 - base64Url.Length % 4) % 4;
            var value = new StringBuilder(base64Url)
                .Replace('-', '+')
                .Replace('_', '/')
                .Append('=', padLength);
            return ToBytes(value.ToString());
        }

        public static string UrlDecodeToString(string base64Url)
        {
            var padLength = (4 - base64Url.Length % 4) % 4;
            var value = new StringBuilder(base64Url)
                .Replace('-', '+')
                .Replace('_', '/')
                .Append('=', padLength);
            return DecodeToString(value.ToString());
        }
    }
}