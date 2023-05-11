using NEW_UM.Properties;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace NEW_UM
{
    public partial class SettingsWindow : Window
    {
        private Settings settings;
        private MainWindow mainWindow;

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            //this.settings = settings;
            //DataContext = this.settings;
            this.mainWindow = mainWindow;
            setting1.Text = mainWindow.GetSetting().ToString();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(setting1.Text, out int pointsVal) && pointsVal > 0)
            mainWindow.SetSetting(pointsVal);
        }
    }
}
