using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using JwtViewer.ViewModels;

namespace JwtViewer.Views;

public partial class MainWindow : Window
{
    private readonly TextEditor _input;
    private readonly JwtEditor _accessToken;
    private readonly JwtColorizer _accessTokenColorizer;
    private readonly JwtEditor _idToken;
    private MainWindowViewModel _vm;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        _accessToken = this.Get<JwtEditor>("AccessToken");
        _accessTokenColorizer = new JwtColorizer();
        _idToken = this.Get<JwtEditor>("IdToken");
        _idToken.IsVisible = false;
        
        _input = this.Get<TextEditor>("Input");
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
                _accessToken.DataContext = vm.AccessToken;
                break;
            }
            case nameof(vm.IdToken):
            {
                _idToken.DataContext = vm.IdToken;
                break;
            }
        }
    }

    private void InputChanged(object sender, EventArgs e)
    {
        if (sender is not TextEditor input)
        {
            return;
        }

        _vm.Input = input.Text;
    }
}