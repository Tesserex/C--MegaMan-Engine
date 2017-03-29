using System.Windows;

namespace MegaMan.Editor.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for DuplicateObjectsDialog.xaml
    /// </summary>
    public partial class DuplicateObjectsDialog : Window
    {
        public DuplicateObjectsDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
