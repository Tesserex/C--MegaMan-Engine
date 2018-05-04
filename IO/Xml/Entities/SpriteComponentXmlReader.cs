using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class SpriteComponentXmlReader : IComponentXmlReader
    {
        private readonly SpriteXmlReader _spriteReader;

        public SpriteComponentXmlReader(SpriteXmlReader spriteReader)
        {
            _spriteReader = spriteReader;
        }

        public string NodeName { get { return null; } }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var spriteComponent = new SpriteComponentInfo();

            FilePath sheetPath = null;
            var sheetNode = node.Element("Tilesheet");
            if (sheetNode != null)
            {
                sheetPath = FilePath.FromRelative(sheetNode.Value, project.BaseDir);
                spriteComponent.SheetPath = sheetPath;
            }

            foreach (var spriteNode in node.Elements("Sprite"))
            {
                if (sheetPath == null)
                {
                    var sprite = _spriteReader.LoadSprite(dataSource, spriteNode, project.BaseDir);
                    spriteComponent.Sprites.Add(sprite.Name ?? "Default", sprite);
                }
                else
                {
                    var sprite = _spriteReader.LoadSprite(spriteNode);
                    sprite.SheetPath = sheetPath;
                    sprite.SheetData = dataSource.GetBytesFromFilePath(sheetPath);
                    spriteComponent.Sprites.Add(sprite.Name ?? "Default", sprite);
                }
            }

            if (spriteComponent.SheetPath != null || spriteComponent.Sprites.Any())
                return spriteComponent;
            else
                return null;
        }
    }
}
