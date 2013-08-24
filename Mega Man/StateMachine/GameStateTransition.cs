using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public abstract class GameStateTransition
    {
        public abstract void Execute();

        public static GameStateTransition ForHandler(HandlerTransfer transfer)
        {

        }
    }
}
