using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using JwtViewer.Conversion;

namespace JwtViewer.ViewModels.Core;

public class JwtObject : Dictionary<string, IJwtNode>, IJwtNode
{
    private static readonly JsonSerializerOptions PrettyUnsafe = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters =
        {
            new JwtNodeConverter()
        }
    };
    
    private static readonly JsonSerializerOptions NonPretty = new()
    {
        WriteIndented = false,
        Converters =
        {
            new JwtNodeConverter()
        }
    };

    public int JsonStartPosition { get; set; }
    public int JsonEndPosition { get; set; }

    public static bool TryParse(string base64, out JwtObject o)
    {
        try
        {
            o = Parse(base64);
            return true;
        }
        catch(Exception)
        {
            o = default;
            return false;
        }
    }

    public static bool TryJParseJson(string json, out JwtObject o)
    {
        try
        {
            o = ParseJson(json);
            return true;
        }
        catch(Exception)
        {
            o = default;
            return false;
        }
    }
    
    public static JwtObject Parse(string base64)
    {
        var json = Base64.UrlDecode(base64);
        
        var reader = new Utf8JsonReader(json);
        if (reader.TryRead() && reader.TokenType == JsonTokenType.StartObject)
        {
            var o = (JwtObject) Read(ref reader);
            return o;
        }
        
        return null;
    }

    public static JwtObject ParseJson(string json)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        if (reader.TryRead() && reader.TokenType == JsonTokenType.StartObject)
        {
            var o = (JwtObject) Read(ref reader);
            return o;
        }
        
        return null;
    }

    private static IJwtNode Read(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return new JwtValue(true);
            case JsonTokenType.False:
                return new JwtValue(true);
            case JsonTokenType.Null:
                return new JwtValue(null);
            case JsonTokenType.Number:
                return reader.TryGetInt32(out var i)
                    ? new JwtValue(i)
                    : reader.TryGetInt64(out var l)
                        ? new JwtValue(l)
                        : reader.TryGetDouble(out var d)
                            ? new JwtValue(d)
                            : throw new InvalidOperationException($"Could not find number type for '{reader.GetString()}'");
            case JsonTokenType.String:
                return new JwtValue(reader.GetString());
            case JsonTokenType.StartObject:
                var o = new JwtObject
                {
                    JsonStartPosition = (int) reader.TokenStartIndex
                };
                
                while (reader.TryRead() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    o[reader.GetString()] = reader.TryRead() ? Read(ref reader) : new JwtValue(null);
                }

                o.JsonEndPosition = reader.TokenEndIndex();
                
                return o;
            case JsonTokenType.StartArray:
                var a = new JwtArray
                {
                    JsonStartPosition = (int) reader.TokenStartIndex
                };
                while (reader.TryRead() && reader.TokenType != JsonTokenType.EndArray)
                {
                    a.Add(Read(ref reader));
                }

                a.JsonEndPosition = reader.TokenEndIndex();
                return a;
            default:
                return null;
        }
    }

    public string ToPrettyJson() => JsonSerializer.Serialize(this, PrettyUnsafe);
    public string ToJson() => JsonSerializer.Serialize(this, NonPretty);
    
}