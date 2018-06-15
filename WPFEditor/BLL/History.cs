using System;
using System.Collections.Generic;

namespace MegaMan.Editor.Bll
{
    public class History
    {
        public List<IUndoableAction> Actions { get; private set; }
        public int CurrentIndex { get; private set; }

        public event EventHandler Updated;

        public History()
        {
            CurrentIndex = -1;
            Actions = new List<IUndoableAction>();
        }

        public void Push(IUndoableAction action)
        {
            CurrentIndex += 1;
            while (Actions.Count > CurrentIndex)
            {
                Actions.RemoveAt(CurrentIndex);
            }

            Actions.Add(action);

            if (Updated != null)
                Updated(this, new EventArgs());
        }

        public void Undo()
        {
            if (CurrentIndex >= 0)
            {
                var action = Actions[CurrentIndex];
                CurrentIndex -= 1;
                action.Reverse().Execute();

                if (Updated != null)
                    Updated(this, new EventArgs());
            }
        }

        public void Redo()
        {
            if (CurrentIndex < Actions.Count - 1)
            {
                CurrentIndex += 1;
                var action = Actions[CurrentIndex];
                action.Execute();

                if (Updated != null)
                    Updated(this, new EventArgs());
            }
        }

        public void MoveTo(int index)
        {
            if (index > CurrentIndex)
            {
                while (CurrentIndex < index && CurrentIndex < Actions.Count - 1)
                {
                    Redo();
                }
            }
            else if (index < CurrentIndex)
            {
                while (CurrentIndex > index && CurrentIndex >= 0)
                {
                    Undo();
                }
            }
        }
    }
}