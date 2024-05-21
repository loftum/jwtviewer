using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using JwtViewer.ViewModels.Core;

namespace JwtViewer.Views;

public class JwtColorizer : DocumentColorizingTransformer
{
    private Jwt _jwt;

    public Jwt Jwt
    {
        get => _jwt;
        set => _jwt = value;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        var jwt = _jwt;
        if (jwt == null)
        {
            return;
        }

        // foreach (var node in nodes)
        // {
        //     var start = Math.Max(line.Offset, node.JsonStartPosition);
        //     var end = Math.Min(line.EndOffset, node.JsonEndPosition);
        //     ChangeLinePart(start, end, MakeColor(Brushes.Magenta));
        // }
    }
    
    private static Action<VisualLineElement> MakeColor(IBrush brush, FontWeight? weight = null, FontStyle? fontstyle = null)
    {
        return e =>
        {
            e.TextRunProperties.SetForegroundBrush(brush);
            e.TextRunProperties.SetTypeface(new Typeface(
                e.TextRunProperties.Typeface.FontFamily,
                fontstyle.GetValueOrDefault(e.TextRunProperties.Typeface.Style),
                weight.GetValueOrDefault(e.TextRunProperties.Typeface.Weight))
            );
        };
    }
}