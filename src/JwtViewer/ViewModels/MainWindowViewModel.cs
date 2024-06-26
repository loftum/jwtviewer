using ReactiveUI;

namespace JwtViewer.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private string _input;
    private string _accessToken;
    private string _idToken;

    public string Input
    {
        get => _input;
        set
        {
            Parse(value);
            _input = value;
            // this.RaiseAndSetIfChanged(ref _input, value);
        }
    }

    public string AccessToken
    {
        get => _accessToken;
        set => this.RaiseAndSetIfChanged(ref _accessToken, value);
    }

    public string IdToken
    {
        get => _idToken;
        set => this.RaiseAndSetIfChanged(ref _idToken, value);
    }


    public MainWindowViewModel()
    {
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
            TokenResponse.TryParse(value, out var tokenResponse);
            Jwt.TryParse(tokenResponse?.AccessToken, out var accessToken);
            AccessToken = accessToken?.DecodedJwt;
            Jwt.TryParse(tokenResponse?.IdToken, out var jdToken);
            IdToken = jdToken?.DecodedJwt;
        }
        
        else if (value.StartsWith("ey"))
        {
            Jwt.TryParse(value, out var accessToken);
            AccessToken = accessToken?.DecodedJwt;
            IdToken = null;
        }
    }
}