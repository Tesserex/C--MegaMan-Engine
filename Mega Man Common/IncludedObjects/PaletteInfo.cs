namespace MegaMan.Common.IncludedObjects
{
    public class PaletteInfo : IncludedObject
    {
        public string Name { get; set; }
        public FilePath ImagePath { get; set; }
        public byte[] ImageData { get; set; }
    }
}
