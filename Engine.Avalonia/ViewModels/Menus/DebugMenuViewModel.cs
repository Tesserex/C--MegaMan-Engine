using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus;
internal class DebugMenuViewModel : ViewModelBase, IMenuViewModel
{
    private bool showDebugBar;

    private string? fpsLabel, thinkLabel, entityLabel;

    public bool IsDebug { get; }

    public bool ShowDebugBar
    {
        get => showDebugBar;
        set { showDebugBar = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowDebugBarHeader)); }
    }

    public string ShowDebugBarHeader { get => ShowDebugBar ? "Hide Debug Bar" : "Show Debug Bar"; }

    public bool ShowHitboxes
    {
        get => Engine.Instance.DrawHitboxes;
        set { Engine.Instance.DrawHitboxes = value; OnPropertyChanged(); }
    }

    public bool Invincibility
    {
        get => Engine.Instance.Invincible;
        set { Engine.Instance.Invincible = value; OnPropertyChanged(); }
    }

    public bool NoDamage
    {
        get => Engine.Instance.NoDamage;
        set { Engine.Instance.NoDamage = value; OnPropertyChanged(); }
    }

    public bool GravityFlip
    {
        get => Game.CurrentGame?.GetFlipGravity() ?? false;
        set { Game.CurrentGame?.DebugFlipGravity(); OnPropertyChanged(); }
    }

    public string FpsLabel { get => fpsLabel ?? ""; set { SetProperty(ref fpsLabel, value); } }
    public string ThinkLabel { get => thinkLabel ?? ""; set { SetProperty(ref thinkLabel, value); } }
    public string EntityLabel { get => entityLabel ?? ""; set { SetProperty(ref entityLabel, value); } }

    public ICommand ShowDebugBarCommand { get; }
    public ICommand ShowHitboxesCommand { get; }
    public ICommand NoDamageCommand { get; }
    public ICommand InvincibilityCommand { get; }
    public ICommand GravityFlipCommand { get; }
    public ICommand KillCommand { get; }
    public ICommand FillHealthCommand { get; }
    public ICommand EmptyWeaponCommand { get; }
    public ICommand FillWeaponCommand { get; }
    public ICommand FramerateUpCommand { get; }
    public ICommand FramerateDownCommand { get; }
    public ICommand DefaultFramerateCommand { get; }


    public DebugMenuViewModel()
    {
        ShowDebugBarCommand = new RelayCommand(() => { ShowDebugBar = !ShowDebugBar; });
        ShowHitboxesCommand = new RelayCommand(() => { ShowHitboxes = !ShowHitboxes; });
        NoDamageCommand = new RelayCommand(() => { NoDamage = !NoDamage; });
        InvincibilityCommand = new RelayCommand(() => { Invincibility = !Invincibility; });
        GravityFlipCommand = new RelayCommand(() => { GravityFlip = !GravityFlip; });

        KillCommand = new RelayCommand(() => { Game.CurrentGame?.DebugEmptyHealth(); }, () => Game.CurrentGame is not null);
        FillHealthCommand = new RelayCommand(() => { Game.CurrentGame?.DebugFillHealth(); }, () => Game.CurrentGame is not null);
        EmptyWeaponCommand = new RelayCommand(() => { Game.CurrentGame?.DebugEmptyWeapon(); }, () => Game.CurrentGame is not null);
        FillWeaponCommand = new RelayCommand(() => { Game.CurrentGame?.DebugFillWeapon(); }, () => Game.CurrentGame is not null);

        FramerateUpCommand = new RelayCommand(() => { Engine.Instance.FPS = Engine.Instance.FPS + 10; });
        FramerateDownCommand = new RelayCommand(() => { Engine.Instance.FPS = Engine.Instance.FPS - 10; });
        DefaultFramerateCommand = new RelayCommand(() => { Engine.Instance.FPS = UserSettings.Default.Debug.Framerate; });

#if DEBUG
        IsDebug = true;
        ShowDebugBar = true;
#endif

        Engine.Instance.GameLogicTick += Instance_GameLogicTick;
    }

    public void LoadSettings(Setting settings)
    {
#if DEBUG
        ShowDebugBar = settings.Debug.ShowMenu;
        ShowHitboxes = settings.Debug.ShowHitboxes;
        Engine.Instance.FPS = settings.Debug.Framerate;

        #region Cheats
        Invincibility = settings.Debug.Cheat.Invincibility;
        NoDamage = settings.Debug.Cheat.NoDamage;
        #endregion
#else
        ShowDebugBar = UserSettings.Default.Debug.ShowMenu;
        ShowHitboxes = UserSettings.Default.Debug.ShowHitboxes;
        Engine.Instance.FPS = UserSettings.Default.Debug.Framerate;

        #region Cheats
        Invincibility = UserSettings.Default.Debug.Cheat.Invincibility;
        NoDamage = UserSettings.Default.Debug.Cheat.NoDamage;
        #endregion
#endif
    }

    public void SaveSettings(Setting settings)
    {
        settings.Debug = new LastDebug {
            ShowMenu = ShowDebugBar,
            ShowHitboxes = ShowHitboxes,
            Framerate = Engine.Instance.FPS,
            Cheat = new LastCheat {
                Invincibility = Invincibility,
                NoDamage = NoDamage
            }
        };
    }

    private void Instance_GameLogicTick(GameTickEventArgs e)
    {
        Dispatcher.UIThread.Post(() => {
            var fps = 1 / e.TimeElapsed;
            FpsLabel = $"FPS: {fps:N0} / {Engine.Instance.FPS}";
            ThinkLabel = "Busy: " + (Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
            EntityLabel = "Entities: " + Game.DebugEntitiesAlive();
        });
    }
}
