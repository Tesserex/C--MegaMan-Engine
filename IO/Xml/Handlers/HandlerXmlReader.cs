using System.Xml.Linq;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Handlers
{
    internal abstract class HandlerXmlReader
    {
        protected void LoadBase(HandlerInfo handler, XElement node, string basePath, IDataSource dataSource)
        {
            handler.Name = node.RequireAttribute("name").Value;

            var spriteLoader = new SpriteXmlReader();
            foreach (var spriteNode in node.Elements("Sprite"))
            {
                var info = new HandlerSpriteInfo();
                info.Sprite = spriteLoader.LoadSprite(dataSource, spriteNode, basePath);
                handler.Objects.Add(info.Name, info);
            }

            var meterLoader = new MeterXmlReader(new SceneBindingXmlReader());
            foreach (var meterNode in node.Elements("Meter"))
            {
                var meter = meterLoader.LoadMeter(meterNode, basePath, dataSource);
                handler.Objects.Add(meter.Name, meter);
            }
        }
    }
}
