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
            IncreaseVolumeCommand = new RelayCommand(() => Engine.Instance.SoundSystem.Volume++);
            DecreaseVolumeCommand = new RelayCommand(() => Engine.Instance.SoundSystem.Volume--);
        }

        public void LoadSettings(Setting settings)
        {
            MusicEnabled = settings.Audio.Musics;
            SfxEnabled = settings.Audio.Sound;
            SquareOneEnabled = settings.Audio.Square1;
            SquareTwoEnabled = settings.Audio.Square2;
            TriangleEnabled = settings.Audio.Triangle;
            NoiseEnabled = settings.Audio.Noise;
            Engine.Instance.SoundSystem.Volume = settings.Audio.Volume;
        }

        public void SaveSettings(Setting settings)
        {
            settings.Audio.Musics = MusicEnabled;
            settings.Audio.Sound = SfxEnabled;
            settings.Audio.Square1 = SquareOneEnabled;
            settings.Audio.Square2 = SquareTwoEnabled;
            settings.Audio.Triangle = TriangleEnabled;
            settings.Audio.Noise = NoiseEnabled;
            settings.Audio.Volume = Engine.Instance.SoundSystem.Volume;
        }

        public bool MusicEnabled
        {
            get => Engine.Instance.SoundSystem.MusicEnabled;
            set { Engine.Instance.SoundSystem.MusicEnabled = value; OnPropertyChanged(); }
        }

        public bool SfxEnabled
        {
            get => Engine.Instance.SoundSystem.SfxEnabled;
            set { Engine.Instance.SoundSystem.SfxEnabled = value; OnPropertyChanged(); }
        }

        public bool SquareOneEnabled
        {
            get => Engine.Instance.SoundSystem.SquareOne;
            set { Engine.Instance.SoundSystem.SquareOne = value; OnPropertyChanged(); }
        }

        public bool SquareTwoEnabled
        {
            get => Engine.Instance.SoundSystem.SquareTwo;
            set { Engine.Instance.SoundSystem.SquareTwo = value; OnPropertyChanged(); }
        }

        public bool TriangleEnabled
        {
            get => Engine.Instance.SoundSystem.Triangle;
            set { Engine.Instance.SoundSystem.Triangle = value; OnPropertyChanged(); }
        }

        public bool NoiseEnabled
        {
            get => Engine.Instance.SoundSystem.Noise;
            set { Engine.Instance.SoundSystem.Noise = value; OnPropertyChanged(); }
        }
    }
}
