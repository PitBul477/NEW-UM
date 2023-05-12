using MahApps.Metro.Controls;
using NEW_UM.Properties;
using System;
using System.Diagnostics;
using System.Runtime;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;


namespace NEW_UM
{
    public partial class SettingsWindow : Window
    {
        private Settings settings, back;
        private MainWindow mainWindow;
        private int _internet = 0;

        public SettingsWindow(MainWindow mainWindow, Settings setting)
        {
            InitializeComponent();
            Internet.Checked += InternetEnable;
            Internet.Unchecked += InternetDisable;
            this.mainWindow = mainWindow;
            //setting1.Text = mainWindow.GetSetting().ToString();
            settings = setting;
            back = Settings.Instance;
            back.Interval1r = settings.Interval1r;
            back.Interval2r = settings.Interval2r;
            back.IPadd = settings.IPadd;
            back.PortsAdd = settings.PortsAdd;
            set1r.Text = settings.Interval1r.ToString();
            set2r.Text = settings.Interval2r.ToString();
            setIP.Text = setting.IPadd.ToString();
            setPort.Text = setting.PortsAdd.ToString();
            info.Text = $"Информация о ПО:\n\nАвтор: PitBul477\nВерсия программы:4.0\n\nПоддержать можно по следующей ссылке:\n";
            Run run3 = new Run("Гид.");
            Hyperlink hyperlink = new Hyperlink(run3)
            {
                NavigateUri = new Uri("https://github.com/PitBul477/NEW-UM")
            };
            hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperlink_Click);
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
            settings.Interval1r = back.Interval1r;
            settings.Interval2r = back.Interval2r;
            settings.IPadd = back.IPadd;
            settings.PortsAdd = back.PortsAdd;
            mainWindow.SetSetting();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(set1r.Text, out int points1Val) && points1Val > 0)
                //mainWindow.SetSetting(pointsVal);
                settings.Interval1r = points1Val;
            if (int.TryParse(set2r.Text, out int points2Val) && points2Val > 0)
                //mainWindow.SetSetting(pointsVal);
                settings.Interval2r = points2Val;
            if (int.TryParse(setPort.Text, out int portVal) && portVal > 0)
                //mainWindow.SetSetting(pointsVal);
                settings.PortsAdd = portVal;
            settings.IPadd = setIP.Text;
            if (_internet == 1)
            {
                mainWindow.InternetEnable();
            }
            else if (_internet == 0)
            {
                mainWindow.InternetDisable();
            }
            mainWindow.SetSetting();
        }
        void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/PitBul477/NEW-UM"); //открытие ссылки в браузере
        }
    }
}
