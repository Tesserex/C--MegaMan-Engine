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

            var project = new Project() { BaseDir = @"C:\" };
            new SceneXmlReader().Load(project, xml);

            Assert.AreEqual("TestScene", project.Scenes.First().Name);
        }
    }
}
