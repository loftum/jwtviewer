using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace JwtViewer.Controls;

public class JwtBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox); 
    
    public JwtBox()
    {
        InitializeIfNeeded();
        Background = Brushes.Fuchsia;
        
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Text == null)
        {
            return;
        }
        var formatted = new FormattedText(Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily), 1, Brushes.Fuchsia);
        
        context.DrawGeometry(Brushes.Fuchsia,  new Pen(), formatted.BuildGeometry(new Point(0, 0)));
    }
}