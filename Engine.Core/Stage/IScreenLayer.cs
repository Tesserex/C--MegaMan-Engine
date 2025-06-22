namespace MegaMan.Engine.Core
{
    public interface IScreenLayer
    {
        int LocationX { get; }
        int LocationY { get; }
        IGameplayContainer Stage { get; }
        MapSquare SquareAt(float px, float py);
    }
}
