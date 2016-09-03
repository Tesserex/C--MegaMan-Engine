using MegaMan.IO.Xml;
using MegaMan.IO.Xml.Includes;
using Ninject;

namespace MegaMan.IO
{
    public class WriterProvider : IWriterProvider
    {
        public IProjectWriter GetProjectWriter()
        {
            return Injector.Container.Get<ProjectXmlWriter>();
        }

        public IStageWriter GetStageWriter()
        {
            return Injector.Container.Get<StageXmlWriter>();
        }

        public ITilesetWriter GetTilesetWriter()
        {
            return new TilesetXmlWriter(new SpriteXmlWriter());
        }

        public IEntityWriter GetEntityWriter()
        {
            return new EntityXmlWriter();
        }

        public IIncludedObjectGroupWriter GetEntityGroupWriter()
        {
            return Injector.Container.Get<EntityGroupXmlWriter>();
        }
    }
}
