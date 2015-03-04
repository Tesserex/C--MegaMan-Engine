using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MegaMan.Engine.Tests.Components
{
    [TestClass]
    public class CollisionComponentTests : ComponentTestBase
    {
        private CollisionComponent _collision;
        private MovementComponent _movement;
        private PositionComponent _position;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            _collision = new CollisionComponent();
            _collision.Parent = EntityMock.Object;
            _movement = new MovementComponent();
            _movement.Parent = EntityMock.Object;
            _position = new PositionComponent();
            _position.Parent = EntityMock.Object;
            _movement.RegisterDependencies(_position);
            _movement.RegisterDependencies(_collision);
            _collision.RegisterDependencies(_position);
            _collision.RegisterDependencies(_movement);

            _position.Start(Container);
            _movement.Start(Container);
            _collision.Start(Container);
        }

        [TestMethod]
        public void Collision_FloatingPointError_StillWorks()
        {
            var hitbox = new CollisionBox(-7.5f, -9f, 15, 21);
            hitbox.Name = "Test";
            hitbox.Environment = true;
            hitbox.PushAway = true;
            _position.SetPosition(5f, 115.999992f);
            _collision.AddBox(hitbox);
            _collision.Message(new HitBoxMessage(EntityMock.Object, new List<CollisionBox>(), new HashSet<string>() { "Test" }, false));

            var layer = new Mock<IScreenLayer>();
            layer.SetupGet(l => l.Stage).Returns(Container);

            var tileset = new Common.Tileset() { TileSize = 16 };
            var backTile = new Common.Tile(1, new Common.TileSprite(tileset));
            var blankSquare = new MapSquare(layer.Object, backTile, 0, 0, 16);

            Screen.Setup(s => s.SquareAt(It.IsAny<float>(), It.IsAny<float>())).Returns(blankSquare);


            var blockTile = new Common.Tile(2, new Common.TileSprite(tileset));
            blockTile.Properties = new Common.TileProperties() { Blocking = true };
            var blockSquare = new MapSquare(layer.Object, blockTile, 0, 8, 16);

            Screen.Setup(s => s.SquareAt(It.IsInRange<float>(0, 16, Range.Inclusive), It.IsInRange<float>(128, 144, Range.Inclusive))).Returns(blockSquare);

            Assert.IsFalse(_collision.BlockBottom);

            Container.Tick();

            Assert.IsTrue(_collision.BlockBottom);

            Container.Tick();

            Assert.IsTrue(_collision.BlockBottom);

            Container.Tick();

            Assert.IsTrue(_collision.BlockBottom);
        }
    }
}
