using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public class GamePlay : IGameplayContainer
    {
        public GameEntity Player { get; set; }

        public IEntityContainer Entities { get; set; }

        /// <summary>
        /// This is the first phase of game logic, but comes after the GameLogicTick event.
        /// During this phase, entities should "think" - decide what they want to do this frame.
        /// </summary>
        public event Action GameThink;

        /// <summary>
        /// During this phase, which comes between GameThink and GameReact, entities should carry out
        /// the actions decided during the thinking phase. Mainly used for movement.
        /// </summary>
        public event Action GameAct;

        /// <summary>
        /// This is the last logic phase, in which entities should react to the actions of other
        /// entities on the screen. Primarily used for collision detection and response.
        /// </summary>
        public event Action GameReact;

        /// <summary>
        /// The final phase before rendering. Used to delete entities,
        /// so please do not enumerate through entity collections during this phase. If you must,
        /// then make a copy first.
        /// </summary>
        public event Action GameCleanup;

        public event Action<HandlerTransfer> End;

        public void StartHandler()
        {
            ResumeHandler();
        }

        public void PauseHandler()
        {
            Engine.Instance.GameLogicTick -= GameTick;
        }

        public void ResumeHandler()
        {
            Engine.Instance.GameLogicTick += GameTick;
        }

        public void StopHandler()
        {
            PauseHandler();
        }

        public void StopDrawing() { }

        public void StartDrawing() { }

        public void EndPlay()
        {
            if (End != null) End(null);
        }

        private void GameTick(GameTickEventArgs e)
        {
            if (GameThink != null) GameThink();
            if (GameAct != null) GameAct();
            if (GameReact != null) GameReact();
            if (GameCleanup != null) GameCleanup();
        }
    }
}
