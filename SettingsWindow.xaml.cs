using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Text;

namespace NEW_UM
{
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        private readonly MainWindow mainWindow;
        private bool _internet, _delay;
        private string _pressedKeys;
        private int _btn = 0;

        public event PropertyChangedEventHandler PropertyChanged;

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
            setpoint2r.Text = ConfigurationManager.AppSettings["MaxPoint2r"];
            stop_btn.Content = ConfigurationManager.AppSettings["BtnStop"];
            play_btn.Content = ConfigurationManager.AppSettings["BtnPlay"];
            answer_btn.Content = ConfigurationManager.AppSettings["BtnAnswer"];
            final_btn.Content = ConfigurationManager.AppSettings["BtnFinal"];
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
            _pressedKeys = string.Empty;
            DataContext = this;
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
            bool max2rParsed = int.TryParse(setpoint2r.Text, out int max2rVal);

            if (points1Parsed && points1Val > 0)
                ConfigurationManager.AppSettings["Interval1r"] = points1Val.ToString();
            if (points2Parsed && points2Val > 0)
                ConfigurationManager.AppSettings["Interval2r"] = points2Val.ToString();
            if (portParsed && portVal > 0)
                ConfigurationManager.AppSettings["Ports"] = portVal.ToString();
            if (delayParsed && delayVal > 0)
                ConfigurationManager.AppSettings["Delay2r"] = delayVal.ToString();
            if (max2rParsed && max2rVal > 0)
                ConfigurationManager.AppSettings["MaxPoint2r"] = max2rVal.ToString();

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
                config.AppSettings.Settings["MaxPoint2r"].Value = max2rVal.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                mainWindow.SetSetting();
            }
        }
        //private void Setting_KeyDown(object sender, KeyEventArgs e)
        //{
        //    RecordKey(e.Key);
        //}

        void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.donationalerts.com/r/pitbul477"); //открытие ссылки в браузере
        }

        private void Click_Set_Button(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Content.ToString())
            {
                case "Остановка музыки":
                    _btn = 1;
                    //MessageBox.Show("A");
                    break;
                case "Продолжить проигрывание":
                    _btn = 2;
                    //MessageBox.Show("B");
                    break;
                case "Ответ":
                    _btn = 3;
                    break;
                case "Вывод ответа в финале":
                    _btn = 4;
                    break;
            }
            button.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            ButtonLabel.Visibility = Visibility.Visible;
            ButtonLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            button.PreviewKeyDown += Button_PreviewKeyDown;
            // вызываем метод Focus(), чтобы кнопка получила фокус
            button.Focus();
        }

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RecordKey(e.Key);
            // Выполните необходимые действия при нажатии клавиши
        }

        public string PressedKeys
        {
            get {return _pressedKeys; }
            set
            {
                _pressedKeys = value;
                OnPropertyChanged("PressedKeys");
                ButtonLabel.Content = _pressedKeys;
            }
        }

        public void RecordKey(Key key)
        {
            StringBuilder sb = new StringBuilder();

            // Добавляем клавиши-модификаторы (Shift, Ctrl), если они были нажаты
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    sb.Append("LeftShift+");
                }
                else if (Keyboard.IsKeyDown(Key.RightShift))
                {
                    sb.Append("RightShift+");
                }
            }

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    sb.Append("LeftCtrl+");
                }
                else if (Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    sb.Append("RightCtrl+");
                }
            }

            // Проверяем, что клавиша не является "System" и не является модификаторной клавишей
            if (key != Key.System && !IsModifierKey(key))
            {
                // Добавляем нажатую клавишу
                sb.Append(key.ToString());

                // Обновляем строку с нажатыми клавишами
                PressedKeys = sb.ToString();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift || key == Key.LeftCtrl || key == Key.RightCtrl;
        }


        private void Button_Click_Apply_btn(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            switch (_btn)
            {
                case 0:
                    break;
                case 1:
                    if (ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnPlay"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnAnswer"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnFinal"])
                    {
                        ButtonLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        return;
                    }
                        ConfigurationManager.AppSettings["BtnStop"] = ButtonLabel.Content.ToString();
                    config.AppSettings.Settings["BtnStop"].Value = ButtonLabel.Content.ToString();
                    stop_btn.Content = ConfigurationManager.AppSettings["BtnStop"];
                    //stop.Background = new SolidColorBrush(Color.FromArgb(255, 0, 100, 0));
                    stop.Style = (Style)this.FindResource("CustomButtonStyle");
                    break;
                case 2:
                    if (ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnStop"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnAnswer"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnFinal"])
                    {
                        ButtonLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        return;
                    }
                    ConfigurationManager.AppSettings["BtnPlay"] = ButtonLabel.Content.ToString();
                    config.AppSettings.Settings["BtnPlay"].Value = ButtonLabel.Content.ToString();
                    play_btn.Content = ConfigurationManager.AppSettings["BtnPlay"];
                    //play.Background = new SolidColorBrush(Color.FromArgb(255, 127, 255, 0));
                    play.Style = (Style)this.FindResource("CustomButtonStyle");
                    break;
                case 3:
                    if (ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnStop"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnPlay"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnFinal"])
                    {
                        ButtonLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        return;
                    }
                    ConfigurationManager.AppSettings["BtnAnswer"] = ButtonLabel.Content.ToString();
                    config.AppSettings.Settings["BtnAnswer"].Value = ButtonLabel.Content.ToString();
                    answer_btn.Content = ConfigurationManager.AppSettings["BtnAnswer"];
                    //answer.Background = new SolidColorBrush(Color.FromArgb(255, 0, 100, 0));
                    answer.Style = (Style)this.FindResource("CustomButtonStyle");
                    break;
                case 4:
                    if (ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnStop"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnPlay"] || ButtonLabel.Content.ToString() == ConfigurationManager.AppSettings["BtnAnswer"])
                    {
                        ButtonLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        return;
                    }
                    ConfigurationManager.AppSettings["BtnFinal"] = ButtonLabel.Content.ToString();
                    config.AppSettings.Settings["BtnFinal"].Value = ButtonLabel.Content.ToString();
                    final_btn.Content = ConfigurationManager.AppSettings["BtnFinal"];
                    //final.Background = new SolidColorBrush(Color.FromArgb(255, 0, 100, 0));
                    final.Style = (Style)this.FindResource("CustomButtonStyle");
                    break;
                default:
                    break;
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            mainWindow.RefreshButton();
            ButtonLabel.Visibility = Visibility.Hidden;
            _pressedKeys = string.Empty;
        }


        private void Button_Click_Default_btn(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.AppSettings["BtnStop"] = "Decimal";
            config.AppSettings.Settings["BtnStop"].Value = "Decimal";
            stop_btn.Content = ConfigurationManager.AppSettings["BtnStop"];
            ConfigurationManager.AppSettings["BtnPlay"] = "NumPad0";
            config.AppSettings.Settings["BtnPlay"].Value = "NumPad0";
            play_btn.Content = ConfigurationManager.AppSettings["BtnPlay"];
            ConfigurationManager.AppSettings["BtnAnswer"] = "A";
            config.AppSettings.Settings["BtnAnswer"].Value = "A";
            answer_btn.Content = ConfigurationManager.AppSettings["BtnAnswer"];
            ConfigurationManager.AppSettings["BtnFinal"] = "F";
            config.AppSettings.Settings["BtnFinal"].Value = "F";
            final_btn.Content = ConfigurationManager.AppSettings["BtnFinal"];
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            mainWindow.RefreshButton();
            ButtonLabel.Visibility = Visibility.Hidden;
            _pressedKeys = string.Empty;
        }

        void Hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/UgadaiMelody"); //открытие ссылки в браузере
        }
    }
}
