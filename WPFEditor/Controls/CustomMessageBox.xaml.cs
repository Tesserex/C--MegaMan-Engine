using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        public static void ShowError(string message, string caption)
        {
            var box = new CustomMessageBox();
            box.Button_Cancel.Visibility = Visibility.Collapsed;
            box.Button_OK.Visibility = Visibility.Visible;
            box.TextBlock_Message.Text = message;
            box.Title = caption;
            box.Image_MessageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/wily.png"));
            box.Image_MessageBox.Visibility = Visibility.Visible;
            box.ShowDialog();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
