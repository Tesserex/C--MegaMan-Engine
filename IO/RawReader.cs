using MegaMan.Common;
using MegaMan.IO.DataSources;

namespace MegaMan.IO
{
    public class RawReader : IRawReader
    {
        private IDataSource dataSource;

        public byte[] GetRawData(FilePath path)
        {
            return dataSource.GetBytesFromFilePath(path);
        }

        public void Init(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }
    }
}
