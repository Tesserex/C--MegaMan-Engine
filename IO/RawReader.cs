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
            using (var stream = _dataSource.GetData(path))
            {
                using (var br = new BinaryReader(stream))
                {
                    return br.ReadBytes((int)stream.Length);
                }
            }
        }

        public void Init(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }
    }
}
