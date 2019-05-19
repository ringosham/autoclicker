using System.Configuration;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Autoclicker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static readonly Regex FloatRegex = new Regex(@"\d*\.\d+|\d+|\d*\.");

        public MainWindow() {
            InitializeComponent();
            InitConfig(false);
            ReadConfig();
            ClickThread clickThread = new ClickThread();
            Thread thread = new Thread(clickThread.Run);
            thread.Start();
        }

        private void ReadConfig() {
            try {
                Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                Config config = manager.GetSection("Autoclicker") as Config;
                ((TextBox)this.FindName("Keybind")).Text = KeyUtil.ConvertKeyToString((Key)config.Key);
                ((TextBox)this.FindName("LowerBoundSecond")).Text =
                    config.LowerSecond.ToString(CultureInfo.InvariantCulture);
                ((TextBox)this.FindName("UpperBoundSecond")).Text =
                    config.UpperSecond.ToString(CultureInfo.InvariantCulture);
                ((ComboBox)this.FindName("ClickOption")).SelectedIndex = config.LeftClick ? 0 : 1;

            }
            catch (ConfigurationErrorsException) {
                InitConfig(true);
            }
        }

        private void InitConfig(bool reset) {
            Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (manager.Sections["Autoclicker"] == null || reset) {
                if (reset)
                    manager.Sections.Remove("Autoclicker");
                manager.Sections.Add("Autoclicker", new Config());
            }
            manager.Save(ConfigurationSaveMode.Full);
        }

        private void OnNewKeybind(object sender, KeyEventArgs e) {
            ((TextBox)sender).Text = KeyUtil.ConvertKeyToString(e.Key);
            e.Handled = true;
        }

        private void OnSetKey(object sender, RoutedEventArgs e) {
            string KeyBindText = ((TextBox)this.FindName("Keybind")).Text;
            string LowerBoundText = ((TextBox)this.FindName("LowerBoundSecond")).Text;
            string UpperBoundText = ((TextBox)this.FindName("UpperBoundSecond")).Text;
            if (KeyBindText.Trim().Length == 0) {
                MessageBox.Show("Please set your key bind first", "No key bind detected", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (LowerBoundText.EndsWith(".") || UpperBoundText.EndsWith(".")) {
                MessageBox.Show("Please type in the seconds between each click", "Invalid number format", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double LowerBoundSecond = double.Parse(LowerBoundText);
            double UpperBoundSecond = double.Parse(UpperBoundText);
            if (LowerBoundSecond.Equals(0f) || UpperBoundSecond.Equals(0f)) {
                MessageBox.Show("The time between clicks cannot be 0", "Invalid number format", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Key newKeyBind = KeyUtil.ConvertStringToKey(KeyBindText);
            Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //TODO Fix saving config
            /*manager.Sections. = new Config();
            collection["key"].Value = CharToKey(KeyBindText).ToString();
            collection["clickOption"].Value = ((ComboBoxItem) ((ComboBox) this.FindName("ClickOption")).SelectedItem).ContentStringFormat.Equals("Left click").ToString();*/

            manager.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(manager.AppSettings.SectionInformation.Name);

        }

        private void OnSecondKeyDown(object sender, TextCompositionEventArgs e) {
            if (!FloatRegex.IsMatch(e.Text))
                e.Handled = true;
        }
    }
}
