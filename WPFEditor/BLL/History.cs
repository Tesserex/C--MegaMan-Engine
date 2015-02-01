using System.Collections.Generic;

namespace MegaMan.Editor.Bll
{
    /* *
    * History - Saves previous actions for "undo"/"redo" functionality
    * */
    public class History
    {
        private readonly List<IUndoableAction> stack;
        private int currentAction;

        public History()
        {
            currentAction = -1;
            stack = new List<IUndoableAction>();
        }

        public void Push(IUndoableAction action)
        {
            currentAction += 1;
            stack.Insert(currentAction, action);
        }

        public void Undo()
        {
            if (currentAction >= 0)
            {
                var action = stack[currentAction];
                currentAction -= 1;
                action.Reverse().Execute();
            }
        }

        public void Redo()
        {
            if (currentAction < stack.Count - 1)
            {
                currentAction += 1;
                var action = stack[currentAction];
                action.Execute();
            }
        }
    }
}