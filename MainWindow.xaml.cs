using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Threading;

namespace Autoclicker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static readonly Regex FloatRegex = new Regex(@"\d*\.\d+|\d+|\d*\.");
        private static KeyboardHook hook = new KeyboardHook();
        private static Thread clickThread;

        public MainWindow() {
            InitializeComponent();
            ReadConfig();
            hook.OnKeyPressed += onGlobalKeyPress;
            hook.OnKeyUnpressed += onGlobalKeyUnpress;
            hook.HookKeyboard();
        }

        public void onGlobalKeyPress(object sender, int e) {
            Key key = KeyInterop.KeyFromVirtualKey(e);
            if (key.Equals(Config.Instance.Key)) {
                if (clickThread == null || !clickThread.IsAlive) {
                    ClickThread click = new ClickThread(Config.Instance.Mean, Config.Instance.Sigma, Config.Instance.LeftClick);
                    clickThread = new Thread(click.Run);
                    clickThread.Start();
                    return;
                }
                clickThread.Abort();
            }
        }

        public void onGlobalKeyUnpress(object sender, int e) { }

        private void ReadConfig() {
            try {
                Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection appSetting = manager.AppSettings.Settings;
                if (appSetting["keybind"] == null || appSetting["mean"] == null || appSetting["sigma"] == null || appSetting["clickOption"] == null)
                    InitConfig();
                Config.Instance.Key = (Key)int.Parse(appSetting["keybind"].Value);
                Config.Instance.Mean = double.Parse(appSetting["mean"].Value);
                Config.Instance.Sigma = double.Parse(appSetting["sigma"].Value);
                if (!appSetting["clickOption"].Value.Equals("left") && !appSetting["clickOption"].Value.Equals("right"))
                    throw new InvalidConstraintException("Invalid config value in clickOption");
                Config.Instance.LeftClick = appSetting["clickOption"].Value.Equals("left");

                ((TextBox)this.FindName("Keybind")).Text = KeyUtil.ConvertKeyToString(Config.Instance.Key);
                ((TextBox)this.FindName("MeanTextBox")).Text =
                    Config.Instance.Mean.ToString(CultureInfo.InvariantCulture);
                ((TextBox)this.FindName("SigmaTextBox")).Text =
                    Config.Instance.Sigma.ToString(CultureInfo.InvariantCulture);
                ((ComboBox)this.FindName("ClickOption")).SelectedIndex = Config.Instance.LeftClick ? 0 : 1;

            }
            catch (Exception) {
                InitConfig();
            }
        }

        private void InitConfig() {
            Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection configCollection = manager.AppSettings.Settings;
            configCollection.Clear();
            configCollection.Add("keybind", "146");
            configCollection.Add("mean", "0.25");
            configCollection.Add("sigma", "0.01");
            configCollection.Add("clickOption", "left");
            manager.Save(ConfigurationSaveMode.Full);
            ReadConfig();
        }

        private void OnNewKeybind(object sender, KeyEventArgs e) {
            ((TextBox)sender).Text = KeyUtil.ConvertKeyToString(e.Key);
            e.Handled = true;
        }

        private void OnSetKey(object sender, RoutedEventArgs e) {
            string KeyBindText = ((TextBox)this.FindName("Keybind")).Text;
            string MeanText = ((TextBox)this.FindName("MeanTextBox")).Text;
            string SigmaText = ((TextBox)this.FindName("SigmaTextBox")).Text;
            if (KeyBindText.Trim().Length == 0 || KeyBindText.Equals("None")) {
                MessageBox.Show("Please set your key bind first", "No key bind detected", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (MeanText.EndsWith(".") || SigmaText.EndsWith(".")) {
                MessageBox.Show("Please type in the seconds between each click", "Invalid number format", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double Mean = double.Parse(MeanText);
            double Sigma = double.Parse(SigmaText);
            if (Mean.Equals(0f)) {
                MessageBox.Show("The time between clicks cannot be 0", "Invalid number format", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool leftClick = ((ComboBox)this.FindName("ClickOption")).SelectedIndex == 0;
            Key newKeyBind = KeyUtil.ConvertStringToKey(KeyBindText);
            Config.Instance.Key = newKeyBind;
            Config.Instance.Mean = Mean;
            Config.Instance.Sigma = Sigma;
            Config.Instance.LeftClick = leftClick;
            Configuration manager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection appSetting = manager.AppSettings.Settings;
            appSetting["keybind"].Value = ((int)Config.Instance.Key).ToString();
            appSetting["mean"].Value = Config.Instance.Mean.ToString(CultureInfo.InvariantCulture);
            appSetting["sigma"].Value = Config.Instance.Sigma.ToString(CultureInfo.InvariantCulture);
            appSetting["clickOption"].Value = Config.Instance.LeftClick ? "left" : "right";
            manager.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection(manager.AppSettings.SectionInformation.Name);
            MessageBox.Show("Settings applied", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnSecondKeyDown(object sender, TextCompositionEventArgs e) {
            if (!FloatRegex.IsMatch(e.Text))
                e.Handled = true;
        }

        private void OnHelp(object sender, RoutedEventArgs e) {
            StringBuilder builder = new StringBuilder();
            builder.Append("Gaussian distribution works like this:\n");
            builder.Append("The standard deviation is often dubbed as the sigma\n");
            builder.Append("\n");
            builder.Append("68% of the generated numbers is within mean ± sigma\n");
            builder.Append("95% of the generated numbers is within mean ± 2 * sigma\n");
            builder.Append("99.7% of the generated numbers is within mean ± 3 * sigma\n\n");
            builder.Append("This means the timing of your clicks range from (mean - 3 * sigma) to (mean + 3 * sigma)\n");
            builder.Append("If sigma is set to 0, you will be clicking at a constant rate\n");
            builder.Append("Tip: Set the standard deviation to a small number (>0.03) to prevent clicking too fast");
            MessageBox.Show(builder.ToString(), "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
