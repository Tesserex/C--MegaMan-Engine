using MegaMan.Common;
using System.Xml.Linq;

namespace MegaMan.IO
{
    public interface IIncludeReader
    {
        void Load(Project project, XElement xmlNode);
    }
}
