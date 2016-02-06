using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class AddInventoryEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "AddInventory";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new AddInventoryEffectPartInfo() {
                ItemName = partNode.RequireAttribute("item").Value,
                Quantity = partNode.TryAttribute<int>("quantity", 1)
            };
        }
    }
}
