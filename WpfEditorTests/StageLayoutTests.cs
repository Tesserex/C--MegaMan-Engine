using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MegaMan.Editor.Bll.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.Editor.Tests
{
    [TestClass]
    public class StageLayoutTests
    {
        private FakeScreenData targetScreen;
        private FakeScreenData otherScreen;

        [TestInitialize]
        public void Init()
        {
            targetScreen = new FakeScreenData();
            otherScreen = new FakeScreenData();
        }

        [TestMethod, STAThread]
        public void OverlapMakesHorizontalJoin()
        {
            var joiner = new ScreenSnapJoiner();
            var target = new ScreenWithPosition() {
                Screen = targetScreen,
                Bounds = new Rect(5, 0, 256, 224)
            };

            var other = new ScreenWithPosition() {
                Screen = otherScreen,
                Bounds = new Rect(256, 0, 256, 224)
            };

            joiner.SnapScreenJoin(target, new[] { other });

            Assert.AreEqual(1, targetScreen.Joins.Count());
            Assert.IsTrue(targetScreen.Joins.Single().Type == Common.JoinType.Vertical);
        }

        [TestMethod, STAThread]
        public void OverlapMakesVerticalJoin()
        {
            var joiner = new ScreenSnapJoiner();
            var target = new ScreenWithPosition() {
                Screen = targetScreen,
                Bounds = new Rect(0, 5, 256, 224)
            };

            var other = new ScreenWithPosition() {
                Screen = otherScreen,
                Bounds = new Rect(0, 224, 256, 224)
            };

            joiner.SnapScreenJoin(target, new[] { other });

            Assert.AreEqual(1, targetScreen.Joins.Count());
            Assert.IsTrue(targetScreen.Joins.Single().Type == Common.JoinType.Horizontal);
        }

        [TestMethod, STAThread]
        public void NearbyMakesHorizontalJoin()
        {
            var joiner = new ScreenSnapJoiner();
            var target = new ScreenWithPosition() {
                Screen = targetScreen,
                Bounds = new Rect(250, 0, 256, 224)
            };

            var other = new ScreenWithPosition() {
                Screen = otherScreen,
                Bounds = new Rect(512, 0, 256, 224)
            };

            joiner.SnapScreenJoin(target, new[] { other });

            Assert.AreEqual(1, targetScreen.Joins.Count());
            Assert.IsTrue(targetScreen.Joins.Single().Type == Common.JoinType.Vertical);
        }

        [TestMethod, STAThread]
        public void NearbyMakesVerticalJoin()
        {
            var joiner = new ScreenSnapJoiner();
            var target = new ScreenWithPosition() {
                Screen = targetScreen,
                Bounds = new Rect(0, 216, 256, 224)
            };

            var other = new ScreenWithPosition() {
                Screen = otherScreen,
                Bounds = new Rect(0, 448, 256, 224)
            };

            joiner.SnapScreenJoin(target, new[] { other });

            Assert.AreEqual(1, targetScreen.Joins.Count());
            Assert.IsTrue(targetScreen.Joins.Single().Type == Common.JoinType.Horizontal);
        }

        [TestMethod, STAThread]
        public void DiagonalDoesNotJoin()
        {
            var joiner = new ScreenSnapJoiner();
            var target = new ScreenWithPosition() {
                Screen = targetScreen,
                Bounds = new Rect(4, 220, 256, 224)
            };

            var other = new ScreenWithPosition() {
                Screen = otherScreen,
                Bounds = new Rect(256, 0, 256, 224)
            };

            joiner.SnapScreenJoin(target, new[] { other });

            Assert.AreEqual(0, targetScreen.Joins.Count());
        }
    }
}
