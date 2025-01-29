using System.Text;
using System.Text.Json;

namespace JwtViewer.ViewModels.Core;

public static class Utf8JsonReaderExtensions
{
    public static bool TryRead(this ref Utf8JsonReader reader)
    {
        try
        {
            return reader.Read();
        }
        catch (JsonException)
        {
            return false;
        }
    }
    
    public static int TokenEndIndex(this ref Utf8JsonReader reader)
    {
        return (int) reader.TokenStartIndex + reader.TokenCharLength();
    }
    
    public static int TokenCharLength(this ref Utf8JsonReader reader)
    {
        return reader.TokenType switch
        {
            JsonTokenType.StartObject => 1,
            JsonTokenType.EndObject => 1,
            JsonTokenType.StartArray => 1,
            JsonTokenType.EndArray => 1,
            JsonTokenType.True => 4,
            JsonTokenType.False => 4,
            JsonTokenType.Null => 4,
            _ => Encoding.UTF8.GetCharCount(reader.ValueSpan)
        };
    }
}