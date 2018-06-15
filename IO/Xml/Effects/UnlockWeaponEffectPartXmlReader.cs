using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class UnlockWeaponEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "UnlockWeapon";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new UnlockWeaponEffectPartInfo {
                WeaponName = partNode.GetAttribute<string>("name")
            };
        }
    }
}
