using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MegaMan.Engine;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;
using Moq;

namespace MegaMan.Engine.Tests.Components
{
    [TestClass]
    public class PositionComponentTests
    {
        private PositionComponent _position;
        private Mock<IEntity> _entityMock;

        [TestInitialize]
        public void Initialize()
        {
            _entityMock = new Mock<IEntity>();
            _position = new PositionComponent();
            _position.Parent = _entityMock.Object;
        }

        [TestMethod]
        public void SetPositionWithPoint()
        {
            var start = new Point(0, 0);
            var end = new PointF(5.1f, 3.66f);

            Assert.AreEqual(start, _position.Position);
            _position.SetPosition(end);
            Assert.AreEqual(new Point(5, 3), _position.Position);
        }

        [TestMethod]
        public void SetPositionWithValues()
        {
            var start = new Point(0, 0);
            var end = new PointF(5.1f, 3.66f);

            Assert.AreEqual(start, _position.Position);
            _position.SetPosition(5.1f, 3.66f);
            Assert.AreEqual(new Point(5, 3), _position.Position);
        }

        [TestMethod]
        public void Offset()
        {
            var start = new Point(0, 0);

            Assert.AreEqual(start, _position.Position);
            _position.Offset(2.4f, 1.92f);
            _position.Offset(2.7f, 1.74f);
            Assert.AreEqual(5, _position.X);
            Assert.AreEqual(3, _position.Y);
        }

        [TestMethod]
        public void Update_DoesntRemoveOnScreen()
        {
            var screen = new Mock<ITiledScreen>();
            screen.Setup(s => s.IsOnScreen(It.IsAny<float>(), It.IsAny<float>())).Returns(true);
            _entityMock.SetupGet(e => e.Screen).Returns(screen.Object);

            var removed = false;
            _entityMock.Setup(e => e.Remove()).Callback(() => { removed = true; });

            var container = new FakeGameplayContainer();
            _position.Start(container);

            container.Tick();

            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void Update_RemovesOffScreen()
        {
            var screen = new Mock<ITiledScreen>();
            screen.Setup(s => s.IsOnScreen(It.IsAny<float>(), It.IsAny<float>())).Returns(false);
            _entityMock.SetupGet(e => e.Screen).Returns(screen.Object);
            _entityMock.SetupGet(e => e.Name).Returns("Foo");

            var removed = false;
            _entityMock.Setup(e => e.Remove()).Callback(() => { removed = true; });

            var container = new FakeGameplayContainer();
            _position.Start(container);

            container.Tick();

            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void Update_DoesntRemoveOffScreenPlayer()
        {
            var screen = new Mock<ITiledScreen>();
            screen.Setup(s => s.IsOnScreen(It.IsAny<float>(), It.IsAny<float>())).Returns(false);
            _entityMock.SetupGet(e => e.Screen).Returns(screen.Object);

            _entityMock.SetupGet(e => e.Name).Returns("Player");

            var removed = false;
            _entityMock.Setup(e => e.Remove()).Callback(() => { removed = true; });

            var container = new FakeGameplayContainer();
            _position.Start(container);

            container.Tick();

            Assert.IsFalse(removed);
        }
    }
}
