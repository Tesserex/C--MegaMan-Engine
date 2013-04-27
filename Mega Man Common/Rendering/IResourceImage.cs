using System;

namespace MegaMan.Common.Rendering
{
    public interface IResourceImage
    {
        Int32 ResourceId { get; }
        String PaletteName { get; }
        Int32 Width { get; }
        Int32 Height { get; }
    }
}
