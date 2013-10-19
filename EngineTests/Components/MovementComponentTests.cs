using MegaMan.Common;
using MegaMan.Engine.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Tests.Components
{
    [TestClass]
    public class MovementComponentTests
    {
        private MovementComponent _movement;
        private Mock<IEntity> _entityMock;
        private FakeGameplayContainer _container;
        private Mock<ITiledScreen> _screen;

        [TestInitialize]
        public void Initialize()
        {
            _entityMock = new Mock<IEntity>();
            _movement = new MovementComponent();
            _movement.Parent = _entityMock.Object;
            _container = new FakeGameplayContainer();
            _screen = new Mock<ITiledScreen>();
            _entityMock.SetupGet(e => e.Screen).Returns(_screen.Object);
        }

        [TestMethod]
        public void Movement_DefaultsToRight()
        {
            _movement.Start(_container);
            Assert.AreEqual(Direction.Right, _movement.Direction);
        }

        [TestMethod]
        public void Movement_DoesNotAcceptVelocityChangeWhenPaused()
        {
            _screen.Setup(s => s.TileAt(It.IsAny<int>(), It.IsAny<int>())).Returns((Tile)null);

            _movement.Start(_container);
            Assert.AreEqual(0, _movement.VelocityX);

            _entityMock.SetupGet(x => x.Paused).Returns(true);
            _movement.VelocityX = 5;
            _container.Tick();
            Assert.AreEqual(0, _movement.VelocityX);

            _entityMock.SetupGet(x => x.Paused).Returns(false);
            _movement.CanMove = false;
            _movement.VelocityX = 5;
            _container.Tick();
            Assert.AreEqual(0, _movement.VelocityX);
        }

        [TestMethod]
        public void Movement_AcceptsVelocityChange()
        {
            _movement.Start(_container);
            Assert.AreEqual(0, _movement.VelocityX);

            _entityMock.Object.Paused = true;
            _movement.VelocityX = 5;
            Assert.AreEqual(0, _movement.VelocityX);

            _entityMock.Object.Paused = false;
            _movement.CanMove = false;
            _movement.VelocityX = 5;
            Assert.AreEqual(0, _movement.VelocityX);
        }

        [TestMethod]
        public void Movement_ChangesDirection()
        {
            _movement.VelocityX = -5;
            Assert.AreEqual(Direction.Left, _movement.Direction);

            _movement.VelocityX = 5;
            Assert.AreEqual(Direction.Right, _movement.Direction);
        }
    }
}
