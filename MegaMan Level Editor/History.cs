using System.Collections.Generic;

namespace MegaMan.LevelEditor
{
    /* *
    * History - Saves previous actions for "undo"/"redo" functionality
    * */
    public class History
    {
        public readonly List<HistoryAction> stack;
        public int currentAction;

        public History()
        {
            currentAction = -1;
            stack = new List<HistoryAction>();
        }

        /*
         * push - Adds a new action to the stack. If the stack already has actions after the current,
         * why not insert them? Even better than photoshop!
         * 
         * Ex: 
         * 
         * Suppose the current history looks like this:
         * 
         *   (1,4,Wall) <- currentAction
         *   (1,3,Wall)
         *   (1,2,Wall)
         *   (1,1,Wall)
         *   
         * Then we execute two undos to get
         * 
         *   (1,4,Wall)
         *   (1,3,Wall)
         *   (1,2,Wall) <- currentAction
         *   (1,1,Wall)
         *   
         * So what happens if we push another onto the stack? Try inserting!
         *
         *   (1,4,Wall)
         *   (1,3,Wall)
         *   (1,3,Enemy) <- currentAction
         *   (1,2,Wall)  
         *   (1,1,Wall)
         *   
         * The only current issue with this is that the action ahead of current doesn't have the correct
         * "previous" brush, so undoing it won't result in the correct state, you have to undo twice
         * and then redo to get back to it. This can be corrected.
         * 
         * */
        public void Push(HistoryAction action)
        {
            currentAction += 1;
            stack.Insert(currentAction, action);
            UpdateHistoryForm();
        }

        public HistoryAction Undo()
        {
            if (currentAction >= 0)
            {
                var action = stack[currentAction];
                currentAction -= 1;                  
                UpdateHistoryForm();
                return action.Reverse();
            }
            
            return null;
        }

        public HistoryAction Redo()
        {
            if (currentAction < stack.Count - 1)
            {
                currentAction += 1;
                var action = stack[currentAction];
                UpdateHistoryForm();
                return action;
            }
            
            return null;
        }

        private void UpdateHistoryForm()
        {
            MainForm.Instance.historyForm.UpdateHistory(this);
        }
    }
}