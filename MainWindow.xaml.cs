using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
//using Path = System.IO.Path;

namespace NEW_UM
{
    public partial class MainWindow : Window
    {
        private readonly static string superClientId = "SuperClient";
        private readonly string path = "../Программа.txt";
        private string _round = string.Empty;
        private string _answer = string.Empty;
        private string[] _final;
        private int _finalcount = 1;
        private int _finalcount2 = 1;
        private readonly MediaPlayer player = new MediaPlayer();
        private int _player;
        private int _ipoints;
        private readonly int[] _counters = new int[6];
        private int _icounter;
        private readonly DispatcherTimer _timer;
        private Button button;
        private Button button1;
        private Slider slider;
        private readonly TextBox[] _counterTexts = new TextBox[6];
        private TextBox _finalText;
        private Socket _socket;
        private int _delay = 0;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["Interval1r"])) // в будущем будет возможность изменить время
        };
            _timer.Tick += (sender, e) => TimerTick();
            try
            {
                using (StreamReader streamReader = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line == "1 РАУНД" && cb.SelectedIndex == 0)
                        {
                            ReadRound(streamReader);
                        }
                        else if (line == "2 РАУНД" && cb.SelectedIndex == 1)
                        {
                            ReadRound(streamReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void InternetEnable()//object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание сокета и установка соединения с сервером
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await _socket.ConnectAsync(ConfigurationManager.AppSettings["IP"], int.Parse(ConfigurationManager.AppSettings["Ports"]));
                // Отправка сообщения на сервер
                byte[] data = Encoding.UTF8.GetBytes($"3|{DateTime.Now:dd.MM.yyyy hh:mm:ss:fff}|{superClientId}");
                await _socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);

                // Получение сообщений от сервера
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytes = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytes);
                    if (message.IndexOf("Вы подключились как суперклиент") != 0)
                    {
                        string[] parts = message.Split(':');
                        if (_player == 1 && _round != "ФИНАЛ")
                        {
                            if (_round == "2 РАУНД" && _ipoints < _delay)
                                continue;
                            else
                            {
                                player.Pause();
                                _timer.Stop();
                                _player = 0;
                                MessageBox.Show($"Отвечает: {parts[6]}");
                            }
                        }
                    }
                    else
                        MessageBox.Show("Подключение к серверу успешно");
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        public void InternetDisable()//object sender, RoutedEventArgs e)
        {
            if (_socket != null && _socket.Connected)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                    MessageBox.Show("Отключение от сервера успешно");
                }
                catch (SocketException ex)
                {
                    MessageBox.Show($"Ошибка отключения: {ex.Message}");
                }
            }
        }

        public void SetDelay(int delay)
        {
            _delay = delay;
        }

        void ReadRound(StreamReader streamReader)
        {
            topic1.Text = streamReader.ReadLine();
            topic2.Text = streamReader.ReadLine();
            topic3.Text = streamReader.ReadLine();
            topic4.Text = streamReader.ReadLine();
            topic5.Text = streamReader.ReadLine();
            topic6.Text = streamReader.ReadLine();
        }

        private void TimerTick()
        {
            switch (_round)
            {
                case "1 РАУНД":
                    if (int.TryParse(points.Text, out int pointsVal) && pointsVal > 0)
                    {
                        points.Text = (--pointsVal).ToString();
                        pgpoint.Value = pointsVal;
                    }
                    else
                    {
                        _timer.Stop();
                        player.Stop();
                        points.Text = "ВРЕМЯ ВЫШЛО!";
                        pgpoint.Value = 0;
                    }
                    break;
                case "2 РАУНД":
                    if (_ipoints < 200)
                    {
                        ++_ipoints;
                        pgpoint.Value = _ipoints;
                        points.Text = _ipoints.ToString();

                        switch (_icounter)
                        {
                            case 1: _counterTexts[0].Text = (++_counters[0]).ToString(); break;
                            case 2: _counterTexts[1].Text = (++_counters[1]).ToString(); break;
                            case 3: _counterTexts[2].Text = (++_counters[2]).ToString(); break;
                            case 4: _counterTexts[3].Text = (++_counters[3]).ToString(); break;
                            case 5: _counterTexts[4].Text = (++_counters[4]).ToString(); break;
                            case 6: _counterTexts[5].Text = (++_counters[5]).ToString(); break;
                        }
                    }
                    else
                    {
                        _timer.Stop();
                    }
                    break;
                case "ФИНАЛ":
                    if (slider.Value > 1)
                    {
                        --slider.Value;
                    }
                    else
                    {
                        slider.Value = 0;
                        _timer.Stop();
                        _player = 0;
                        player.Stop();
                    }
                    break;
                default: return;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is FrameworkElement source)) return;
            var name = source.Name;
            source.IsEnabled = false;
            if (!name.StartsWith("bt")) return;
            var index = int.Parse(name.Substring(2, 1));
            _icounter = index;
            if (!(FindName($"topic{index}") is TextBox textBox))
            {
                MessageBox.Show("Строка не определена");
                return;
            }
            //MessageBox.Show(_timer.Interval.ToString());
            MusicPlay(textBox.Text, name.Substring(3, 1));
        }

        private void MusicPlay(string I, string No)
        {
            string str1 = $"../{_round}/{I}/";
            _answer = $"../{_round}/{I}/Ответ/{No}.mp3";
            //MessageBox.Show($"{str1}{No}.mp3");
            player.Open(new Uri($"{str1}{No}.mp3", UriKind.RelativeOrAbsolute));
            player.Play();
            _player = 1;

            if (_round == "1 РАУНД")
            {
                string[] lines = File.ReadAllLines($"{str1}Баллы.txt", Encoding.UTF8);
                string str2 = lines[int.Parse(No) - 1];
                int num = str2.LastIndexOf('-');
                string str3 = str2.Substring(num + 2);
                points.Text = str3;
                pgpoint.Maximum = int.Parse(str3);
                pgpoint.Value = int.Parse(str3);
            }
            else if (_round == "2 РАУНД")
            {
                _ipoints = 0;
                _timer.Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["Interval2r"]));
                points.Text = "0";
            }
            _timer.Start();
            PrigressBarAdd();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad0 && _player == 1 && _round != "ФИНАЛ")
            {
                player.Pause();
                _timer.Stop();
                _player = 0;
            }
            else if (e.Key == Key.NumPad0 && _player == 0 && _round != "ФИНАЛ")
            {
                player.Play();
                _timer.Start();
                _player = 1;
            }
            if (e.Key == Key.A && _answer != "" && _round != "ФИНАЛ")
            {
                player.Close();
                _timer.Stop();
                player.Open(new Uri(_answer, UriKind.RelativeOrAbsolute));
                player.Play();
                _answer = "";
            }
            if (e.Key != Key.F || !(_round == "ФИНАЛ") || _player != 0)
                return;
            _finalText.Text = _final[_finalcount2-1];
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _timer.Stop();
            player.Stop();
            switch (cb.SelectedIndex)
            {
                case 0:
                    LoadRound("1 РАУНД", "images/1 Раунд.png", new string[] { "topic1", "topic2", "topic3", "topic4", "topic5", "topic6" });
                    break;
                case 1:
                    LoadRound("2 РАУНД", "images/2 Раунд.png", new string[] { "topic1", "topic2", "topic3", "topic4", "topic5", "topic6" });
                    break;
                case 2:
                    LoadFinalRound();
                    break;
            }
        }

        private void SetBackgroundImage(string imagePath)
        {
            layoutGrid.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), imagePath)))
            };
        }

        private void LoadFinalRound()
        {
            SetBackgroundImage("images/Финал.png");
            layoutGrid.Children.Clear();
            layoutGrid.ColumnDefinitions.Clear();
            vb.Margin = new Thickness(0, -50, 0, -50);
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            slider = new Slider
            {
                Maximum = 20,
                Width = 655,
                Height = 42,
                Margin = new Thickness(50, 170, 50, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TickPlacement = TickPlacement.BottomRight,
                IsSelectionRangeEnabled = true,
                IsSnapToTickEnabled = true
            };
            slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);
            button = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 80,
                Margin = new Thickness(250, 230, 0, 88),
                VerticalAlignment = VerticalAlignment.Top,
                Width = byte.MaxValue,
                FontSize = 33,
                FontFamily = new FontFamily("Book Antiqua"),
                Style = Resources["X"] as Style
            };
            button.Click += new RoutedEventHandler(Button_Click1);
            button1 = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 40,
                MaxWidth = 40,
                Margin = new Thickness(0, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = byte.MaxValue,
                FontSize = 33,
                FontFamily = new FontFamily("Book Antiqua"),
                Style = Resources["ArrowButtonStyle"] as Style
            };
            button1.Click += new RoutedEventHandler(Button_Click2);
            _finalText = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 59,
                Margin = new Thickness(50, 40, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 655,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 33,
                IsReadOnly = true,
                FontFamily = new FontFamily("Book Antiqua"),
                Background = Brushes.AntiqueWhite
            };
            layoutGrid.Children.Add(button);
            if(_finalcount>1)
            layoutGrid.Children.Add(button1);
            layoutGrid.Children.Add(slider);
            layoutGrid.Children.Add(_finalText);
            slider.Value = 20;
            _round = "ФИНАЛ";
        }

        private void LoadRound(string roundName, string imagePath, string[] topicNames)
        {
            if (_player == 1)
            {
                player.Pause();
                _timer.Stop();
                _player = 0;
                points.Text = "";
            }
            try
            {
                using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
                {
                    string str;
                    while ((str = streamReader.ReadLine()) != null)
                    {
                        switch (str)
                        {
                            case "ФИНАЛ":
                                // Получаем текущую позицию чтения
                                long currentPosition = streamReader.BaseStream.Position;
                                // Считываем оставшиеся строки в файле
                                string remainingText = streamReader.ReadToEnd();
                                // Разбиваем текст на отдельные строки
                                string[] remainingLinesArray = remainingText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                // Записываем первую строку после слова "ФИНАЛ", если она есть
                                if (remainingLinesArray.Length > 0)
                                {
                                    _finalcount = remainingLinesArray.Length;
                                    _final = new string[remainingLinesArray.Length];
                                    for (int i = 0; i < remainingLinesArray.Length; i++)
                                    {
                                        _final[i] = remainingLinesArray[i];
                                    }
                                }
                                // Выводим результат
                                //MessageBox.Show("Осталось строк после ФИНАЛ: " + remainingLinesArray.Length);
                                break;
                            case var s when s == roundName:
                                imageback.Source = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), imagePath)));
                                for (int i = 0; i < topicNames.Length; i++)
                                {
                                    var topic = FindName(topicNames[i]) as TextBox;
                                    topic.Text = streamReader.ReadLine();
                                }
                                if (roundName == "2 РАУНД")
                                {
                                    pgpoint.Maximum = 200;
                                    // Обратно включить кнопки
                                    Button[] answerButtons = {
                                        bt11, bt12, bt13, bt14, bt15,
                                        bt21, bt22, bt23, bt24, bt25,
                                        bt31, bt32, bt33, bt34, bt35,
                                        bt41, bt42, bt43, bt44, bt45,
                                        bt51, bt52, bt53, bt54, bt55,
                                        bt61, bt62, bt63, bt64, bt65
                                    };
                                    foreach (Button button in answerButtons)
                                    {
                                        button.IsEnabled = true;
                                    }
                                    // Выключить лишние кнопки
                                    Button[] buttonsToDisable = {
                                        bt16, bt17, bt18, bt26, bt27, bt28,
                                        bt36, bt37, bt38, bt46, bt47, bt48,
                                        bt56, bt57, bt58, bt66, bt67, bt68
                                    };
                                    foreach (Button button in buttonsToDisable)
                                    {
                                        button.IsEnabled = false;
                                        button.Visibility = Visibility.Collapsed;
                                    }
                                    // Создание новых текстовых полей
                                    for (int i = 0; i < _counterTexts.Length; i++)
                                    {
                                        var textBox = new TextBox
                                        {
                                            Width = 140,
                                            Height = 23,
                                            Margin = new Thickness(327, 75 + i * 45, 0, 0),
                                            VerticalAlignment = VerticalAlignment.Top,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            IsReadOnly = true,
                                            VerticalContentAlignment = VerticalAlignment.Center,
                                            TextWrapping = TextWrapping.Wrap
                                        };

                                        layoutGrid.Children.Add(textBox);
                                        Grid.SetColumn(textBox, 2);
                                        _counterTexts[i] = textBox;
                                    }
                                    // остальные переменные
                                    _round = "2 РАУНД";
                                    progress.Value = 0;
                                    progress.Maximum = 30;
                                    pgpoint.Value = 0;
                                }
                                else _round = "1 РАУНД";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"{_finalcount} > 0 && {_finalcount} < {_finalcount2}");
            if (_finalcount > 0)
            {
                player.Open(new Uri($"../final{_finalcount2}.mp3", UriKind.RelativeOrAbsolute));
                player.Play();
                _player = 1;
                _timer.Start();
            }
            else
            { 
                MessageBox.Show("Песен для финала нет");
                layoutGrid.Children.Clear();
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            _finalcount2++;
            player.Pause();
            _timer.Stop();
            _finalText.Clear();
            slider.Value = 20;
            if (_finalcount == _finalcount2)
            {
                //MessageBox.Show("Песен больше нет");
                layoutGrid.Children.Remove(button1);
            }
            _player = 0;
        }

            private void ShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(this);
            settingsWindow.ShowDialog();
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                ((Slider)sender).SelectionEnd = e.NewValue;
                button.Content = "Секунд: " + ((Slider)sender).SelectionEnd.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SetSetting()
        {
            if(_round=="1 РАУНД")
                _timer.Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["Interval1r"]));
            if (_round=="2 РАУНД")
                _timer.Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["Interval2r"]));
        }
        private void PrigressBarAdd() => ++progress.Value;
    }
}