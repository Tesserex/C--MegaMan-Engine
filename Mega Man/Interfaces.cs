using System.Drawing;
using System.Collections.Generic;
using System;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public interface IHandleGameEvents
    {
        void StartHandler();
        void StopHandler();
        void PauseHandler();
        void ResumeHandler();
        void StopDrawing();
        void StartDrawing();

        event Action<HandlerTransfer> End;
    }

    public interface IEntityContainer
    {
        int TileSize { get; }
        float OffsetX { get; }
        float OffsetY { get; }
        MapSquare SquareAt(int x, int y);
        IEnumerable<MapSquare> Tiles { get; }
        MegaMan.Common.Tile TileAt(int tx, int ty);
        void AddEntity(GameEntity entity);
        IEnumerable<GameEntity> GetEntities(string name);
        void ClearEntities();
        bool IsOnScreen(float x, float y);
    }

    public interface IGameplayContainer : IHandleGameEvents
    {
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
