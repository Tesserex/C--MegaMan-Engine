using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        private CustomMessageBox()
        {
            InitializeComponent();

            Image_MessageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/wily.png"));
            Image_MessageBox.Visibility = Visibility.Visible;
        }

        private MessageBoxResult result;

        public static void ShowError(string message, string caption)
        {
            var box = new CustomMessageBox();
            box.Button_OK.Visibility = Visibility.Visible;
            box.TextBlock_Message.Text = message;
            box.Title = caption;
            box.ShowDialog();
        }

        public static MessageBoxResult ShowSavePrompt()
        {
            var box = new CustomMessageBox();
            box.Button_Yes.Visibility = Visibility.Visible;
            box.Button_No.Visibility = Visibility.Visible;
            box.Button_Cancel.Visibility = Visibility.Visible;
            box.TextBlock_Message.Text = "Do you wish to save your changes?";
            box.Title = "Save Changes?";
            box.ShowDialog();
            return box.result;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            Close();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.OK;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Yes;
            Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.No;
            Close();
        }
    }
}
