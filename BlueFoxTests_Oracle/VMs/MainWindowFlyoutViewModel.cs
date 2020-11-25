using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace BlueFoxTests_Oracle.VMs
{
    class MainWindowFlyoutViewModel
    {
        public ICommand ToggleBaseCommand { get; } = new AnotherCommandImplementation(o => ApplyBase((bool)o));

        public IEnumerable<Swatch> Swatches { get; }

        public ICommand ShowLeftFlyoutCommand { get; }

        private readonly ResourceDictionary _dialogDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        public Flyout LeftFlyout { get; set; }

        public ICommand ShowLogOutDialogCommand { get; }

        public MainWindowFlyoutViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            ShowLeftFlyoutCommand = new AnotherCommandImplementation(_ => ShowLeftFlyout());
            ShowLogOutDialogCommand = new AnotherCommandImplementation(_ => LogOutAcceptDialog());
        }

        private void ShowLeftFlyout()
        {
            LeftFlyout.IsOpen = !LeftFlyout.IsOpen;
        }

        private void LogOutAcceptDialog()
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary = _dialogDictionary,
                NegativeButtonText = "NO",
                AffirmativeButtonText = "YES"
            };
            DialogCoordinator.Instance.ShowMessageAsync(this, "Are you sure you want to log out?", "Confirm Log Out", MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings);
        }

        private static void ApplyBase(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();

            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
