using System.Linq;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class FuncEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Func";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new FuncEffectPartInfo {
                Statements = partNode.Value.Split(';').Where(st => !string.IsNullOrEmpty(st.Trim()))
            };
        }
    }
}
