namespace MegaMan.Engine.Core.StateMachine
{
    public interface IStateMachineCommand
    {
        void Apply(IStateMachine stateMachine);
    }
}
