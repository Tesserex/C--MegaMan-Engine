using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Rendering
{
    public interface IResourceImage
    {
        Int32 ResourceId { get; }
        String PaletteName { get; }
        Int32 Width { get; }
        Int32 Height { get; }
    }
}
