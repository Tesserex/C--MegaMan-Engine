using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;
using MegaMan.IO.Xml;

namespace MegaMan.IO
{
    public class ReaderProvider : IReaderProvider
    {
        public IProjectReader GetProjectReader(FilePath path)
        {
            var mainExt = path.Relative.Substring(path.Relative.Length - 3);
            if (ProjectReaders.ContainsKey(mainExt))
                return ProjectReaders[mainExt];

            throw new ArgumentException("The game file is not of a supported type.");
        }

        private static Dictionary<string, IProjectReader> ProjectReaders;

        static ReaderProvider()
        {
            ProjectReaders = Extensions.GetImplementersOf<IProjectReader>()
                .ToDictionary(r => r.Extension);
        }
    }
}
