using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using SharpIRC.Properties;

namespace SharpIRC.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow
    {

         public SettingsWindow(Window parent)
            : this()
        {
            Owner = parent;
        }

        public override void EndInit()
        {
            base.EndInit();
            ThemeComboBox.ItemsSource = new[]{"Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", 
                    "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Sienna"};
            if (Settings.Default.Style == "Dark")
                StyleDarkRadio.IsChecked = true;
            else
                StyleLightRadio.IsChecked = true;

        }

        private SettingsWindow()
        {
            InitializeComponent();

            Settings.Default.SettingChanging += (sender, args) => Debug.WriteLine("setting changing");
            Settings.Default.SettingsSaving += (sender, args) => Debug.WriteLine("settings saved");

            SaveButton.Click += (sender, args) =>
            {
                Save();
                Close();
            };

            CancelButton.Click += (sender, args) => Close();
        }

        private void Save()
        {
            Settings.Default.Nickname = NicknameTextBox.Text;
            Settings.Default.Nickname2 = SecondTextBox.Text;
            Settings.Default.Nickname3 = ThirdTextBox.Text;
            Settings.Default.RealName = RealNameTextBox.Text;
            Settings.Default.LeaveMessage = LeaveTextBox.Text;
            Settings.Default.Server = ServerTextBox.Text;
            Settings.Default.Port = PortTextBox.Text;
            Settings.Default.Theme = ThemeComboBox.Text;
            Settings.Default.Style = StyleDarkRadio.IsChecked != null && (bool) (StyleDarkRadio.IsChecked) ? "Dark" : "Light";
            Settings.Default.Save();
            ThemeManager.ChangeTheme(Application.Current,
                ThemeManager.DefaultAccents.First(x => x.Name == ThemeComboBox.Text),
                (Settings.Default.Style == "Dark") ? Theme.Dark : Theme.Light);
        }


    }
}
