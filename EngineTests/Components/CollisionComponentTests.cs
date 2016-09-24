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
    }
}
