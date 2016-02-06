using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SpawnEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly PositionEffectPartXmlReader _positionReader;

        public SpawnEffectPartXmlReader(PositionEffectPartXmlReader positionReader)
        {
            _positionReader = positionReader;
        }


        public string NodeName
        {
            get
            {
                return "Spawn";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            var info = new SpawnEffectPartInfo();
            info.Name = partNode.GetAttribute<string>("name");
            info.State = partNode.TryAttribute<string>("state", "Start");

            info.Position = (PositionEffectPartInfo)_positionReader.Load(partNode);

            return info;
        }
    }
}
