using System;

namespace MegaMan.Engine.Core
{
    public class GameRunException : Exception
    {
        public GameRunException(string message)
            : base(message)
        {
        }
    }
}
