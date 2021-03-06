﻿using System;
using System.Threading.Tasks;
using JwtViewer.Core;
using Newtonsoft.Json;

namespace JwtViewer.ViewModels
{
    public class ValidationViewModel : PropertyChangedNotifier
    {
        private string _config;
        public string Config
        {
            get => _config;
            set
            {
                _config = value;
                OnPropertyChanged();
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        private string _validationResult;
        public string ValidationResult
        {
            get => _validationResult;
            set
            {
                _validationResult = value;
                OnPropertyChanged();
            }
        }

        private TokenValidator _tokenValidator;

        public async Task LoadConfiguration(string authority)
        {
            try
            {
                var config = await DiscoveryLoader.LoadConfigurationAsync(authority);
                _tokenValidator = new TokenValidator(config);
                Config = JsonConvert.SerializeObject(config, Formatting.Indented);
            }
            catch (Exception e)
            {
                _tokenValidator = new TokenValidator();
                Config = e.ToString();
            }
        }

        public async Task RefreshConfiguration(string authority)
        {
            try
            {
                var config = await DiscoveryLoader.RefreshConfigurationAsync(authority);
                _tokenValidator = new TokenValidator(config);
                Config = JsonConvert.SerializeObject(config, Formatting.Indented);
            }
            catch (Exception e)
            {
                _tokenValidator = new TokenValidator();
                Config = e.ToString();
            }
        }

        public void ValidateToken(Jwt jwt)
        {
            try
            {
                _tokenValidator.ValidateToken(jwt);
                ValidationResult = "Valid";
                IsValid = true;
            }
            catch (Exception e)
            {
                ValidationResult = e.ToString();
                IsValid = false;
            }
        }
    }
}