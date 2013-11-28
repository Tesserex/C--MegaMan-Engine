using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class NewProjectViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged("Author");
            }
        }

        private string _directoryPath;
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                _directoryPath = value;
                OnPropertyChanged("DirectoryPath");
            }
        }

        private bool _createDirectory;
        public bool CreateProjectDirectory
        {
            get { return _createDirectory; }
            set
            {
                _createDirectory = value;
                OnPropertyChanged("CreateProjectDirectory");
            }
        }

        public NewProjectViewModel()
        {
            Name = GetDefaultProjectName();
            Author = GetMostRecentAuthor();
            DirectoryPath = GetMostRecentDirectory();
            CreateProjectDirectory = true;
        }

        private string GetMostRecentDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private string GetMostRecentAuthor()
        {
            return "";
        }

        private string GetDefaultProjectName()
        {
            return "My Fan Game";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Create()
        {
            var path = DirectoryPath;
            if (CreateProjectDirectory)
            {
                var invalidChars = System.IO.Path.GetInvalidPathChars();
                var nameFolder = new String(Name
                    .Where(x => !invalidChars.Contains(x))
                    .ToArray());
                path = System.IO.Path.Combine(path, nameFolder);
            }

            var document = ProjectDocument.CreateNew(path);

            document.Name = Name;
            document.Author = Author;
            document.Save();

            var args = new ProjectOpenedEventArgs() { Project = document };
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Raise(this, args);
        }
    }
}
