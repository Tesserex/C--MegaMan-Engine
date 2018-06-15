using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

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
