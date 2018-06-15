using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.IO.Tests
{
    [TestClass]
    public class SpriteTests
    {
        [TestMethod, TestCategory("Sprite")]
        public void FromXml_NormalInput_Created()
        {
            Sprite sprite = GetSpriteFromFile("Sprite_NormalInput.xml", @"C:\");

            Assert.AreEqual("MyTestSprite", sprite.Name);
            Assert.AreEqual("foo.png", sprite.SheetPathRelative);
            Assert.AreEqual(48, sprite.Width);
            Assert.AreEqual(24, sprite.Height);

            Assert.AreEqual("MyPalette", sprite.PaletteName);
            Assert.AreEqual(3, sprite.Layer);
            Assert.IsTrue(sprite.Reversed);

            Assert.AreEqual(24, sprite.HotSpot.X);
            Assert.AreEqual(8, sprite.HotSpot.Y);
            Assert.AreEqual(AnimationStyle.PlayOnce, sprite.AnimStyle);
            Assert.AreEqual(AnimationDirection.Backward, sprite.AnimDirection);

            Assert.AreEqual(2, sprite.Count);
            Assert.AreEqual(6, sprite[0].Duration);
            Assert.AreEqual(4, sprite[1].Duration);
        }

        [TestMethod, TestCategory("Sprite")]
        public void FromXml_NormalInput_SensibleDefaults()
        {
            var sprite = GetSpriteFromFile("Sprite_Defaults.xml");

            Assert.IsNull(sprite.PaletteName);
            Assert.AreEqual(0, sprite.Layer);
            Assert.IsFalse(sprite.Reversed);

            Assert.AreEqual(0, sprite.HotSpot.X);
            Assert.AreEqual(0, sprite.HotSpot.Y);
            Assert.AreEqual(AnimationStyle.Repeat, sprite.AnimStyle);
            Assert.AreEqual(AnimationDirection.Forward, sprite.AnimDirection);

            Assert.AreEqual(0, sprite[0].Duration);
        }

        public static Sprite GetSpriteFromFile(string filename, string basePath = null)
        {
            String xmlInput = TestHelpers.GetInputFile(filename);

            var xmlNode = XElement.Parse(xmlInput);

            var reader = new SpriteXmlReader();

            if (basePath == null)
            {
                return reader.LoadSprite(xmlNode);
            }

            var source = new XmlFileSource();
            return reader.LoadSprite(source, xmlNode, basePath);
        } 
    }
}
