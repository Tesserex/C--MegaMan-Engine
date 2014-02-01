using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class IncludeFileXmlReader
    {
        private Project _project;

        private Dictionary<string, IGameObjectXmlReader> _readers = new Dictionary<string, IGameObjectXmlReader>();

        public void LoadIncludedFile(Project project, string path)
        {
            _project = project;

            try
            {
                XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    _readers[element.Name.LocalName].Load(project, element);
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = path;
                throw;
            }
        }
    }
}
