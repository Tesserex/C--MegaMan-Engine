using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Factories;
using MegaMan.Editor.Mediator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.IO;
using MegaMan.Editor.Services;
using System.Windows.Input;
using System.Resources;
using System.Collections;
using System.IO;
using System.Reflection;

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

        private IProjectDocumentFactory _projectFactory;
        private IDataAccessService _dataService;

        public ICommand CreateCommand { get; private set; }

        public NewProjectViewModel(IProjectDocumentFactory projectFactory, IDataAccessService dataService)
        {
            _projectFactory = projectFactory;
            _dataService = dataService;

            CreateCommand = new RelayCommand(Create);

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

        public void Create(object param)
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

            var document = _projectFactory.CreateNew(path);

            document.Name = Name;
            document.Author = Author;
            _dataService.SaveProject(document);
            
            var resPath = "resources/includes";
            var includes = GetResourcesUnder(resPath);
            foreach (var resName in includes)
            {
                var filePath = Path.Combine(path, resName.Substring(resPath.Length + 1)); // remove final slash
                WriteResourceToFile(resName, filePath);
            }

            var args = new ProjectOpenedEventArgs() { Project = document };
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Raise(this, args);
        }

        private static string[] GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly       = Assembly.GetCallingAssembly();
            var resourcesName  = assembly.GetName().Name + ".g.resources";
            var stream         = assembly.GetManifestResourceStream(resourcesName);
            var resourceReader = new ResourceReader(stream);

            var resources = resourceReader.OfType<DictionaryEntry>()
                .Select(p => p.Key.ToString())
                .Where(theme => theme.StartsWith(folder));

            return resources.ToArray();
        }

        private static void WriteResourceToFile(string resname, string path)
        {
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists) 
            {
                Directory.CreateDirectory(fi.DirectoryName); 
            } 

            Stream res = System.Windows.Application.GetResourceStream(new Uri(resname, UriKind.Relative)).Stream;
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = res.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
