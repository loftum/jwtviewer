using JwtViewer.IO;
using JwtViewer.ViewModels.Core;
using ReactiveUI;

namespace JwtViewer.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private string _input;
    private Jwt _accessToken;
    private Jwt _idToken;
    private readonly FileManager _fileManager = new("jwtviewer");

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
        set => this.RaiseAndSetIfChanged(ref _idToken, value);
    }

    public MainWindowViewModel()
    {
        _input = _fileManager.GetTextOrDefault("input.txt");
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
                AccessToken = accessToken;
            }

            if (Jwt.TryParse(tokenResponse.IdToken, out var idToken))
            {
                idToken.RawChanged += (_, _, newValue) =>
                {
                    tokenResponse.IdToken = newValue;
                    this.RaiseAndSetIfChanged(ref _input, tokenResponse.ToPrettyJson(), nameof(Input));
                };
                IdToken = idToken;
            }
        }
        
        else if (value.StartsWith("ey"))
        {
            Jwt.TryParse(value, out var accessToken);
            AccessToken = accessToken;
            IdToken = null;
        }
    }
}