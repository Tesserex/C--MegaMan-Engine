using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Includes
{
    internal class FunctionXmlReader : IIncludeXmlReader
    {
        private readonly EffectXmlReader _effectReader;

        public FunctionXmlReader(EffectXmlReader effectReader)
        {
            _effectReader = effectReader;
        }

        public string NodeName
        {
            get
            {
                return "Function";
            }
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var effect = _effectReader.Load(xmlNode);
            project.AddFunction(effect);
            return effect;
        }
    }
}
