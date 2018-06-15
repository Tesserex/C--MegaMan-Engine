using System;
using System.Xml;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class GameXmlException : Exception
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
        public string Entity { get; set; }
        public string Tag { get; set; }
        public string Attribute { get; set; }

        public GameXmlException(XElement element, string message)
            : base(message)
        {
            Line = (element as IXmlLineInfo).LineNumber;
            Position = (element as IXmlLineInfo).LinePosition;
            Tag = element.Name.LocalName;
        }

        public GameXmlException(XAttribute attribute, string message)
            : this(attribute.Parent, message)
        {
            Attribute = attribute.Name.LocalName;
        }

        public GameXmlException(string file, int line, string entity, string tag, string attribute, string message) : base(message)
        {
            File = file;
            Line = line;
            Entity = entity;
            Tag = tag;
            Attribute = attribute;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class GameEntityException : Exception
    {
        public GameEntityException(string message)
            : base(message)
        {
        }
    }
}
