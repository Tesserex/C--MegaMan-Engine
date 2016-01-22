using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntityXmlWriter
    {
        public void Write(EntityInfo entity, string filepath)
        {
            XmlTextWriter writer = new XmlTextWriter(filepath, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Entity");
            writer.WriteAttributeString("name", entity.Name);
            writer.WriteAttributeString("maxAlive", entity.MaxAlive.ToString());

            writer.WriteElementString("GravityFlip", entity.GravityFlip.ToString());

            if (entity.EditorData != null)
            {
                writer.WriteStartElement("EditorData");

                if (entity.DefaultSprite != null)
                    writer.WriteAttributeString("defaultSprite", entity.DefaultSprite.Name);

                writer.WriteAttributeString("hide", entity.EditorData.HideFromPlacement.ToString());
            }

            if (entity.SpriteComponent != null)
                WriteSprites(writer, entity.SpriteComponent);

            writer.Close();
        }

        private void WriteSprites(XmlTextWriter writer, SpriteComponentInfo spriteComponent)
        {
            if (spriteComponent.SheetPath != null)
                writer.WriteElementString("Tilesheet", spriteComponent.SheetPath.Relative);

            foreach (var sprite in spriteComponent.Sprites.Values)
                sprite.WriteTo(writer);
        }
    }
}
