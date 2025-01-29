using System.Windows.Input;
using Avalonia.Controls;
using JwtViewer.IO;
using JwtViewer.ViewModels.Core;
using ReactiveUI;

namespace JwtViewer.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private const int DefaultFontSize = 14;
    
    private string _input;
    private Jwt _accessToken;
    private Jwt _idToken;
    private readonly FileManager _fileManager = new("jwtviewer");
    private int _fontSize = DefaultFontSize;
    private GridLength _idtokenWidth;

    public string Input
    {
        get => _input;
        set
        {
            Parse(value);
            _input = value;
        }
    }

    public Jwt AccessToken
    {
        get => _accessToken;
        set => this.RaiseAndSetIfChanged(ref _accessToken, value);
    }

    public Jwt IdToken
    {
        get => _idToken;
        set
        {
            this.RaiseAndSetIfChanged(ref _idToken, value);
            IdtokenWidth = value == null ? new GridLength(0) : GridLength.Star;
        }
    }

    // Controls width of id_token editor.
    // Set to 0 when IdToken is null, so only access token is shown
    public GridLength IdtokenWidth
    {
        get => _idtokenWidth;
        set => this.RaiseAndSetIfChanged(ref _idtokenWidth, value);
    }

    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public ICommand IncreaseFontSize { get; }
    public ICommand DecreaseFontSize { get; }
    public ICommand ResetFontSize { get; }
    

    public MainWindowViewModel()
    {
        _input = _fileManager.GetTextOrDefault("input.txt");
        DecreaseFontSize = ReactiveCommand.Create(() =>
        {
            if (FontSize >= 4)
            {
                FontSize--;
            }
        });
        
        IncreaseFontSize = ReactiveCommand.Create(() =>
        {
            if (FontSize < 80)
            {
                FontSize++;
            }
        });
        ResetFontSize = ReactiveCommand.Create(() =>
        {
            FontSize = DefaultFontSize;
        });
    }

    public void Save()
    {
        _fileManager.SaveText(_input, "input.txt");
    }

    private void Parse(string value)
    {
        value = value?.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            AccessToken = null;
            IdToken = null;
            return;
        }

        if (value.StartsWith('{'))
        {
            if (!TokenResponse.TryParse(value, out var tokenResponse))
            {
                AccessToken = null;
                IdToken = null;
                return;
            }

            if (Jwt.TryParse(tokenResponse.AccessToken, out var accessToken))
            {
                accessToken.RawChanged += (_, _, newValue) =>
                {
                    tokenResponse.AccessToken = newValue;
                    this.RaiseAndSetIfChanged(ref _input, tokenResponse.ToPrettyJson(), nameof(Input));
                };
                accessToken.Title = "access_token";
            }
            AccessToken = accessToken;

            if (Jwt.TryParse(tokenResponse.IdToken, out var idToken))
            {
                idToken.RawChanged += (_, _, newValue) =>
                {
                    tokenResponse.IdToken = newValue;
                    this.RaiseAndSetIfChanged(ref _input, tokenResponse.ToPrettyJson(), nameof(Input));
                };
                idToken.Title = "id_token";
            }
            IdToken = idToken;
        }
        
        else if (value.StartsWith("ey"))
        {
            Jwt.TryParse(value, out var accessToken);
            AccessToken = accessToken;
            IdToken = null;
        }
    }
}