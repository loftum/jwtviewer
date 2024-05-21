using Avalonia.Controls;
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
        InitializeComponent();

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