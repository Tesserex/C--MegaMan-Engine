using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.Tests
{
    class FakeGameplayContainer : IGameplayContainer
    {
        public Entities.IEntityPool Entities
        {
            get;
            set;
        }

        public ITiledScreen Screen
        {
            get;
            set;
        }

        public bool IsGravityFlipped { get; set; }

        public event Action GameThink;

        public event Action GameAct;

        public event Action GameReact;

        public event Action GameCleanup;

        public event GameRenderEventHandler Draw;

        public event Action<Common.HandlerTransfer> End;

        public void Tick()
        {
            if (GameThink != null) GameThink();
            if (GameAct != null) GameAct();
            if (GameReact != null) GameReact();
            if (GameCleanup != null) GameCleanup();
        }

        public void StartHandler(Entities.IEntityPool entityPool)
        {
            Entities = entityPool;
        }

        public void StopHandler()
        {
        }

        public void PauseHandler()
        {
        }

        public void ResumeHandler()
        {
        }

        public void StopDrawing()
        {
        }

        public void StartDrawing()
        {
        }
    }
}
