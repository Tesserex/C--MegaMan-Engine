using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Services;

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
        
        private IDataAccessService _dataService;

        public ICommand CreateCommand { get; private set; }

        public NewProjectViewModel(IDataAccessService dataService)
        {
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
            var fullProjectPath = DirectoryPath;
            if (CreateProjectDirectory)
            {
                var invalidChars = Path.GetInvalidPathChars();
                var nameFolder = new String(Name
                    .Where(x => !invalidChars.Contains(x))
                    .ToArray());
                fullProjectPath = Path.Combine(fullProjectPath, nameFolder);
            }

            var document = _dataService.CreateProject(fullProjectPath);

            document.Name = Name;
            document.Author = Author;
            _dataService.SaveProject(document);
            
            var resBasePath = "resources/includes";
            var includes = GetResourcesUnder(resBasePath).ToDictionary(n => n, n => n.Substring(resBasePath.Length + 1));
            foreach (var p in includes)
            {
                var resPath = p.Value;
                var filePath = Path.Combine(fullProjectPath, resPath);
                var stream = Application.GetResourceStream(new Uri(p.Key, UriKind.Relative)).Stream;
                WriteResourceToFile(filePath, stream);
            }

            var embeddedKey = "Resources.Includes.";
            var embeddedIncludes = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(n => n.Contains(embeddedKey))
                .ToDictionary(n => n, n => EmbedPathToFilePath(n, embeddedKey));

            foreach (var p in embeddedIncludes)
            {
                var filePath = Path.Combine(fullProjectPath, p.Value);
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(p.Key);
                WriteResourceToFile(filePath, stream);
            }

            var allPaths = includes.Values.Concat(embeddedIncludes.Values);
            var folders = allPaths.Select(p => p.Split('/')[0]).Distinct();
            document.Project.AddIncludeFolders(folders);
            document.MusicNsf = document.EffectsNsf = fullProjectPath + "/sound/mm5.nsf";
            _dataService.SaveProject(document);

            var args = new ProjectChangedEventArgs { Project = document };
            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Raise(this, args);
        }

        private string EmbedPathToFilePath(string embedPath, string key)
        {
            var p = embedPath.Substring(embedPath.IndexOf(key) + key.Length);
            var lastDot = p.LastIndexOf('.');
            return p.Substring(0, lastDot).Replace('.', '/') + p.Substring(lastDot);
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

        private static void WriteResourceToFile(string path, Stream stream)
        {
            FileInfo fi = new FileInfo(path);
            if (!fi.Directory.Exists) 
            {
                Directory.CreateDirectory(fi.DirectoryName); 
            }
            
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
