using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class FunctionsXmlReader : IIncludeXmlReader
    {
        private readonly FunctionXmlReader functionReader;

        public FunctionsXmlReader(FunctionXmlReader functionReader)
        {
            this.functionReader = functionReader;
        }

        public string NodeName
        {
            get
            {
                return "Functions";
            }
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (var el in xmlNode.Elements("Function"))
            {
                group.Add(functionReader.Load(project, el, dataSource));
            }

            return group;
        }
    }
}
