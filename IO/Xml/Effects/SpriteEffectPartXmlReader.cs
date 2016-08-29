using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class SpriteEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Sprite";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new SpriteEffectPartInfo()
            {
                Name = partNode.TryElementValue<string>("Name"),
                Playing = partNode.TryElementValue<bool?>("Playing"),
                Visible = partNode.TryElementValue<bool?>("Visible"),
                Facing = (partNode.TryElementValue<string>("Facing") == null) ? null : (FacingValues?)Enum.Parse(typeof(FacingValues), partNode.TryElementValue<string>("Facing"), true)
            };
        }
    }
}
