using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;

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

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var el in xmlNode.Elements("Function"))
            {
                _functionReader.Load(project, el);
            }
        }
    }
}
