using MegaMan.Common;
using System.Xml.Linq;

namespace MegaMan.IO
{
    public interface ISoundReader
    {
        SoundInfo Load(XElement node);
    }
}
