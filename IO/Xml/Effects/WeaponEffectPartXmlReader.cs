using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class WeaponEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Weapon";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new WeaponEffectPartInfo();

            if (partNode.Value == "Shoot")
            {
                info.Action = WeaponAction.Shoot;
            }
            else if (partNode.Value == "RotateForward")
            {
                info.Action = WeaponAction.RotateForward;
            }
            else if (partNode.Value == "RotateBackward")
            {
                info.Action = WeaponAction.RotateBackward;
            }
            else
            {
                var ammoNode = partNode.Element("Ammo");
                if (ammoNode != null)
                {
                    info.Ammo = ammoNode.GetAttribute<int>("val");
                    info.Action = WeaponAction.Ammo;
                }

                var changeNode = partNode.Element("Change");
                if (changeNode != null)
                {
                    info.ChangeName = changeNode.RequireAttribute("name").Value;
                    info.Action = WeaponAction.Change;
                }
            }

            return info;
        }
    }
}
