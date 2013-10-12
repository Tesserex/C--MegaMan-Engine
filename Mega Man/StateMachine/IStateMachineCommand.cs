using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine.StateMachine
{
    public interface IStateMachineCommand
    {
        void Apply(IStateMachine stateMachine);
    }
}
