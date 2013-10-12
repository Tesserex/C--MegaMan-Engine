using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public interface IStateMachine
    {
        void RemoveAllEndHandlers();
        void StopAllHandlers();
        void Push(IGameplayContainer handler);
        void StartHandler(HandlerTransfer _transfer);

        void PauseTopOfStack();

        void ResumeTopOfStack();

        void PauseDrawingTopOfStack();

        void ResumeDrawingTopOfStack();

        void FinalizeTopHandler();

        void RemoveTopHandler();
    }
}
