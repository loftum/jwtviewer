using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JwtViewer.Annotations;
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
            Authorities = new List<string>
            {
                "hansolav-innlogging.dev.nrk.no",
                "test-innlogging.nrk.no",
                "stage-innlogging.nrk.no",
                "preprod-innlogging.nrk.no",
                "innlogging.nrk.no"
            };
            Authority = Authorities.First();
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
            Authority = data.Authority ?? Authorities.First();
            Raw = data.Raw;
        }
    }
}