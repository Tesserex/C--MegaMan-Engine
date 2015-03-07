using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using MegaManR.Audio;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class StagePropertiesViewModel : INotifyPropertyChanged
    {
        private StageDocument _stage;
        private BackgroundMusic bgm;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (_stage != null && _stage.Name != value)
                {
                    _stage.Name = value;
                }
                OnPropertyChanged("Name");
            }
        }

        private uint _track;
        public uint Track
        {
            get { return _track; }
            set
            {
                _track = value;
                if (_stage != null && _stage.MusicTrack != value)
                {
                    _stage.MusicTrack = (int)value;
                    bgm.CurrentTrack = value - 1;
                }
                OnPropertyChanged("Track");
            }
        }

        public uint MaxTrack
        {
            get;
            private set;
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public StagePropertiesViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);

            PlayCommand = new RelayCommand(Play, o => _stage != null && (!AudioManager.Instance.IsBGMPlaying || AudioManager.Instance.Paused));
            PauseCommand = new RelayCommand(Pause, o => _stage != null && AudioManager.Instance.IsBGMPlaying);
            StopCommand = new RelayCommand(Stop, o => _stage != null && (AudioManager.Instance.IsBGMPlaying || AudioManager.Instance.Paused));

            AudioManager.Instance.Initialize();
            AudioManager.Instance.Stereo = true;

            Track = 1;
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            MaxTrack = 0;

            if (e.Project.MusicNsf != null)
            {
                bgm = new BackgroundMusic(AudioContainer.LoadContainer(e.Project.MusicNsf));
                AudioManager.Instance.LoadBackgroundMusic(bgm);
                MaxTrack = bgm.AudioContainer.TrackCount;
            }

            OnPropertyChanged("MaxTrack");
        }

        private void Stop(object obj)
        {
            AudioManager.Instance.StopBGMPlayback();
            AudioManager.Instance.ResumeBGMPlayback();
        }

        private void Pause(object obj)
        {
            if (AudioManager.Instance.Paused)
                AudioManager.Instance.ResumeBGMPlayback();
            else
                AudioManager.Instance.PauseBGMPlayback();
        }

        private void Play(object obj)
        {
            bgm.CurrentTrack = (uint)Track - 1;
            AudioManager.Instance.PlayBackgroundMusic(bgm);
            AudioManager.Instance.ResumeBGMPlayback();
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            _stage = e.Stage;
            if (_stage != null)
            {
                Name = _stage.Name;
                Track = (uint)_stage.MusicTrack;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
