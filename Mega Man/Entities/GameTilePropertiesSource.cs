using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.IO.Xml;

namespace MegaMan.Engine.Entities
{
    public class GameTilePropertiesSource : ITilePropertiesSource
    {
        private readonly Dictionary<string, TileProperties> entityProperties = new Dictionary<string, TileProperties>();

        public GameTilePropertiesSource()
        {
            entityProperties["Default"] = TileProperties.Default;
        }

        public void LoadProperties(XElement doc)
        {
            var reader = new TilesetXmlReader();
            XElement propHead = doc.Element("Properties");
            if (propHead != null)
            {
                foreach (XElement propNode in propHead.Elements("Properties"))
                {
                    TileProperties p = reader.LoadProperties(propNode);
                    entityProperties[p.Name] = p;
                }
            }
        }

        public TileProperties GetProperties(string name)
        {
            if (entityProperties.ContainsKey(name)) return entityProperties[name];
            return TileProperties.Default;
        }
    }
}
