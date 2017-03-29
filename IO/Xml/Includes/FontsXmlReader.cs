﻿using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    internal class FontsXmlReader : IIncludeXmlReader
    {
        private FontXmlReader _fontReader;

        public FontsXmlReader(FontXmlReader fontReader)
        {
            _fontReader = fontReader;
        }

        public IIncludedObject Load(Project project, XElement xmlNode)
        {
            var group = new IncludedObjectGroup();
            foreach (var fontNode in xmlNode.Elements("Font"))
            {
                group.Add(_fontReader.Load(project, fontNode));
            }

            return group;
        }

        public string NodeName
        {
            get { return "Fonts"; }
        }
    }
}
