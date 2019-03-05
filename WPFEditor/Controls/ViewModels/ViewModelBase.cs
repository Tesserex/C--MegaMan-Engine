using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MegaMan.Editor.Controls.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
