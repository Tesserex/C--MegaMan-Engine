using MegaMan.Engine.Avalonia;

namespace Engine.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public EngineGame CurrentGame { get; set; } = new EngineGame();
}
