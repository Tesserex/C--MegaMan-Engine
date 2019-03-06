using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.Engine.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MegaMan.Engine.Tests.Components
{
    [TestClass]
    public class CollisionBoxTests
    {
        private Mock<IEntity> _entityMock;
        private FakeGameplayContainer _container;
        private CollisionComponent _collision;
        private Mock<ITiledScreen> _screen;

        [TestInitialize]
        public void Initialize()
        {
            _entityMock = new Mock<IEntity>();
            _container = new FakeGameplayContainer();
            _screen = new Mock<ITiledScreen>();
            _entityMock.SetupGet(e => e.Screen).Returns(_screen.Object);
            _entityMock.SetupGet(e => e.Container).Returns(_container);
            _entityMock.SetupGet(e => e.IsGravitySensitive).Returns(false);
            _container.IsGravityFlipped = false;

            _collision = new CollisionComponent();
            _collision.Parent = _entityMock.Object;
        }

        [TestMethod]
        public void CollisionBox_TooFarLeft_NoCollision()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(-15, 0, 10, 10));

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsFalse(didCollide);
            Assert.AreEqual(PointF.Empty, collisionPoint);
        }

        [TestMethod]
        public void CollisionBox_TooFarRight_NoCollision()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(15, 0, 10, 10));

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsFalse(didCollide);
            Assert.AreEqual(PointF.Empty, collisionPoint);
        }

        [TestMethod]
        public void CollisionBox_TooFarAbove_NoCollision()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(0, -15, 10, 10));

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsFalse(didCollide);
            Assert.AreEqual(PointF.Empty, collisionPoint);
        }

        [TestMethod]
        public void CollisionBox_TooFarBelow_NoCollision()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(0, 15, 10, 10));

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsFalse(didCollide);
            Assert.AreEqual(PointF.Empty, collisionPoint);
        }

        [TestMethod]
        public void CollisionBox_TouchingButNotOverapping_Collision()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(10, 0, 10, 10)); // up against the right edge

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsTrue(didCollide);
            Assert.AreEqual(PointF.Empty, collisionPoint);
        }

        [TestMethod]
        public void CollisionBox_Overapping_DefaultVerticalApproach()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var square = GetMapSquare(new Rectangle(8, 4, 10, 10));

            var collisionPoint = Point.Empty;

            var didCollide = box.EnvironmentCollisions(Point.Empty, square, ref collisionPoint);

            Assert.IsTrue(didCollide);
            // pushes the block out vertically
            Assert.AreEqual(new Point(0, -6), collisionPoint);
        }

        [TestMethod]
        public void IntersectionOffset_HorizontalApproach_PushOutHorizontally()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var tileBox = new Rectangle(8, 4, 10, 10);

            var collisionPoint = box.GetIntersectionOffset(tileBox, box.BoxAt(Point.Empty), 1.0f, 0, false, false);

            // pushes the block out horizontally
            Assert.AreEqual(-2, collisionPoint.X);
            Assert.AreEqual(0, collisionPoint.Y);
        }

        [TestMethod]
        public void IntersectionOffset_DownOnly_GoingUp_NoPushOut()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var tileBox = new Rectangle(8, 4, 10, 10);

            var collisionPoint = box.GetIntersectionOffset(tileBox, box.BoxAt(Point.Empty), 0, -1, false, true);

            Assert.AreEqual(Point.Empty, collisionPoint);
        }

        [TestMethod]
        public void IntersectionOffset_UpOnly_GoingDown_NoPushOut()
        {
            var box = new CollisionBox(0, 0, 10, 10);
            box.SetParent(_collision);

            var tileBox = new Rectangle(8, -4, 10, 10);

            var collisionPoint = box.GetIntersectionOffset(tileBox, box.BoxAt(Point.Empty), 0, 1, true, false);

            Assert.AreEqual(Point.Empty, collisionPoint);
        }

        private IMapSquare GetMapSquare(Rectangle rectangle)
        {
            var square = new Mock<IMapSquare>();
            square.SetupGet(s => s.BlockBox).Returns(rectangle);
            square.SetupGet(s => s.Properties).Returns(TileProperties.Default);

            return square.Object;
        }
    }
}
