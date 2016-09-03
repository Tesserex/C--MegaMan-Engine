using System.Xml.Linq;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO
{
    public interface ISoundReader
    {
        SoundInfo Load(XElement node);
    }
}
