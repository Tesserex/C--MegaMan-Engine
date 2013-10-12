using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public class EndStateTransition : IStateMachineCommand
    {
        private HandlerTransfer _transfer;

        public EndStateTransition(HandlerTransfer transfer)
        {
            _transfer = transfer;
        }

        public void Apply(IStateMachine stateMachine)
        {
            stateMachine.RemoveAllEndHandlers();

            if (_transfer.Fade)
                Engine.Instance.FadeTransition(() => EmptyStackAndStart(stateMachine));
            else
                EmptyStackAndStart(stateMachine);
        }

        private void EmptyStackAndStart(IStateMachine stateMachine)
        {
            stateMachine.StopAllHandlers();
            stateMachine.StartHandler(_transfer);
        }
    }
}
