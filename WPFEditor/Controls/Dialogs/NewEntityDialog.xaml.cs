using System.ComponentModel;
using System.Windows;
using MegaMan.Editor.Controls.ViewModels.Dialogs;
using Ninject;

namespace MegaMan.Editor.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for NewEntityDialog.xaml
    /// </summary>
    public partial class NewEntityDialog : Window
    {
        public MessageBoxResult Result
        {
            get; private set;
        }

        public NewEntityDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = App.Container.Get<NewEntityDialogViewModel>();
            }
        }

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
