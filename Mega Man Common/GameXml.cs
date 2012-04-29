using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.Common
{
    public static class GameXml
    {
        public static XAttribute RequireAttribute(this XElement node, string name)
        {
            XAttribute attr = node.Attribute(name);
            if (attr == null)
            {
                string msg = string.Format("{0} node requires the attribute \"{1}\"", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return attr;
        }

        public static bool TryBool(this XElement node, string name, out bool result)
        {
            XAttribute attr = node.Attribute(name);
            result = false;
            if (attr == null) return false;
            result = RequireBool(node, attr);
            return true;
        }

        public static bool GetBool(this XElement node)
        {
            bool result;
            if (!bool.TryParse(node.Value, out result))
            {
                string msg = string.Format("{0} node's value must be a boolean (\"true\" or \"false\").", node.Name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool GetBool(this XElement node, string name)
        {
            return RequireBool(node, node.RequireAttribute(name));
        }

        private static bool RequireBool(XElement node, XAttribute attr)
        {
            bool result;
            if (!bool.TryParse(attr.Value, out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a boolean (\"true\" or \"false\").", node.Name, attr.Name);
                throw new GameXmlException(attr, msg);
            }
            return result;
        }

        public static bool TryInteger(this XElement node, string name, out int result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (attr == null) return false;
            result = RequireInteger(node, attr);
            return true;
        }

        public static int GetInteger(this XElement node, string name)
        {
            return RequireInteger(node, node.RequireAttribute(name));
        }

        private static int RequireInteger(XElement node, XAttribute attr)
        {
            int result;
            if (!attr.Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be an integer.", node.Name, attr.Name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool TryFloat(this XElement node, string name, out float result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (attr == null) return false;
            result = RequireFloat(node, attr);
            return true;
        }

        public static float GetFloat(this XElement node, string name)
        {
            return RequireFloat(node, node.RequireAttribute(name));
        }

        public static float RequireFloat(XElement node, XAttribute attr)
        {
            float result;
            if (!attr.Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a number, using a period as a decimal mark.", node.Name, attr.Name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool TryDouble(this XElement node, string name, out double result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (attr == null) return false;
            result = RequireDouble(node, attr);
            return true;
        }

        public static double GetDouble(this XElement node, string name)
        {
            return RequireDouble(node, node.RequireAttribute(name));
        }

        public static double RequireDouble(XElement node, XAttribute attr)
        {
            float result;
            if (!attr.Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a number, using a period as a decimal mark.", node.Name, attr.Name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }
    }
}
