namespace MegaMan.Engine.StateMachine
{
    public interface IStateMachineCommand
    {
        void Apply(IStateMachine stateMachine);
    }
}
