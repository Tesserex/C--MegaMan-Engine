using MegaMan.Common;

namespace MegaMan.Engine.StateMachine
{
    public interface IStateMachine
    {
        void RemoveAllEndHandlers();
        void StopAllHandlers();
        void StopAllInput();
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
