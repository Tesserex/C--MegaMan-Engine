namespace MegaMan.Engine
{
    public class Const
    {
        // ===================== ENGINE ===================
        /// <summary>
        /// Defines the enforced framerate for the game.
        /// </summary>
        public const int FPS = 60;

        /// <summary>
        /// Images are stupid, and require resolution even for pixel to pixel conversion.
        /// </summary>
        public const int Resolution = 72;

        public const float PixelEpsilon = 0.001f;

        // ==================== GAME =====================
        public const int PixelsAcross = 256;
        public const int PixelsDown = 224;

        /// <summary>
        /// Defines the speed of screen scrolling, in pixels per frame.
        /// </summary>
        public const float ScrollSpeed = 3.0f;

        /// <summary>
        /// How long the map hangs around after the player dies before resetting.
        /// </summary>
        public const int MapDeadFrames = 240;

        /// <summary>
        /// Defines how many pixels from the edge the player will be placed after a screen scroll.
        /// </summary>
        public const int PlayerScrollOffset = 4;

        /// <summary>
        /// Defines how close to the edge, in pixels, the player must be to trigger a scroll.
        /// Must be smaller than PlayerScrollOffset or else it will scroll back and forth forever.
        /// </summary>
        public const int PlayerScrollTrigger = 0;

        /// <summary>
        /// Terminal velocity for falling entities. Eventually this may be controlled in the
        /// entity definitions, but it's useful to set a global constraint so at least everyone agrees.
        /// </summary>
        public const float TerminalVel = 12;
    }
}
