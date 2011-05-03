using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan;

namespace Mega_Man
{
    public static class GameXml
    {
        public static XAttribute RequireAttribute(this XElement node, string name)
        {
            XAttribute attr;
            if (!node.TryAttribute(name, out attr))
            {
                string msg = string.Format("{0} node requires the attribute \"{1}\"", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return attr;
        }

        public static bool TryAttribute(this XElement node, string name, out XAttribute attribute)
        {
            XAttribute attr = node.Attribute(name);
            if (node == null)
            {
                attribute = null;
                return false;
            }
            attribute = attr;
            return true;
        }

        public static bool TryBool(this XElement node, string name, out bool result)
        {
            XAttribute attr = node.Attribute(name);
            result = false;
            if (node == null) return false;
            return bool.TryParse(attr.Value, out result);
        }

        public static bool GetBool(this XElement node, string name)
        {
            bool result;
            if (!bool.TryParse(node.RequireAttribute(name).Value, out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a boolean (\"true\" or \"false\").", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool TryInteger(this XElement node, string name, out int result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (node == null) return false;
            return attr.Value.TryParse(out result);
        }

        public static int GetInteger(this XElement node, string name)
        {
            int result;
            if (!node.RequireAttribute(name).Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be an integer.", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool TryFloat(this XElement node, string name, out float result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (node == null) return false;
            return attr.Value.TryParse(out result);
        }

        public static float GetFloat(this XElement node, string name)
        {
            float result;
            if (!node.RequireAttribute(name).Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a number, using a period as a decimal mark.", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }

        public static bool TryDouble(this XElement node, string name, out double result)
        {
            XAttribute attr = node.Attribute(name);
            result = 0;
            if (node == null) return false;
            return attr.Value.TryParse(out result);
        }

        public static double GetDouble(this XElement node, string name)
        {
            double result;
            if (!node.RequireAttribute(name).Value.TryParse(out result))
            {
                string msg = string.Format("{0} node's {1} attribute must be a number, using a period as a decimal mark.", node.Name, name);
                throw new GameXmlException(node, msg);
            }
            return result;
        }
    }
}
