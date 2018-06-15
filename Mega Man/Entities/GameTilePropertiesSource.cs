using System.Collections.Generic;
using MegaMan.Common;

namespace MegaMan.Engine.Entities
{
    public class GameTilePropertiesSource : ITilePropertiesSource
    {
        private IDictionary<string, TileProperties> entityProperties = new Dictionary<string, TileProperties>();

        public GameTilePropertiesSource()
        {
            entityProperties["Default"] = TileProperties.Default;
        }

        public void LoadProperties(IDictionary<string, TileProperties> properties)
        {
            entityProperties = properties;
        }

        public TileProperties GetProperties(string name)
        {
            if (entityProperties.ContainsKey(name)) return entityProperties[name];
            return TileProperties.Default;
        }
    }
}
