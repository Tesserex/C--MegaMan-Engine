using System;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public static class XmlExtensions
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

        public static T TryAttribute<T>(this XElement node, String attributeName)
        {
            return TryAttribute<T>(node, attributeName, default(T));
        }

        public static T TryAttribute<T>(this XElement node, String attributeName, T defaultValue)
        {
            XAttribute attr = node.Attribute(attributeName);

            if (attr == null || String.IsNullOrEmpty(attr.Value))
            {
                return defaultValue;
            }

            return ParseValue<T>(node, attr);
        }

        public static T GetAttribute<T>(this XElement node, String attributeName)
        {
            return ParseValue<T>(node, node.RequireAttribute(attributeName));
        }

        private static T ParseValue<T>(XElement node, XAttribute attribute)
        {
            try
            {
                return ConvertValue<T>(attribute.Value);
            }
            catch (Exception ex)
            {
                if (ex is InvalidCastException || ex is FormatException)
                {
                    String msg = String.Format("{0} node's {1} attribute was of the wrong type.", node.Name, attribute.Name);
                    throw new GameXmlException(attribute, msg);
                }

                throw;
            }
        }

        public static T TryValue<T>(this XElement node)
        {
            return TryValue<T>(node, default(T));
        }

        public static T TryValue<T>(this XElement node, T defaultValue)
        {
            if (String.IsNullOrEmpty(node.Value))
            {
                return defaultValue;
            }

            return GetValue<T>(node);
        }

        public static T TryElementValue<T>(this XElement node, string name)
        {
            return TryElementValue(node, name, default(T));
        }

        public static T TryElementValue<T>(this XElement node, string name, T defaultValue)
        {
            var el = node.Element(name);
            if (el == null)
                return defaultValue;

            return TryValue(el, defaultValue);
        }

        public static T GetValue<T>(this XElement node)
        {
            try
            {
                return ConvertValue<T>(node.Value);
            }
            catch (Exception ex)
            {
                if (ex is InvalidCastException || ex is FormatException)
                {
                    String msg = String.Format("{0} node's value was of the wrong type.", node.Name);
                    throw new GameXmlException(node, msg);
                }

                throw;
            }
        }

        private static T ConvertValue<T>(string value)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (underlyingType.IsEnum)
            {
                return (T)Enum.Parse(underlyingType, value, true);
            }
            else
            {
                return (T)Convert.ChangeType(value, underlyingType, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
