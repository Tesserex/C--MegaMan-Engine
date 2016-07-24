using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class RemoveInventoryEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(RemoveInventoryEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var item = (RemoveInventoryEffectPartInfo)info;
            writer.WriteStartElement("RemoveInventory");
            writer.WriteAttributeString("item", item.ItemName);
            writer.WriteAttributeString("quantity", item.Quantity.ToString());
            writer.WriteEndElement();
        }
    }
}
