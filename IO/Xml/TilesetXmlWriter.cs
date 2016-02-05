using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class TilesetXmlWriter : ITilesetWriter
    {
        private readonly SpriteXmlWriter _spriteWriter;

        public TilesetXmlWriter(SpriteXmlWriter spriteWriter)
        {
            _spriteWriter = spriteWriter;
        }

        public void Save(Tileset tileset)
        {
            XmlTextWriter writer = new XmlTextWriter(tileset.FilePath.Absolute, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Tileset");

            if (tileset.SheetPath != null)
                writer.WriteAttributeString("tilesheet", tileset.SheetPath.Relative);

            writer.WriteAttributeString("tilesize", tileset.TileSize.ToString());

            writer.WriteStartElement("TileProperties");
            foreach (TileProperties properties in tileset.Properties)
            {
                if (properties.Name == "Default" && properties == TileProperties.Default)
                    continue;

                SaveProperties(properties, writer);
            }
            writer.WriteEndElement();

            foreach (Tile tile in tileset)
            {
                writer.WriteStartElement("Tile");
                writer.WriteAttributeString("id", tile.Id.ToString());
                writer.WriteAttributeString("name", tile.Name);
                writer.WriteAttributeString("properties", tile.Properties.Name);

                _spriteWriter.Write(tile.Sprite, writer);

                writer.WriteEndElement();   // end Tile
            }
            writer.WriteEndElement();

            writer.Close();
        }

        private void SaveProperties(TileProperties properties, XmlTextWriter writer)
        {
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("name", properties.Name);
            if (properties.Blocking) writer.WriteAttributeString("blocking", "true");
            if (properties.Climbable) writer.WriteAttributeString("climbable", "true");
            if (properties.Lethal) writer.WriteAttributeString("lethal", "true");
            if (properties.GravityMult != 1) writer.WriteAttributeString("gravitymult", properties.GravityMult.ToString());
            if (properties.PushX != 0) writer.WriteAttributeString("pushX", properties.PushX.ToString());
            if (properties.PushY != 0) writer.WriteAttributeString("pushY", properties.PushY.ToString());
            if (properties.ResistX != 1) writer.WriteAttributeString("resistX", properties.ResistX.ToString());
            if (properties.ResistY != 1) writer.WriteAttributeString("resistY", properties.ResistY.ToString());
            if (properties.DragX != 1) writer.WriteAttributeString("dragX", properties.DragX.ToString());
            if (properties.DragY != 1) writer.WriteAttributeString("dragY", properties.DragY.ToString());
            if (properties.Sinking != 0) writer.WriteAttributeString("sinking", properties.Sinking.ToString());
            if (properties.OnEnter != null) writer.WriteAttributeString("onenter", properties.OnEnter);
            if (properties.OnLeave != null) writer.WriteAttributeString("onleave", properties.OnLeave);
            if (properties.OnOver != null) writer.WriteAttributeString("onover", properties.OnOver);
            writer.WriteEndElement();
        }
    }
}
