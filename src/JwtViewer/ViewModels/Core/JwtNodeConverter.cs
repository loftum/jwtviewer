using System.Text.Json;
using System.Text.Json.Serialization;

namespace JwtViewer.ViewModels.Core;

public class JwtNodeConverter : JsonConverter<IJwtNode>
{
    private readonly int _offset;
    
    public JwtNodeConverter()
    {
        _offset = 0;
    }

    public JwtNodeConverter(int offset)
    {
        _offset = offset;
    }
    
    private static readonly Type[] Types = [typeof(IJwtNode), typeof(JwtObject), typeof(JwtArray), typeof(JwtValue)];
    
    public override bool CanConvert(Type typeToConvert)
    {
        return Types.Contains(typeToConvert);
    }

    public override IJwtNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                return new JwtValue(reader.GetInt32());
            case JsonTokenType.String:
                return new JwtValue(reader.GetString());
            case JsonTokenType.StartObject:
                var o = new JwtObject
                {
                    JsonStartPosition = _offset + (int) reader.TokenStartIndex
                };
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    o[reader.GetString()] = reader.Read() ? Read(ref reader, typeof(IJwtNode), null) : new JwtValue(null);
                }

                o.JsonEndPosition = _offset + (int) reader.TokenStartIndex;
                
                return o;
            case JsonTokenType.StartArray:
                var a = new JwtArray
                {
                    JsonStartPosition = _offset + (int) reader.TokenStartIndex
                };
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    a.Add(Read(ref reader, typeof(IJwtNode), null));
                }

                a.JsonEndPosition = _offset + (int) reader.TokenStartIndex;
                return a;
            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, IJwtNode value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case JwtValue v:
                JsonSerializer.Serialize(writer, v.Value, options);
                return;
            case JwtObject o:
                writer.WriteStartObject();
                foreach (var (k, v) in o)
                {
                    writer.WritePropertyName(k);
                    JsonSerializer.Serialize(writer, v, options);
                }
                writer.WriteEndObject();
                return;
            case JwtArray a:
                writer.WriteStartArray();
                foreach (var v in a)
                {
                    JsonSerializer.Serialize(writer, v, options);
                }
                writer.WriteEndArray();
                return;
            default:
                writer.WriteNullValue();
                return;
        }
    }
}