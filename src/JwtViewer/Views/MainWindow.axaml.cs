using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using JwtViewer.ViewModels;

namespace JwtViewer.Views;

public partial class MainWindow : Window
{
    private readonly TextBox _input;
    private readonly TextEditor _accessToken;
    private readonly TextEditor _idToken;
    private MainWindowViewModel _vm;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        _accessToken = this.Get<TextEditor>("AccessToken");

        var options = new TextEditorOptions
        {
            EnableHyperlinks = false,
            ConvertTabsToSpaces = true,
            EnableEmailHyperlinks = false
        };
        
        _accessToken.Options = options;
        
        _idToken = this.Get<TextEditor>("IdToken");
        _idToken.Options = options;
        _idToken.IsVisible = false;
        
        _input = this.Get<TextBox>("Input");
        _input.Focus();
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        var old = _vm;
        if (old != null)
        {
            old.PropertyChanged -= OnPropertyChanged;
        }
        _vm = DataContext as MainWindowViewModel;
        if (_vm != null)
        {
            _vm.PropertyChanged += OnPropertyChanged;    
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainWindowViewModel vm)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(vm.AccessToken):
            {
                _accessToken.Text = vm.AccessToken;
                break;
            }
            case nameof(vm.IdToken):
            {
                _idToken.Text = vm.IdToken;
                _idToken.IsVisible = vm.IdToken != null;
                
                break;
            }
        }
    }
}