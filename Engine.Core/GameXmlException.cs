using System;

namespace MegaMan.Engine
{
    public class GameRunException : Exception
    {
        public GameRunException(string message)
            : base(message)
        {
        }
    }
}
