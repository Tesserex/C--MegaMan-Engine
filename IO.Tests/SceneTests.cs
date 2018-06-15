using System.Linq;
using System.Xml.Linq;
using MegaMan.IO.Tests;
using MegaMan.IO.Xml.Handlers;
using MegaMan.IO.Xml.Handlers.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var project = new Project { GameFile = FilePath.FromRelative("game.xml", @"C:\") };
            new SceneXmlReader(new HandlerTransferXmlReader(), new HandlerCommandXmlReader()).Load(project, xml, null);

            Assert.AreEqual("TestScene", project.Scenes.First().Name);
        }
    }
}
