namespace JwtViewer.ViewModels.Core;

public class JwtValue : IJwtNode
{
    public object Value { get; set; }
    public int Position { get; set; }
    public int JsonStartPosition { get; set; }
    public int JsonEndPosition { get; set; }

    public JwtValue(object value)
    {
        Value = value;
    }

    public JwtValue()
    {
        
    }
}