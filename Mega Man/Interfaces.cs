using System.Drawing;
using System.Collections.Generic;
using System;

namespace MegaMan.Engine
{
    public interface IHandleGameEvents
    {
        void StartHandler();
        void StopHandler();

        event Action End;
    }

    public interface IScreenInformation
    {
        int TileSize { get; }
        float OffsetX { get; }
        float OffsetY { get; }
        MapSquare SquareAt(int x, int y);
        IEnumerable<MapSquare> Tiles { get; }
        MegaMan.Common.Tile TileAt(int tx, int ty);
        void AddSpawnedEntity(GameEntity entity);
        bool IsOnScreen(float x, float y);
    }

    public interface IGameplayContainer : IHandleGameEvents
    {
        GameEntity Player { get; }

        event Action GameThink;

        event Action GameAct;

        event Action GameReact;

        event Action GameCleanup;
    }

    public abstract class Component
    {
        public GameEntity Parent { get; set; }

        public abstract Component Clone();

        public abstract void Start();

        public abstract void Stop();

        public abstract void Message(IGameMessage msg);

        protected abstract void Update();

        public abstract void RegisterDependencies(Component component);

        public abstract void LoadXml(System.Xml.Linq.XElement xmlNode);
    }
}
