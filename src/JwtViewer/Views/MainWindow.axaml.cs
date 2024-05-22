using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using JwtViewer.IO;
using JwtViewer.ViewModels;
using ReactiveUI;

namespace JwtViewer.Views;

public partial class MainWindow : Window
{
    private readonly TextEditor _input;
    private readonly JwtEditor _accessToken;
    private readonly JwtEditor _idToken;
    private MainWindowViewModel _vm;
    private readonly FileManager _fileManager = new("jwtviewer");

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        _accessToken = this.Get<JwtEditor>("AccessToken");
        _idToken = this.Get<JwtEditor>("IdToken");
        _idToken.IsVisible = false;
        _input = this.Get<TextEditor>("Input");
        _input.Text = _vm?.Input;
        _input.Focus();
        var settings = _fileManager.LoadJsonOrDefault<WindowData>() ?? new WindowData();
        Position = new PixelPoint(settings.X, settings.Y);
        Height = settings.Height;
        Width = settings.Width;

        // Why is this necessary?
        var fontSize = this.GetObservable(FontSizeProperty);
        _accessToken.Bind(FontSizeProperty, fontSize);
        _idToken.Bind(FontSizeProperty, fontSize);
        
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var settings = new WindowData
        {
            X = Position.X,
            Y = Position.Y,
            Height = Height,
            Width = Width
        };
        _fileManager.SaveJson(settings);
        _vm?.Save();
        base.OnClosing(e);
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
            SetTextInput(_vm.Input);
            _vm.PropertyChanged += OnPropertyChanged;
            KeyBindings.Clear();

            var meta = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? KeyModifiers.Meta : KeyModifiers.Control;
            KeyBindings.Add(new KeyBinding
            {
                Gesture = new KeyGesture(Key.OemMinus, meta),
                Command = _vm.DecreaseFontSize
            });
            KeyBindings.Add(new KeyBinding
            {
                Gesture = new KeyGesture(Key.OemPlus, meta),
                Command = _vm.IncreaseFontSize
            });
            KeyBindings.Add(new KeyBinding
            {
                Gesture = new KeyGesture(Key.D0, meta),
                Command = _vm.ResetFontSize
            });
            this.SetPlatformSpecificKeyBindings();
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
            case nameof(vm.Input) when !_settingVmInput:
            {
                SetTextInput(vm.Input);
                break;
            }
        }
    }

    private void SetTextInput(string input)
    {
        _settingTextInput = true;
        if (_input != null)
        {
            _input.Text = input;    
        }
        _settingTextInput = false;
    }

    private bool _settingVmInput;
    private bool _settingTextInput;

    private void InputChanged(object sender, EventArgs e)
    {
        if (_settingTextInput || sender is not TextEditor input)
        {
            return;
        }
        _settingVmInput = true;
        _vm.Input = input.Text;
        _settingVmInput = false;
    }
}