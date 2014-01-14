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
    public abstract class ComponentTestBase
    {
        protected Mock<IEntity> EntityMock { get; private set; }
        protected FakeGameplayContainer Container { get; private set; }
        protected Mock<ITiledScreen> Screen { get; private set; }
        protected Mock<IEntityPool> EntityPool { get; private set; }
        protected List<IEntity> AllEntities { get; private set; }

        [TestInitialize]
        public virtual void Initialize()
        {
            AllEntities = new List<IEntity>();

            EntityMock = new Mock<IEntity>();
            Container = new FakeGameplayContainer();
            Screen = new Mock<ITiledScreen>();
            EntityPool = new Mock<IEntityPool>();

            Screen.SetupGet(s => s.TileSize).Returns(16);
            EntityPool.Setup(p => p.GetAll()).Returns(AllEntities);
            EntityMock.SetupGet(e => e.Screen).Returns(Screen.Object);
            EntityMock.SetupGet(e => e.Container).Returns(Container);
            EntityMock.SetupGet(e => e.Entities).Returns(EntityPool.Object);

            AllEntities.Add(EntityMock.Object);

            Container.StartHandler(EntityPool.Object);
        }
    }
}
