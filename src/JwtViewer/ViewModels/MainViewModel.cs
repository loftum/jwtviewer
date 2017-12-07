using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JwtViewer.Core;
using JwtViewer.IO;

namespace JwtViewer.ViewModels
{
    public class MainViewModel : PropertyChangedNotifier
    {
        private readonly FileManager _fileManager = new FileManager();
        private Jwt _jwt;
        private string _authority;
        public event EventHandler AuthorityChanged;

        public ObservableCollection<string> Authorities { get; }

        public string Authority
        {
            get => _authority;
            set
            {
                _authority = value;
                AuthorityChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public string NewAuthority
        {
            set
            {
                if (value == null && Authority != null)
                {
                    Authorities.Remove(Authority);
                    Authority = Authorities.FirstOrDefault();
                    return;
                }
                if (Authority != null)
                {
                    return;
                }
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Authorities.Add(value);
                    Authority = value;
                }
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
            Authorities = new ObservableCollection<string>(settings.Authorities);
            Authority = settings.Authorities.FirstOrDefault();
            RefreshAuthority = new DelegateCommand(DoRefreshAuthority);
            AuthorityChanged += UpdateConfiguration;
        }

        private async void UpdateConfiguration(object sender, EventArgs e)
        {
            await Validation.LoadConfiguration(Authority);
            if (_jwt != null)
            {
                Validation.ValidateToken(_jwt);
            }
        }

        private async void DoRefreshAuthority()
        {
            await Validation.RefreshConfiguration(Authority);
            if (_jwt != null)
            {
                Validation.ValidateToken(_jwt);
            }
        }

        public void Save()
        {
            _fileManager.SaveJson(new TempData
            {
                Authority = Authority,
                Raw = Raw
            });
            _fileManager.SaveJson(new Settings
            {
                Authorities = Authorities.ToList()
            });
        }

        public void Load()
        {
            var data = _fileManager.LoadJson<TempData>() ?? new TempData();
            Authority = data.Authority ?? Authorities.FirstOrDefault();
            Raw = data.Raw;
        }

        public void RemoveAuthority()
        {
            if (Authority != null)
            {
                var index = Authorities.IndexOf(Authority);
                Authorities.Remove(Authority);
                while (index >= 0 && index >= Authorities.Count)
                {
                    index--;
                }
                Authority = index >= 0 && index < Authorities.Count
                    ? Authorities[index]
                    : Authorities.FirstOrDefault();
            }
        }
    }
}