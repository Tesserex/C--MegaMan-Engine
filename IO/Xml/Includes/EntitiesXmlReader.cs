using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntitiesXmlReader : IIncludeXmlReader
    {
        public string NodeName
        {
            get { return "Entities"; }
        }

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var node in xmlNode.Elements("Entity"))
            {
                var info = new EntityInfo() {
                    Name = node.RequireAttribute("name").Value,
                    MaxAlive = node.TryAttribute<int>("maxAlive", 50)
                };

                var editorData = node.Element("EditorData");
                if (editorData != null)
                {
                    info.EditorData = new EntityEditorData() {
                        DefaultSpriteName = editorData.TryAttribute<string>("defaultSprite"),
                        HideFromPlacement = editorData.TryAttribute<bool>("hide", false)
                    };
                }

                FilePath sheetPath = null;
                var sheetNode = node.Element("Tilesheet");
                if (sheetNode != null)
                    sheetPath = FilePath.FromRelative(sheetNode.Value, project.BaseDir);

                foreach (var spriteNode in node.Elements("Sprite"))
                {
                    if (sheetPath == null)
                    {
                        var sprite = GameXmlReader.LoadSprite(spriteNode, project.BaseDir);
                        info.Sprites.Add(sprite.Name ?? "Default", sprite);
                    }
                    else
                    {
                        var sprite = GameXmlReader.LoadSprite(spriteNode);
                        sprite.SheetPath = sheetPath;
                        info.Sprites.Add(sprite.Name ?? "Default", sprite);
                    }

                }

                project.AddEntity(info);
            }
        }
    }
}
