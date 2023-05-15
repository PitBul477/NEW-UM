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
        private MainWindow mainWindow;
        private int _internet = 0;

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            Internet.Checked += InternetEnable;
            Internet.Unchecked += InternetDisable;
            this.mainWindow = mainWindow;
            set1r.Text = ConfigurationManager.AppSettings["Interval1r"];
            set2r.Text = ConfigurationManager.AppSettings["Interval2r"];
            setIP.Text = ConfigurationManager.AppSettings["IP"];
            setPort.Text = ConfigurationManager.AppSettings["Ports"];
            info.Text = $"Информация о ПО:\n\nАвтор: PitBul477\nВерсия программы: 4.0\n\nПоддержать можно по следующей ссылке:\n";
            Run run1 = new Run("Donation Alerts");
            Hyperlink hyperlink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("https://www.donationalerts.com/r/pitbul477")
            };
            hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperlink_Click);
            info.Inlines.Add(hyperlink);
            info.Inlines.Add($"\n\nДополнительная информация и связь с автором через телеграм: ");
            Run run2 = new Run("Угадай Мелодию");
            hyperlink = new Hyperlink(run2)
            {
                NavigateUri = new Uri("https://t.me/UgadaiMelody")
            };
            hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperlink2_Click);
            info.Inlines.Add(hyperlink);
        }

        private void InternetEnable(object sender, RoutedEventArgs e)
        {
            _internet = 1;
        }

        private void InternetDisable(object sender, RoutedEventArgs e)
        {
            _internet = 0;            
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int points1Val, points2Val, portVal;
            bool points1Parsed = int.TryParse(set1r.Text, out points1Val);
            bool points2Parsed = int.TryParse(set2r.Text, out points2Val);
            bool portParsed = int.TryParse(setPort.Text, out portVal);

            if (points1Parsed && points1Val > 0)
                ConfigurationManager.AppSettings["Interval1r"] = points1Val.ToString();
            if (points2Parsed && points2Val > 0)
                ConfigurationManager.AppSettings["Interval2r"] = points2Val.ToString();
            if (portParsed && portVal > 0)
                ConfigurationManager.AppSettings["Ports"] = portVal.ToString();

            ConfigurationManager.AppSettings["IP"] = setIP.Text;

            if (_internet == 1)
                mainWindow.InternetEnable();
            else if (_internet == 0)
                mainWindow.InternetDisable();

            // Установка значения настройки
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (points1Parsed && points2Parsed && portParsed)
            {
                config.AppSettings.Settings["IP"].Value = setIP.Text;
                config.AppSettings.Settings["Ports"].Value = portVal.ToString();
                config.AppSettings.Settings["Interval1r"].Value = points1Val.ToString();
                config.AppSettings.Settings["Interval2r"].Value = points2Val.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                mainWindow.SetSetting();
            }
        }

        void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.donationalerts.com/r/pitbul477"); //открытие ссылки в браузере
        }
        void hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/UgadaiMelody"); //открытие ссылки в браузере
        }
    }
}
