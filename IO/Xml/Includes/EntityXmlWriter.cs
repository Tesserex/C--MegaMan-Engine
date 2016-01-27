using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common.Entities;
using MegaMan.IO.Xml.Entities;

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

            foreach (var component in entity.Components)
                WritePart(component, writer);

            writer.Close();
        }

        public void WritePart(IComponentInfo info, XmlWriter writer)
        {
            if (!ComponentWriters.ContainsKey(info.GetType()))
                throw new Exception("No xml writer for component type: " + info.GetType().Name);

            var compWriter = ComponentWriters[info.GetType()];

            compWriter.Write(info, writer);
        }

        private static Dictionary<Type, IComponentXmlWriter> ComponentWriters;

        static EntityXmlWriter()
        {
            ComponentWriters = Extensions.GetImplementersOf<IComponentXmlWriter>()
                .ToDictionary(x => x.ComponentType);
        }
    }
}
