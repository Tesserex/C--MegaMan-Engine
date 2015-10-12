using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Editor.Controls.Dialogs;
using MegaMan.Editor.Controls.ViewModels.Dialogs;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Services
{
    public class DialogService : IDialogService
    {
        public void ShowNewEntityDialog()
        {
            var dialog = new NewEntityDialog();
            dialog.ShowDialog();

            if (dialog.Result == System.Windows.MessageBoxResult.OK)
            {
                var vm = (NewEntityDialogViewModel)dialog.DataContext;
                ViewModelMediator.Current.GetEvent<NewEntityEventArgs>().Raise(this, new NewEntityEventArgs() { Name = vm.Name });
            }
        }
    }
}
