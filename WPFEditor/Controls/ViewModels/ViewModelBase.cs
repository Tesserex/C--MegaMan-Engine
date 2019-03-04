using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MegaMan.Editor.Controls.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
