using MegaMan.Common;

namespace MegaMan.Engine.StateMachine
{
    public class PushStateTransition : IStateMachineCommand
    {
        private HandlerTransfer _transfer;

        public PushStateTransition(HandlerTransfer transfer)
        {
            _transfer = transfer;
        }

        public void Apply(IStateMachine stateMachine)
        {
            if (_transfer.Fade)
                PushFade(stateMachine);
            else
                PushImmediate(stateMachine);
        }

        private void PushFade(IStateMachine stateMachine)
        {
            if (_transfer.Pause)
            {
                stateMachine.PauseTopOfStack();
            }

            Engine.Instance.FadeTransition(() =>
            {
                if (_transfer.Pause)
                {
                    stateMachine.PauseDrawingTopOfStack();
                }
                stateMachine.StartHandler(_transfer);
                stateMachine.PauseTopOfStack();
            },
            () =>
            {
                stateMachine.ResumeTopOfStack();
            });
        }

        private void PushImmediate(IStateMachine stateMachine)
        {
            if (_transfer.Pause)
            {
                stateMachine.PauseTopOfStack();
                stateMachine.PauseDrawingTopOfStack();
            }
            stateMachine.StartHandler(_transfer);
        }
    }
}
