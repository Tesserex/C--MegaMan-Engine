using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common.Entities;
using MegaMan.IO.Xml.Effects;
using MegaMan.IO.Xml.Entities;

namespace MegaMan.IO.Xml.Includes
{
    public class EntityXmlWriter : IEntityWriter
    {
        private EffectXmlWriter effectWriter;

        public EntityXmlWriter()
        {
            effectWriter = new EffectXmlWriter();
        }

        public void Write(EntityInfo entity, string filepath)
        {
            var writer = new XmlTextWriter(filepath, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            Write(entity, writer);

            writer.Close();
        }

        internal void Write(EntityInfo entity, XmlWriter writer)
        {
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
                writer.WriteAttributeString("isProjectile", entity.EditorData.IsProjectile.ToString());

                writer.WriteEndElement();
            }

            foreach (var component in entity.Components)
                WritePart(component, writer);

            if (entity.Death != null)
            {
                writer.WriteStartElement("Death");
                effectWriter.WriteEffectContents(entity.Death, writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        internal void WritePart(IComponentInfo info, XmlWriter writer)
        {
            if (!componentWriters.ContainsKey(info.GetType()))
                throw new Exception("No xml writer for component type: " + info.GetType().Name);

            var compWriter = componentWriters[info.GetType()];

            compWriter.Write(info, writer);
        }

        private static Dictionary<Type, IComponentXmlWriter> componentWriters;

        static EntityXmlWriter()
        {
            componentWriters = Extensions.GetImplementersOf<IComponentXmlWriter>()
                .ToDictionary(x => x.ComponentType);
        }
    }
}
