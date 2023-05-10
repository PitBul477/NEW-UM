using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace NEW_UM
{
    public partial class MainWindow : Window
    {
        private readonly static string superClientId = "SuperClient";
        private readonly string path = "../Программа.txt";
        private string _round = string.Empty;
        private string _answer = string.Empty;
        private string _final = string.Empty;
        private readonly MediaPlayer player = new MediaPlayer();
        private int _player;
        private int _ipoints;
        private readonly int[] _counters = new int[6];
        private int _icounter;
        private readonly DispatcherTimer _timer;
        private Button button;
        private Slider slider;
        private readonly TextBox[] _counterTexts = new TextBox[6];
        private TextBox _finalText;
        //Path = System.IO.Path;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000) // в будущем будет возможность изменить время
            };
            _timer.Tick += (sender, e) => TimerTick();
            ConnectToServer();
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

        void ReadRound(StreamReader streamReader)
        {
            first.Text = streamReader.ReadLine();
            second.Text = streamReader.ReadLine();
            third.Text = streamReader.ReadLine();
            fourth.Text = streamReader.ReadLine();
            fifth.Text = streamReader.ReadLine();
            sixth.Text = streamReader.ReadLine();
        }
        private async void ConnectToServer()
        {
            try
            {
                // Создание сокета и установка соединения с сервером
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync("92.124.142.200", 12345);

                // Отправка сообщения на сервер
                byte[] data = Encoding.UTF8.GetBytes($"3|{DateTime.Now:dd.MM.yyyy hh:mm:ss:fff}|{superClientId}");
                await socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);

                // Получение сообщений от сервера
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytes = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytes);
                    if (message.IndexOf("Вы подключились как суперклиент") != 0)
                    {
                        string[] parts = message.Split(':');
                        if (_player == 1 && _round != "ФИНАЛ")
                        {
                            player.Pause();
                            _timer.Stop();
                            _player = 0;
                            MessageBox.Show($"Отвечает: {parts[6]}");
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
            string name = ((FrameworkElement)e.OriginalSource).Name;
            ((UIElement)e.OriginalSource).IsEnabled = false;
            if (name.IndexOf("bt") != 0)
                return;
            string str = name.Substring(2);
            char ch = str[0];
            _icounter = int.Parse(ch.ToString());
            ch = str[0];
            switch (ch)
            {
                case '1':
                    MusicPlay(first.Text, str[1].ToString());
                    break;
                case '2':
                    MusicPlay(second.Text, str[1].ToString());
                    break;
                case '3':
                    MusicPlay(third.Text, str[1].ToString());
                    break;
                case '4':
                    MusicPlay(fourth.Text, str[1].ToString());
                    break;
                case '5':
                    MusicPlay(fifth.Text, str[1].ToString());
                    break;
                case '6':
                    MusicPlay(sixth.Text, str[1].ToString());
                    break;
                default:
                    MessageBox.Show("Строка не определена");
                    break;
            }
        }

        private void MusicPlay(string I, string No)
        {
            string str1 = "../" + _round + "/" + I + "/";
            _answer = "../" + _round + "/" + I + "/Ответ/" + No + ".mp3";
            player.Open(new Uri(str1 + No + ".mp3", UriKind.RelativeOrAbsolute));
            player.Play();
            _player = 1;
            if (_round == "1 РАУНД")
            {
                using (StreamReader streamReader = new StreamReader(str1 + "Баллы.txt", Encoding.UTF8))
                {
                    string str2 = "";
                    for (int index = 0; index < Convert.ToInt32(No); ++index)
                        str2 = streamReader.ReadLine();
                    int num = str2.LastIndexOf('-');
                    string str3 = str2.Remove(0, num + 2);
                    points.Text = str3;
                    pgpoint.Maximum = Convert.ToInt32(str3);
                    pgpoint.Value = Convert.ToInt32(str3);
                }
            }
            else if (_round == "2 РАУНД")
            {
                _ipoints = 0;
                _timer.Interval = TimeSpan.FromMilliseconds(200);
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
            _finalText.Text = _final;
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        if (str == "1 РАУНД" && cb.SelectedIndex == 0)
                        {
                            imageback.Source = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "images/1 Раунд.png")));
                            first.Text = streamReader.ReadLine();
                            second.Text = streamReader.ReadLine();
                            third.Text = streamReader.ReadLine();
                            fourth.Text = streamReader.ReadLine();
                            fifth.Text = streamReader.ReadLine();
                            sixth.Text = streamReader.ReadLine();
                            _round = "1 РАУНД";
                            break;
                        }
                        if (str == "2 РАУНД" && cb.SelectedIndex == 1)
                        {
                            imageback.Source = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "images/2 Раунд.png")));
                            pgpoint.Maximum = 200;
                            first.Text = streamReader.ReadLine();
                            second.Text = streamReader.ReadLine();
                            third.Text = streamReader.ReadLine();
                            fourth.Text = streamReader.ReadLine();
                            fifth.Text = streamReader.ReadLine();
                            sixth.Text = streamReader.ReadLine();
                            bt11.IsEnabled = true;
                            bt12.IsEnabled = true;
                            bt13.IsEnabled = true;
                            bt14.IsEnabled = true;
                            bt15.IsEnabled = true;
                            bt21.IsEnabled = true;
                            bt22.IsEnabled = true;
                            bt23.IsEnabled = true;
                            bt24.IsEnabled = true;
                            bt25.IsEnabled = true;
                            bt31.IsEnabled = true;
                            bt32.IsEnabled = true;
                            bt33.IsEnabled = true;
                            bt34.IsEnabled = true;
                            bt35.IsEnabled = true;
                            bt41.IsEnabled = true;
                            bt42.IsEnabled = true;
                            bt43.IsEnabled = true;
                            bt44.IsEnabled = true;
                            bt45.IsEnabled = true;
                            bt51.IsEnabled = true;
                            bt52.IsEnabled = true;
                            bt53.IsEnabled = true;
                            bt54.IsEnabled = true;
                            bt55.IsEnabled = true;
                            bt61.IsEnabled = true;
                            bt62.IsEnabled = true;
                            bt63.IsEnabled = true;
                            bt64.IsEnabled = true;
                            bt65.IsEnabled = true;
                            bt16.IsEnabled = false;
                            bt16.Visibility = Visibility.Collapsed;
                            bt17.IsEnabled = false;
                            bt17.Visibility = Visibility.Collapsed;
                            bt18.IsEnabled = false;
                            bt18.Visibility = Visibility.Collapsed;
                            bt26.IsEnabled = false;
                            bt26.Visibility = Visibility.Collapsed;
                            bt27.IsEnabled = false;
                            bt27.Visibility = Visibility.Collapsed;
                            bt28.IsEnabled = false;
                            bt28.Visibility = Visibility.Collapsed;
                            bt36.IsEnabled = false;
                            bt36.Visibility = Visibility.Collapsed;
                            bt37.IsEnabled = false;
                            bt37.Visibility = Visibility.Collapsed;
                            bt38.IsEnabled = false;
                            bt38.Visibility = Visibility.Collapsed;
                            bt46.IsEnabled = false;
                            bt46.Visibility = Visibility.Collapsed;
                            bt47.IsEnabled = false;
                            bt47.Visibility = Visibility.Collapsed;
                            bt48.IsEnabled = false;
                            bt48.Visibility = Visibility.Collapsed;
                            bt56.IsEnabled = false;
                            bt56.Visibility = Visibility.Collapsed;
                            bt57.IsEnabled = false;
                            bt57.Visibility = Visibility.Collapsed;
                            bt58.IsEnabled = false;
                            bt58.Visibility = Visibility.Collapsed;
                            bt66.IsEnabled = false;
                            bt66.Visibility = Visibility.Collapsed;
                            bt67.IsEnabled = false;
                            bt67.Visibility = Visibility.Collapsed;
                            bt68.IsEnabled = false;
                            bt68.Visibility = Visibility.Collapsed;
                            _counterTexts[0] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 75, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            _counterTexts[1] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 120, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            _counterTexts[2] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 165, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            _counterTexts[3] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 210, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            _counterTexts[4] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 255, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            _counterTexts[5] = new TextBox
                            {
                                Width = 140,
                                Height = 23,
                                Margin = new Thickness(560, 300, 0, 0),
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                IsReadOnly = true,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap
                            };
                            layoutGrid.Children.Add(_counterTexts[0]);
                            layoutGrid.Children.Add(_counterTexts[1]);
                            layoutGrid.Children.Add(_counterTexts[2]);
                            layoutGrid.Children.Add(_counterTexts[3]);
                            layoutGrid.Children.Add(_counterTexts[4]);
                            layoutGrid.Children.Add(_counterTexts[5]);
                            _round = "2 РАУНД";
                            progress.Value = 0;
                            progress.Maximum = 30;
                            break;
                        }
                        if (str == "ФИНАЛ")
                            _final = streamReader.ReadLine();
                    }
                    if (cb.SelectedIndex != 2)
                        return;
                    layoutGrid.Background = new ImageBrush()
                    {
                        ImageSource = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "images/Финал.png")))
                    };
                    layoutGrid.Children.Clear();
                    _timer.Interval = TimeSpan.FromMilliseconds(1000);
                    slider = new Slider
                    {
                        Maximum = 20,
                        Width = 655,
                        Height = 42,
                        Margin = new Thickness(50, 150, 0, 0),
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
                        Margin = new Thickness(250, 230, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = byte.MaxValue,
                        FontSize = 33,
                        FontFamily = new FontFamily("Book Antiqua"),
                        Style = Resources["X"] as Style
                    };
                    button.Click += new RoutedEventHandler(Button_Click1);
                    _finalText = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Height = 59,
                        Margin = new Thickness(50, 27, 0, 0),
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 655,
                        TextAlignment = TextAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 33,
                        FontFamily = new FontFamily("Book Antiqua"),
                        Background = Brushes.AntiqueWhite
                    };
                    layoutGrid.Children.Add(button);
                    layoutGrid.Children.Add(slider);
                    layoutGrid.Children.Add(_finalText);
                    slider.Value = 20;
                    _round = "ФИНАЛ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*private void UpdateRoundData(string[] lines, int startIndex)
        {
            string[] roundData = lines.Skip(startIndex).Take(6).ToArray();

            first.Text = roundData[0];
            second.Text = roundData[1];
            third.Text = roundData[2];
            fourth.Text = roundData[3];
            fifth.Text = roundData[4];
            sixth.Text = roundData[5];
        }*/

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            player.Open(new Uri("../final.mp3", UriKind.RelativeOrAbsolute));
            player.Play();
            _player = 1;
            _timer.Start();
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

        private void PrigressBarAdd() => ++progress.Value;
    }
}
