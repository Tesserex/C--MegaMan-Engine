namespace MegaMan.IO
{
    public interface IGameLoader
    {
        IReaderProvider Load(string filepath);
    }
}
