using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ProjectTree.xaml
    /// </summary>
    public partial class ProjectTree : UserControl
    {
        private ProjectViewModel _project;

        public ProjectTree()
        {
            InitializeComponent();
        }

        public void Update(MegaMan.Common.Project project)
        {
            _project = new ProjectViewModel(project);

            base.DataContext = _project;
        }
    }
}
