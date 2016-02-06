using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class RemoveInventoryEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "RemoveInventory";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new RemoveInventoryEffectPartInfo() {
                ItemName = partNode.RequireAttribute("item").Value,
                Quantity = partNode.TryAttribute<int>("quantity", 1)
            };
        }
    }
}
