using System.Windows;

namespace MegaMan.Editor.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for TilesetImageImportDialog.xaml
    /// </summary>
    public partial class TilesetImageImportDialog : Window
    {
        public TilesetImageImportDialog()
        {
            InitializeComponent();
        }

        public MessageBoxResult Result { get; private set; }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
    }
}
