using System.Collections.Generic;
using System.Linq;
using JwtViewer.Core;
using JwtViewer.IO;

namespace JwtViewer.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        private readonly FileManager _fileManager = new FileManager();
        private string _raw;
        private string _authority;

        public List<string> Authorities { get; }

        private TokenValidator _tokenValidator;

        public string Authority
        {
            get { return _authority; }
            set
            {
                _authority = value;
                OnPropertyChanged();
                _tokenValidator = value == null ? null : TokenValidator.For(value);
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        public string Raw
        {
            get { return _raw; }
            set
            {
                _raw = value;
                IsValid = _tokenValidator.ValidateSignature(value);
                OnPropertyChanged();
                Jwt.Parse(value);
            }
        }

        public JwtViewModel Jwt { get; }

        public MainViewModel()
        {
            var settings = _fileManager.LoadJson<Settings>() ?? new Settings();
            Authorities = settings.Authorities;
            Authority = settings.Authorities.FirstOrDefault();
            Jwt = new JwtViewModel();
        }

        public void Save()
        {
            _fileManager.SaveJson(new TempData
            {
                Authority = Authority,
                Raw = Raw
            });
        }

        public void Load()
        {
            var data = _fileManager.LoadJson<TempData>() ?? new TempData();
            Authority = data.Authority ?? Authorities.FirstOrDefault();
            Raw = data.Raw;
        }
    }
}