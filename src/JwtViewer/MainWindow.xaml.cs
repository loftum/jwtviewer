using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JwtViewer.IO;
using JwtViewer.ViewModels;

namespace JwtViewer
{
    public partial class MainWindow : Window
    {
        private readonly FileManager _fileManager = new FileManager();
        private MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var settings = _fileManager.LoadJson<WindowSettings>() ?? new WindowSettings();
            Height = settings.Height;
            Width = settings.Width;
            Vm.Load();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var settings = new WindowSettings
            {
                Top = Top,
                Left = Left,
                Height = Height,
                Width = Width
            };
            _fileManager.SaveJson(settings);
            Vm.Save();
            base.OnClosing(e);
        }

        private void Authorities_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && (e.SystemKey == Key.Delete || e.Key == Key.Delete))
            {
                e.Handled = true;
                Vm.RemoveAuthority();
            }
            else if (e.Key == Key.Enter || e.SystemKey == Key.Enter)
            {
                e.Handled = true;
                Vm.NewAuthority = ((ComboBox) sender).Text;
            }
        }
    }

    public class WindowSettings
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }

        public WindowSettings()
        {
            Height = 600;
            Width = 800;
        }
    }
}
