using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using JwtViewer.ViewModels.Core;

namespace JwtViewer.Views;

public partial class JwtEditor : UserControl
{
    private Jwt _jwt;
    private readonly TextEditor _header;
    private readonly TextEditor _payload;
    private readonly TextEditor _signature;

    public JwtEditor()
    {
        AvaloniaXamlLoader.Load(this);

        var options = new TextEditorOptions
        {
            EnableHyperlinks = false,
            ConvertTabsToSpaces = true,
            EnableEmailHyperlinks = false
        };
        
        _header = this.Get<TextEditor>("Header");
        _header.Options = options;
            
        _payload = this.Get<TextEditor>("Payload");
        _payload.Options = options;
        _signature = this.Get<TextEditor>("Signature");
        _signature.Options = options;
        
        ChainElements(new[]{_header, _payload, _signature});
    }

    private static void ChainElements(IList<TextEditor> elements)
    {
        using var enumerator = elements.GetEnumerator();
        TextEditor previous = null;
        while (enumerator.MoveNext() && enumerator.Current != null)
        {
            var current = enumerator.Current;
            if (previous != null)
            {
                Chain(previous, current);
            }
            previous = current;
        }
        
        static void Chain(TextEditor previous, TextEditor current)
        {
            previous.KeyUp += (s, e) =>
            {
                var p = (TextEditor) s;
                if (e.Key == Key.Down && p.TextArea.Caret.Line == p.LineCount)
                {
                    current.CaretOffset = 0;
                    current.Focus();
                }
            };
        
            current.KeyUp += (s, e) =>
            {
                var c = (TextEditor) s;
                if (e.Key == Key.Up && c.TextArea.Caret.Line == 1)
                {
                    previous.CaretOffset = previous.Text.Length;
                    previous.Focus();
                }
            };
        }
    }


    protected override void OnDataContextChanged(EventArgs e)
    {
        var jwt = DataContext as Jwt;

        _jwt = jwt;
        if (!IsInitialized)
        {
            return;
        }
        _header.Text = _jwt?.Header?.ToPrettyJson();
        _payload.Text = _jwt?.Payload?.ToPrettyJson();
        _signature.Text = _jwt?.Signature;
        IsVisible = _jwt != null;
        
    }

    private void TextChanged(object sender, EventArgs e)
    {
        if (_jwt == null)
        {
            return;
        }
        if (ReferenceEquals(sender, _header))
        {
            _jwt.Header = JwtObject.TryJParseJson(_header.Text, out var o) ? o : null;
        }
        else if (ReferenceEquals(sender, _payload))
        {
            _jwt.Payload = JwtObject.TryJParseJson(_payload.Text, out var o) ? o : null;
        }
        else if (ReferenceEquals(sender, _signature))
        {
            _jwt.Signature = _signature.Text;
        }
    }
}