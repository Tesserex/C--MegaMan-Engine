using System;
using System.Xml;
using MegaMan.Common.Entities;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Entities
{
    internal class MovementComponentXmlWriter : IComponentXmlWriter
    {
        private readonly MovementEffectPartXmlWriter _effectWriter;

        public MovementComponentXmlWriter(MovementEffectPartXmlWriter effectWriter)
        {
            _effectWriter = effectWriter;
        }

        public Type ComponentType
        {
            get { return typeof(MovementComponentInfo); }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var move = (MovementComponentInfo)info;

            if (move.EffectInfo != null)
            {
                _effectWriter.Write(move.EffectInfo, writer);
            }
        }
    }
}
