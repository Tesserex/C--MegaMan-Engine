using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class AddInventoryEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(AddInventoryEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var partInfo = (AddInventoryEffectPartInfo)info;

            writer.WriteStartElement("AddInventory");
            writer.WriteAttributeString("item", partInfo.ItemName);
            writer.WriteAttributeString("quantity", partInfo.Quantity.ToString());
            writer.WriteEndElement();
        }
    }
}
