using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public class PopStateTransition : IStateMachineCommand
    {
        private HandlerTransfer _transfer;

        public PopStateTransition(HandlerTransfer transfer)
        {
            _transfer = transfer;
        }

        public void Apply(IStateMachine stateMachine)
        {
            if (_transfer.Fade)
                PopFade(stateMachine);
            else
                PopImmediate(stateMachine);
        }

        private void PopFade(IStateMachine stateMachine)
        {
            stateMachine.FinalizeTopHandler();

            Engine.Instance.FadeTransition(() =>
            {
                stateMachine.RemoveTopHandler();
                stateMachine.ResumeDrawingTopOfStack();
            },
            () =>
            {
                stateMachine.ResumeTopOfStack();
            });
        }

        private void PopImmediate(IStateMachine stateMachine)
        {
            stateMachine.RemoveTopHandler();
            stateMachine.ResumeTopOfStack();
        }
    }
}
