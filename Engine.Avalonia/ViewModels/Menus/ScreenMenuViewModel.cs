using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus
{
    internal class ScreenMenuViewModel : ViewModelBase, IMenuViewModel
    {
        private int gameWidth, gameHeight;
        private ScreenScale scale;
        private snes_ntsc_setup_t ntscSetup = snes_ntsc_setup_t.snes_ntsc_composite;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size GameSize { get => new Size(gameWidth, gameHeight); }

        public ICommand Screen1XCommand { get; }
        public ICommand Screen2XCommand { get; }
        public ICommand Screen3XCommand { get; }
        public ICommand Screen4XCommand { get; }
        public ICommand NtscCommand { get; }
        public ICommand NtscCompositeCommand { get; }
        public ICommand NtscSVideoCommand { get; }
        public ICommand NtscRGBCommand { get; }

        public bool Is1X { get => scale == ScreenScale.X1; }
        public bool Is2X { get => scale == ScreenScale.X2; }
        public bool Is3X { get => scale == ScreenScale.X3; }
        public bool Is4X { get => scale == ScreenScale.X4; }
        public bool IsNTSC { get => scale == ScreenScale.NTSC; }
        public snes_ntsc_setup_t NTSCSetup
        {
            get => ntscSetup;
            private set { SetProperty(ref ntscSetup, value); Scale(ScreenScale.NTSC); }
        }
        public bool IsFullscreen { get => scale == ScreenScale.Fullscreen; }

        public ScreenMenuViewModel()
        {
            Screen1XCommand = new RelayCommand(() => Scale(ScreenScale.X1));
            Screen2XCommand = new RelayCommand(() => Scale(ScreenScale.X2));
            Screen3XCommand = new RelayCommand(() => Scale(ScreenScale.X3));
            Screen4XCommand = new RelayCommand(() => Scale(ScreenScale.X4));
            NtscCommand = new RelayCommand(() => Scale(ScreenScale.NTSC));
            NtscCompositeCommand = new RelayCommand(() => { NTSCSetup = snes_ntsc_setup_t.snes_ntsc_composite; });
            NtscSVideoCommand = new RelayCommand(() => { NTSCSetup = snes_ntsc_setup_t.snes_ntsc_svideo; });
            NtscRGBCommand = new RelayCommand(() => { NTSCSetup = snes_ntsc_setup_t.snes_ntsc_rgb; });

            Game.ScreenSizeChanged += Game_ScreenSizeChanged;

            gameWidth = Const.PixelsAcross;
            gameHeight = Const.PixelsDown;
            Scale(ScreenScale.X1);
        }

        public void LoadSettings(Setting settings)
        {
            scale = settings.Screens.Size;
        }

        public void SaveSettings(Setting settings)
        {
            if (settings.Screens == null)
                settings.Screens = new LastScreen();

            settings.Screens.Size = scale;
        }

        private void Scale(ScreenScale scale)
        {
            this.scale = scale;

            if (scale == ScreenScale.NTSC)
            {
                Width = 602;
                Height = 448;
            }
            else
            {
                int factor = 1;
                if (scale == ScreenScale.X2)
                {
                    factor = 2;
                }
                else if (scale == ScreenScale.X3)
                {
                    factor = 3;
                }
                else if (scale == ScreenScale.X4)
                {
                    factor = 4;
                }

                Width = gameWidth * factor;
                Height = gameHeight * factor;
            }
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(Is1X));
            OnPropertyChanged(nameof(Is2X));
            OnPropertyChanged(nameof(Is3X));
            OnPropertyChanged(nameof(Is4X));
            OnPropertyChanged(nameof(IsNTSC));
        }

        void Game_ScreenSizeChanged(object? sender, ScreenSizeChangedEventArgs e)
        {
            gameWidth = e.PixelsAcross;
            gameHeight = e.PixelsDown;

            OnPropertyChanged(nameof(GameSize));
        }
    }
}
