using System;
using System.Diagnostics;
using System.Windows;
using MegaMan.Editor.Controls.ViewModels.Dialogs;

namespace MegaMan.Editor.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public static ProgressDialog Open(Progress<ProgressDialogState> reporter, Stopwatch stopwatch = null)
        {
            var box = new ProgressDialog();
            var vm = new ProgressDialogViewModel(box, reporter, stopwatch);
            box.DataContext = vm;
            return box;
        }
    }
}
