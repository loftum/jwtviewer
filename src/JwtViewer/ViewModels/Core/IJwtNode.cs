using System.Text.Json.Serialization;

namespace JwtViewer.ViewModels.Core;

public interface IJwtNode
{
    [JsonIgnore]
    public int JsonStartPosition { get; }
    
    [JsonIgnore]
    public int JsonEndPosition { get; }
}