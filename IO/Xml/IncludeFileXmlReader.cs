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

        public IncludeFileXmlReader(Project project)
        {
            _project = project;
        }

        public void LoadIncludedFile(string path)
        {
            try
            {
                XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
                foreach (XElement element in document.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "Sounds":
                            LoadSounds(element);
                            break;
                    }
                }
            }
            catch (GameXmlException ex)
            {
                ex.File = path;
                throw;
            }
        }

        private void LoadSounds(XElement node)
        {
            foreach (XElement soundNode in node.Elements("Sound"))
            {
                _project.AddSound(SoundInfo.FromXml(soundNode, _project.BaseDir));
            }
        }
    }
}
