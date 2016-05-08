using System.Collections.Generic;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO
{
    public interface IIncludedObjectGroupWriter
    {
        void Write(IEnumerable<IIncludedObject> includedObjects, string filepath);
    }
}
