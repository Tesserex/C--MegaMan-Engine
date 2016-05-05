using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class FuncEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(FuncEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var func = (FuncEffectPartInfo)info;
            var statements = string.Join(";", func.Statements);
            writer.WriteElementString("Func", statements);
        }
    }
}
