using System.ComponentModel;
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

        public string Name
        {
            get { return _stage != null ? _stage.Name : null; }
            set
            {
                if (_stage != null)
                {
                    _stage.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public int Track
        {
            get { return _stage != null ? _stage.MusicTrack : 0; }
            set
            {
                if (_stage != null)
                {
                    _stage.MusicTrack = value;
                    OnPropertyChanged("Track");
                }
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
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);

            PlayCommand = new RelayCommand(Play, o => _stage != null);
            PauseCommand = new RelayCommand(Pause, o => _stage != null);
            StopCommand = new RelayCommand(Stop, o => _stage != null);

            AudioManager.Instance.Initialize();
            AudioManager.Instance.Stereo = true;
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            bgm = new BackgroundMusic(AudioContainer.LoadContainer(e.Project.MusicNsf));
            AudioManager.Instance.LoadBackgroundMusic(bgm);
            MaxTrack = bgm.AudioContainer.TrackCount;
            OnPropertyChanged("MaxTrack");
        }

        private void Stop(object obj)
        {
            AudioManager.Instance.StopBGMPlayback();
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
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            _stage = e.Stage;
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
    }
}
