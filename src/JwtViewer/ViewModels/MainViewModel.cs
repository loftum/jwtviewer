using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using JwtViewer.Core;
using JwtViewer.IO;

namespace JwtViewer.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class MainViewModel : PropertyChangedNotifier
    {
        private readonly FileManager _fileManager = new FileManager();
        private Jwt _jwt;
        private string _authority;

        public List<string> Authorities { get; }

        public string Authority
        {
            get => _authority;
            set
            {
                _authority = value;
                Validation.Load(value);
                if (_jwt != null)
                {
                    Validation.ValidateToken(_jwt);
                }
                OnPropertyChanged();
            }
        }

        public string Raw
        {
            get => _jwt?.Raw;
            set
            {
                var jwt = new Jwt(value);
                Jwt.Show(jwt);
                Validation.ValidateToken(jwt);
                _jwt = jwt;
                OnPropertyChanged();
            }
        }

        public ValidationViewModel Validation { get; }
        public JwtViewModel Jwt { get; }

        public ICommand RefreshAuthority { get; }

        public MainViewModel()
        {
            Jwt = new JwtViewModel();
            Validation = new ValidationViewModel();
            var settings = _fileManager.LoadJson<Settings>() ?? new Settings();
            Authorities = settings.Authorities;
            Authority = settings.Authorities.FirstOrDefault();
            RefreshAuthority = new DelegateCommand(DoRefreshAuthority);
        }

        private void DoRefreshAuthority()
        {
            Validation.Refresh(Authority);
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