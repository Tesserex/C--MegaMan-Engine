using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus
{
    internal class AudioMenuViewModel : ViewModelBase, IMenuViewModel
    {
        public ICommand ToggleMusicCommand { get; }
        public ICommand ToggleSfxCommand { get; }
        public ICommand ToggleSquareOneCommand { get; }
        public ICommand ToggleSquareTwoCommand { get; }
        public ICommand ToggleTriangleCommand { get; }
        public ICommand ToggleNoiseCommand { get; }
        public ICommand IncreaseVolumeCommand { get; }
        public ICommand DecreaseVolumeCommand { get; }

        public AudioMenuViewModel()
        {
            ToggleMusicCommand = new RelayCommand(() => MusicEnabled = !MusicEnabled);
            ToggleSfxCommand = new RelayCommand(() => SfxEnabled = !SfxEnabled);
            ToggleSquareOneCommand = new RelayCommand(() => SquareOneEnabled = !SquareOneEnabled);
            ToggleSquareTwoCommand = new RelayCommand(() => SquareTwoEnabled = !SquareTwoEnabled);
            ToggleTriangleCommand = new RelayCommand(() => TriangleEnabled = !TriangleEnabled);
            ToggleNoiseCommand = new RelayCommand(() => NoiseEnabled = !NoiseEnabled);
            IncreaseVolumeCommand = new RelayCommand(() => Core.Engine.Instance.SoundSystem.Volume++);
            DecreaseVolumeCommand = new RelayCommand(() => Core.Engine.Instance.SoundSystem.Volume--);
        }

        public void LoadSettings(Setting settings)
        {
            MusicEnabled = settings.Audio.Musics;
            SfxEnabled = settings.Audio.Sound;
            SquareOneEnabled = settings.Audio.Square1;
            SquareTwoEnabled = settings.Audio.Square2;
            TriangleEnabled = settings.Audio.Triangle;
            NoiseEnabled = settings.Audio.Noise;
            Core.Engine.Instance.SoundSystem.Volume = settings.Audio.Volume;
        }

        public void SaveSettings(Setting settings)
        {
            settings.Audio.Musics = MusicEnabled;
            settings.Audio.Sound = SfxEnabled;
            settings.Audio.Square1 = SquareOneEnabled;
            settings.Audio.Square2 = SquareTwoEnabled;
            settings.Audio.Triangle = TriangleEnabled;
            settings.Audio.Noise = NoiseEnabled;
            settings.Audio.Volume = Core.Engine.Instance.SoundSystem.Volume;
        }

        public bool MusicEnabled
        {
            get => Core.Engine.Instance.SoundSystem.MusicEnabled;
            set { Core.Engine.Instance.SoundSystem.MusicEnabled = value; OnPropertyChanged(); }
        }

        public bool SfxEnabled
        {
            get => Core.Engine.Instance.SoundSystem.SfxEnabled;
            set { Core.Engine.Instance.SoundSystem.SfxEnabled = value; OnPropertyChanged(); }
        }

        public bool SquareOneEnabled
        {
            get => Core.Engine.Instance.SoundSystem.SquareOne;
            set { Core.Engine.Instance.SoundSystem.SquareOne = value; OnPropertyChanged(); }
        }

        public bool SquareTwoEnabled
        {
            get => Core.Engine.Instance.SoundSystem.SquareTwo;
            set { Core.Engine.Instance.SoundSystem.SquareTwo = value; OnPropertyChanged(); }
        }

        public bool TriangleEnabled
        {
            get => Core.Engine.Instance.SoundSystem.Triangle;
            set { Core.Engine.Instance.SoundSystem.Triangle = value; OnPropertyChanged(); }
        }

        public bool NoiseEnabled
        {
            get => Core.Engine.Instance.SoundSystem.Noise;
            set { Core.Engine.Instance.SoundSystem.Noise = value; OnPropertyChanged(); }
        }
    }
}
