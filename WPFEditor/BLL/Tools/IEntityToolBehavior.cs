namespace MegaMan.Editor.Bll.Tools
{
    public interface IEntityToolBehavior : IToolBehavior
    {
        int SnapX { get; set; }
        int SnapY { get; set; }
    }
}
