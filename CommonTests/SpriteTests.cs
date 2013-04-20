using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using MegaMan.IO.Xml;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class SpriteTests
    {
        [TestMethod, TestCategory("Sprite")]
        public void FromXml_NormalInput_Created()
        {
            Sprite sprite = GetSpriteFromFile("Sprite_NormalInput.xml", @"C:\");

            Assert.AreEqual("MyTestSprite", sprite.Name);
            Assert.AreEqual("foo.png", sprite.SheetPath.Relative);
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

        [TestMethod, TestCategory("Sprite")]
        public void Update_Playing_Runs()
        {
            var sprite = GetEmptySprite(5);

            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Play();
            sprite.Update();

            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_GreaterThanOne_SkipsFrames()
        {
            var sprite = GetEmptySprite(4);
            sprite.Play();

            sprite.Update(3);

            Assert.AreEqual(3, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Stopped_DoesntRun()
        {
            var sprite = GetEmptySprite(5);
            sprite.Play();
            sprite.Update();

            // should be at 1...

            sprite.Pause();
            sprite.Update();

            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Stop_Resets()
        {
            var sprite = GetEmptySprite(5);
            sprite.Play();
            sprite.Update();

            sprite.Stop();

            Assert.AreEqual(0, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_LongFrame_Stays()
        {
            var sprite = GetEmptySprite(5);
            sprite[0].Duration = 2;
            sprite.Play();

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_ForwardOnce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.PlayOnce;
            sprite.Play();

            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(2, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_BackwardOnce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Backward;
            sprite.AnimStyle = AnimationStyle.PlayOnce;
            sprite.Play();

            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_ForwardRepeat()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.Repeat;
            sprite.Play();

            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_BackwardRepeat()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Backward;
            sprite.AnimStyle = AnimationStyle.Repeat;
            sprite.Play();

            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Bounce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.Bounce;
            sprite.Play();

            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(2, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(0, sprite.CurrentIndex);

            sprite.Update();
            Assert.AreEqual(1, sprite.CurrentIndex);
        }

        private static Sprite GetEmptySprite(int frames)
        {
            var sprite = new Sprite(1, 1);

            for (int i = 0; i < frames; i++)
            {
                sprite.AddFrame(0, 0, 1);
            }

            return sprite;
        }

        public static Sprite GetSpriteFromFile(string filename, string basePath = null)
        {
            String xmlInput = TestHelpers.GetInputFile(filename);

            var xmlNode = XElement.Parse(xmlInput);

            if (basePath == null)
            {
                return GameXmlReader.LoadSprite(xmlNode);
            }
            else
            {
                return GameXmlReader.LoadSprite(xmlNode, basePath);
            }
        } 
    }
}
