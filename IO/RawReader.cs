using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using MegaMan.IO.DataSources;

namespace MegaMan.IO
{
    public class RawReader : IRawReader
    {
        private IDataSource _dataSource;

        public byte[] GetRawData(FilePath path)
        {
            return Extensions.GetBytesFromFilePath(_dataSource, path);
        }

        public void Init(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }
    }
}
