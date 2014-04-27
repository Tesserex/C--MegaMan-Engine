using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml.Includes
{
    internal class FontsXmlReader : IIncludeXmlReader
    {
        private FontXmlReader _fontReader;

        public FontsXmlReader(FontXmlReader fontReader)
        {
            _fontReader = fontReader;
        }

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var fontNode in xmlNode.Elements("Font"))
            {
                _fontReader.Load(project, fontNode);
            }
        }

        public string NodeName
        {
            get { return "Fonts"; }
        }
    }
}
