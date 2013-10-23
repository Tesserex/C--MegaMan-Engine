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
        private PositionComponent _position;

        [TestInitialize]
        public void Initialize()
        {
            _entityMock = new Mock<IEntity>();
            _movement = new MovementComponent();
            _movement.Parent = _entityMock.Object;
            _container = new FakeGameplayContainer();
            _screen = new Mock<ITiledScreen>();
            _entityMock.SetupGet(e => e.Screen).Returns(_screen.Object);
            _entityMock.SetupGet(e => e.Container).Returns(_container);

            _position = new PositionComponent();
            _movement.RegisterDependencies(_position);
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

            _movement.VelocityX = 5;
            _container.Tick();
            Assert.AreEqual(5, _movement.VelocityX);
        }

        [TestMethod]
        public void Movement_ChangesDirection()
        {
            _movement.VelocityX = -5;
            Assert.AreEqual(Direction.Left, _movement.Direction);

            _movement.VelocityX = 5;
            Assert.AreEqual(Direction.Right, _movement.Direction);
        }

        [TestMethod]
        public void Movement_GravityCausesFalling()
        {
            _movement.Start(_container);
            Assert.AreEqual(0, _movement.VelocityX);
            Assert.AreEqual(0, _movement.VelocityY);
            _movement.Flying = false;

            _container.Tick();
            Assert.AreEqual(_container.Gravity, _movement.VelocityY);
            Assert.AreEqual(0, _movement.VelocityX);
        }

        [TestMethod]
        public void Movement_TerminalVelocity()
        {
            _movement.Start(_container);
            _movement.Flying = false;

            while (_movement.VelocityY < Const.TerminalVel)
                _container.Tick();

            Assert.IsTrue(_movement.VelocityY == Const.TerminalVel);
            _container.Tick();
            Assert.IsTrue(_movement.VelocityY == Const.TerminalVel);
        }

        [TestMethod]
        public void Movement_FlippedGravity()
        {
            _container.IsGravityFlipped = true;
            _movement.Start(_container);
            _movement.Flying = false;

            _container.Tick();
            Assert.IsTrue(_movement.VelocityY < 0);

            while (_movement.VelocityY > -Const.TerminalVel)
                _container.Tick();

            Assert.IsTrue(_movement.VelocityY == -Const.TerminalVel);
            _container.Tick();
            Assert.IsTrue(_movement.VelocityY == -Const.TerminalVel);
        }

        [TestMethod]
        public void Movement_FlyingUnaffectedByGravity()
        {
            _movement.Start(_container);
            _movement.Flying = true;

            _container.Tick();

            Assert.AreEqual(0, _movement.VelocityY);
        }

        [TestMethod]
        public void Movement_Push()
        {
            _movement.Start(_container);
            _movement.Flying = true;

            _movement.PushX(3.5f);
            _movement.PushY(2.6f);
            _container.Tick();

            Assert.AreEqual(3.5f, _position.Position.X, 0.0001);
            Assert.AreEqual(2.6f, _position.Position.Y, 0.0001);
            Assert.AreEqual(0, _movement.VelocityX);
            Assert.AreEqual(0, _movement.VelocityY);

            _movement.PushX(-1.3f);
            _movement.PushY(-0.8f);
            _container.Tick();

            Assert.AreEqual(2.2f, _position.Position.X, 0.0001);
            Assert.AreEqual(1.8f, _position.Position.Y, 0.0001);
            Assert.AreEqual(0, _movement.VelocityX);
            Assert.AreEqual(0, _movement.VelocityY);
        }

        [TestMethod]
        public void Movement_OppositePushes()
        {
            _movement.Start(_container);
            _movement.Flying = true;

            _movement.PushX(3.5f);
            _movement.PushY(2.6f);
            _movement.PushX(-4.7f);
            _movement.PushY(-1.1f);
            _container.Tick();

            Assert.AreEqual(-1.2f, _position.Position.X, 0.0001);
            Assert.AreEqual(1.5f, _position.Position.Y, 0.0001);
            Assert.AreEqual(0, _movement.VelocityX);
            Assert.AreEqual(0, _movement.VelocityY);
        }

        [TestMethod]
        public void Movement_ComplimentaryPushes()
        {
            _movement.Start(_container);
            _movement.Flying = true;

            _movement.PushX(3.5f);
            _movement.PushY(-2.6f);
            _movement.PushX(4.7f);
            _movement.PushY(-1.1f);
            _container.Tick();

            Assert.AreEqual(4.7f, _position.Position.X, 0.0001);
            Assert.AreEqual(-2.6f, _position.Position.Y, 0.0001);
            Assert.AreEqual(0, _movement.VelocityX);
            Assert.AreEqual(0, _movement.VelocityY);
        }

        [TestMethod]
        public void Movement_Drag()
        {
            // drag hinders acceleration

            _movement.Start(_container);
            _movement.Flying = true;

            _movement.DragX(0.8f);
            _movement.VelocityX = 10;
            _container.Tick();

            Assert.AreEqual(8, _position.Position.X);
            Assert.AreEqual(8, _movement.VelocityX);

            _movement.DragX(0.5f);
            _movement.VelocityX = -10;
            _container.Tick();

            // velocity intended to go from 8 to -10, a change of 18.
            // drag of 0.5 cuts that down to a change of 9
            Assert.AreEqual(7, _position.Position.X);
            Assert.AreEqual(-1, _movement.VelocityX);
        }

        [TestMethod]
        public void Movement_Resist()
        {
            // resist slows you down after movement has occured

            _movement.Start(_container);
            _movement.Flying = true;

            _movement.ResistX(0.8f);
            _movement.VelocityX = 10;
            _container.Tick();

            Assert.AreEqual(10, _position.Position.X);
            Assert.AreEqual(8, _movement.VelocityX);

            _movement.ResistX(0.5f);
            _container.Tick();

            Assert.AreEqual(18, _position.Position.X);
            Assert.AreEqual(4, _movement.VelocityX);
        }
    }
}
