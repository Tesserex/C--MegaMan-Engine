using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    internal class FunctionsXmlReader : IIncludeXmlReader
    {
        private readonly FunctionXmlReader _functionReader;

        public FunctionsXmlReader(FunctionXmlReader functionReader)
        {
            _functionReader = functionReader;
        }

        public string NodeName
        {
            get
            {
                return "Functions";
            }
        }

        public IIncludedObject Load(Project project, XElement xmlNode)
        {
            var group = new IncludedObjectGroup();
            foreach (var el in xmlNode.Elements("Function"))
            {
                group.Add(_functionReader.Load(project, el));
            }

            return group;
        }
    }
}
