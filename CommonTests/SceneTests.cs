using MegaMan.IO.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class SceneTests
    {
        [TestMethod, TestCategory("Scene")]
        public void LoadScene()
        {
            var xmlString = TestHelpers.GetInputFile("Scene_Correct.xml");
            var xml = XElement.Parse(xmlString);

            var reader = new IncludeFileXmlReader();
            var scene = reader.LoadScene(xml, @"C:\");

            Assert.AreEqual("TestScene", scene.Name);
        }
    }
}
