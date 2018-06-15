using MegaMan.IO.DataSources;

namespace MegaMan.IO
{
    public interface IGameFileReader
    {
        void Init(IDataSource dataSource);
    }
}
