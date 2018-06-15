using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

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
            return new RemoveInventoryEffectPartInfo {
                ItemName = partNode.RequireAttribute("item").Value,
                Quantity = partNode.TryAttribute("quantity", 1)
            };
        }
    }
}
