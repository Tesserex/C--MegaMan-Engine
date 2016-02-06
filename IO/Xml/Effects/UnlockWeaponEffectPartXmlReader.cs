using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

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
            return new UnlockWeaponEffectPartInfo() {
                WeaponName = partNode.GetAttribute<string>("name")
            };
        }
    }
}
