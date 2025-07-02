using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Audio;
using MegaMan.Editor.Bll.Audio.CSCore;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class StagePropertiesViewModel : INotifyPropertyChanged
    {
        private StageDocument _stage;
        private BetterSoundSystem soundsystem = new BetterSoundSystem();
        private MusicNsf bgm;

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

        private int _track;
        public int Track
        {
            get { return _track; }
            set
            {
                _track = value;
                if (soundsystem != null)
                {
                    var playing = bgm?.IsPlaying == true;
                    bgm?.Stop();
                    bgm = soundsystem.LoadMusicNsf(value) as MusicNsf;
                    if (playing) bgm?.Play();
                }
                
                if (_stage != null && _stage.MusicTrack != value)
                {
                    _stage.MusicTrack = value;
                }
                OnPropertyChanged("Track");
            }
        }

        public int MaxTrack
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

            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);

            PlayCommand = new RelayCommand(Play, o => bgm != null && (!bgm.IsPlaying));
            PauseCommand = new RelayCommand(Pause, o => bgm?.IsPlaying == true);
            StopCommand = new RelayCommand(Stop, o => bgm?.IsPlaying == true);

            Track = 1;
        }

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            MaxTrack = 0;

            if (e.Project != null)
            {
                if (e.Project.MusicNsf != null)
                {
                    soundsystem.LoadNSF(e.Project.MusicNsf);

                    bgm = soundsystem.LoadMusicNsf(Track) as MusicNsf;
                    soundsystem.Start();
                    MaxTrack = soundsystem.NsfTracks;
                }
            }
            else
            {
                bgm = null;
                soundsystem.StopMusicNsf();
            }

            OnPropertyChanged("MaxTrack");
        }

        private void Stop(object obj)
        {
            bgm?.Stop();
        }

        private void Pause(object obj)
        {
            if (bgm?.IsPlaying == true)
                bgm?.Stop();
            else
                bgm?.Play();
        }

        private void Play(object obj)
        {
            bgm?.Play();
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            _stage = e.Stage;
            if (_stage != null)
            {
                Name = _stage.Name;
                Track = _stage.MusicTrack;
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
