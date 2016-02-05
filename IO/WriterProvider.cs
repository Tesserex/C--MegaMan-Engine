using MegaMan.IO.Xml;
using Ninject;

namespace MegaMan.IO
{
    public class WriterProvider : IWriterProvider
    {
        public IProjectWriter GetProjectWriter()
        {
            return new ProjectXmlWriter();
        }

        public IStageWriter GetStageWriter()
        {
            return Injector.Container.Get<StageXmlWriter>();
        }

        public ITilesetWriter GetTilesetWriter()
        {
            return new TilesetXmlWriter(new SpriteXmlWriter());
        }
    }
}
