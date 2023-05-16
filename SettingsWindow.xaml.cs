using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;


namespace NEW_UM
{
    public partial class SettingsWindow : Window
    {        
        private readonly MainWindow mainWindow;
        private bool _internet, _delay;

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            Internet.Checked += InternetEnable;
            Internet.Unchecked += InternetDisable;
            delay2r.Checked += DelayEnable;
            delay2r.Unchecked += DelayDisable;
            ResizeCheck.Checked += ResizeEnable;
            ResizeCheck.Unchecked += ResizeDisable;
            this.mainWindow = mainWindow;
            set1r.Text = ConfigurationManager.AppSettings["Interval1r"];
            set2r.Text = ConfigurationManager.AppSettings["Interval2r"];
            setIP.Text = ConfigurationManager.AppSettings["IP"];
            setPort.Text = ConfigurationManager.AppSettings["Ports"];
            set2r_delay.Text = ConfigurationManager.AppSettings["Delay2r"];
            info.Text = $"Информация о ПО:\n\nАвтор: PitBul477\nВерсия программы: 4.0\n\nПоддержать можно по следующей ссылке:\n";
            Run run1 = new Run("Donation Alerts");
            Hyperlink hyperlink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("https://www.donationalerts.com/r/pitbul477")
            };
            hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(Hyperlink_Click);
            info.Inlines.Add(hyperlink);
            info.Inlines.Add($"\n\nДополнительная информация и связь с автором через телеграм: ");
            Run run2 = new Run("Угадай Мелодию");
            hyperlink = new Hyperlink(run2)
            {
                NavigateUri = new Uri("https://t.me/UgadaiMelody")
            };
            hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(Hyperlink2_Click);
            info.Inlines.Add(hyperlink);
        }

        private void InternetEnable(object sender, RoutedEventArgs e)
        {
            _internet = true;
        }

        private void InternetDisable(object sender, RoutedEventArgs e)
        {
            _internet = false;            
        }

        private void ResizeEnable(object sender, RoutedEventArgs e)
        {
            mainWindow.ResizeMode = ResizeMode.CanResize;
        }

        private void ResizeDisable(object sender, RoutedEventArgs e)
        {
            mainWindow.ResizeMode = ResizeMode.NoResize;
        }

        private void DelayEnable(object sender, RoutedEventArgs e)
        {
            _delay = true;
        }

        private void DelayDisable(object sender, RoutedEventArgs e)
        {
            _delay = false;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainWindow.SetSetting();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool points1Parsed = int.TryParse(set1r.Text, out int points1Val);
            bool points2Parsed = int.TryParse(set2r.Text, out int points2Val);
            bool portParsed = int.TryParse(setPort.Text, out int portVal);
            bool delayParsed = int.TryParse(set2r_delay.Text, out int delayVal);

            if (points1Parsed && points1Val > 0)
                ConfigurationManager.AppSettings["Interval1r"] = points1Val.ToString();
            if (points2Parsed && points2Val > 0)
                ConfigurationManager.AppSettings["Interval2r"] = points2Val.ToString();
            if (portParsed && portVal > 0)
                ConfigurationManager.AppSettings["Ports"] = portVal.ToString();
            if (delayParsed && delayVal > 0)
                ConfigurationManager.AppSettings["Delay2r"] = delayVal.ToString();

            ConfigurationManager.AppSettings["IP"] = setIP.Text;

            if (_internet == true)
                mainWindow.InternetEnable();
            else if (_internet == false)
                mainWindow.InternetDisable();

            if (_delay == true)
                mainWindow.SetDelay(delayVal);
            else if (_delay == false)
                mainWindow.SetDelay(0);

            // Установка значения настройки
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (points1Parsed && points2Parsed && portParsed)
            {
                config.AppSettings.Settings["IP"].Value = setIP.Text;
                config.AppSettings.Settings["Ports"].Value = portVal.ToString();
                config.AppSettings.Settings["Interval1r"].Value = points1Val.ToString();
                config.AppSettings.Settings["Interval2r"].Value = points2Val.ToString();
                config.AppSettings.Settings["Delay2r"].Value = delayVal.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                mainWindow.SetSetting();
            }
        }

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.donationalerts.com/r/pitbul477"); //открытие ссылки в браузере
        }
        void Hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/UgadaiMelody"); //открытие ссылки в браузере
        }
    }
}
