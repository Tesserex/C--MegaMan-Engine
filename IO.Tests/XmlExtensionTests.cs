using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using MegaMan.IO.Xml;

namespace MegaMan.IO.Tests
{
    [TestClass]
    public class XmlExtensionTests
    {
        [TestMethod, TestCategory("Xml")]
        public void RequireAttribute_HasAttribute_Succeed()
        {
            var input = TestHelpers.GetInputFile("Xml_RequireAttribute.xml");
            var xmlNode = XElement.Parse(input);

            xmlNode.RequireAttribute("fooString");
        }

        [TestMethod, TestCategory("Xml")]
        [ExpectedException(typeof(GameXmlException))]
        public void RequireAttribute_NoAttribute_Exception()
        {
            var input = TestHelpers.GetInputFile("Xml_RequireAttribute.xml");
            var xmlNode = XElement.Parse(input);

            xmlNode.RequireAttribute("does-not-exist");
        }

        [TestMethod, TestCategory("Xml")]
        public void GetAttributeT_Valid_Parses()
        {
            var input = TestHelpers.GetInputFile("Xml_RequireAttribute.xml");
            var xmlNode = XElement.Parse(input);

            Assert.AreEqual("bar", xmlNode.GetAttribute<String>("fooString"));
            Assert.AreEqual(true, xmlNode.GetAttribute<Boolean>("fooBool"));
            Assert.AreEqual(64, xmlNode.GetAttribute<Int32>("fooInt"));
            Assert.AreEqual(-1.5, xmlNode.GetAttribute<Double>("fooDouble"));
        }

        [TestMethod, TestCategory("Xml")]
        [ExpectedException(typeof(GameXmlException))]
        public void TryAttributeT_BadValue_Exception()
        {
            var input = TestHelpers.GetInputFile("Xml_RequireAttribute.xml");
            var xmlNode = XElement.Parse(input);

            xmlNode.TryAttribute<Double>("fooString");
        }

        [TestMethod, TestCategory("Xml")]
        public void TryAttributeT_NoAttribute_Defaults()
        {
            var input = TestHelpers.GetInputFile("Xml_RequireAttribute.xml");
            var xmlNode = XElement.Parse(input);

            Assert.AreEqual(null, xmlNode.TryAttribute<String>("does-not-exist"));
            Assert.AreEqual(false, xmlNode.TryAttribute<Boolean>("does-not-exist"));
            Assert.AreEqual(0, xmlNode.TryAttribute<Int32>("does-not-exist"));
            Assert.AreEqual(0.0, xmlNode.TryAttribute<Double>("does-not-exist"));
        }
    }
}
