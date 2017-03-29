using System;
using System.Collections.Generic;
using MegaMan.Common;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public interface ITiledScreen
    {
        int TileSize { get; }
        float OffsetX { get; }
        float OffsetY { get; }
        MapSquare SquareAt(float px, float py);
        IEnumerable<MapSquare> Tiles { get; }
        MegaMan.Common.Tile TileAt(float px, float py);
        bool IsOnScreen(float x, float y);
    }

    public interface IGameplayContainer
    {
        IEntityPool Entities { get; }

        ITiledScreen Screen { get; }

        float Gravity { get; }

        bool IsGravityFlipped { get; set; }

        bool DidGravityFlipPreviousFrame { get; }

        event Action GameThink;

        event Action GameAct;

        event Action GameReact;

        event Action GameCleanup;

        event GameRenderEventHandler Draw;

        event Action<HandlerTransfer> End;

        void StartHandler(IEntityPool entityPool);
        void StopHandler();
        void PauseHandler();
        void ResumeHandler();
        void StopDrawing();
        void StartDrawing();
        void StopInput();
        void StartInput();
    }

    public abstract class Component
    {
        public IEntity Parent { get; set; }

        public abstract Component Clone();

        public abstract void Start(IGameplayContainer container);

        public abstract void Stop(IGameplayContainer container);

        public abstract void Message(IGameMessage msg);

        protected abstract void Update();

        public abstract void RegisterDependencies(Component component);
    }
}
