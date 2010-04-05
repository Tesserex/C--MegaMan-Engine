using System.Windows.Forms;

public class Const
{
    // ===================== ENGINE ===================
    /// <summary>
    /// Defines the enforced framerate for the game.
    /// </summary>
    public const float FPS = 60.0f;

    /// <summary>
    /// Images are stupid, and require resolution even for pixel to pixel conversion.
    /// </summary>
    public const int Resolution = 72;

    public const int MusicVolume = 500;
    public const int SoundVolume = 1000;

    public const float PixelEpsilon = 0.001f;
    public const float CollisionEps = 0.5f;

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
    public const int PlayerBossScrollTrigger = 8;

    public const float TerminalVel = 12;
}

public enum Direction : int
{
    Down,
    Up,
    Left,
    Right
}

public enum Axis : int
{
    X = 1,
    Y = 2,
    Both = 3
}

public enum JoinType : int
{
    Horizontal = 1,
    Vertical = 2
}

public enum EntityState : int
{
    Standing,
    Walking,
    Air,
    BeginWalk,
    Climbing,
    Sliding,
    ClimbTop,
    Teleport,
    Hurt
}

public enum GameInput : int
{
    Up,
    Down,
    Left,
    Right,
    Jump,
    Shoot,
    Start,
    Select,
    None
}

public class GameInputKeys : System.Collections.IEnumerable
{
    public static Keys Right = Keys.Right;
    public static Keys Left = Keys.Left;
    public static Keys Up = Keys.Up;
    public static Keys Down = Keys.Down;
    public static Keys Jump = Keys.O;
    public static Keys Shoot = Keys.A;
    public static Keys Start = Keys.Enter;
    public static Keys Select = Keys.Shift;

    private static GameInputKeys instance;
    public static GameInputKeys Instance
    {
        get
        {
            if (instance == null) instance = new GameInputKeys();
            return instance;
        }
    }

    #region IEnumerable Members

    public System.Collections.IEnumerator GetEnumerator()
    {
        yield return Right;
        yield return Left;
        yield return Up;
        yield return Down;
        yield return Jump;
        yield return Shoot;
        yield return Start;
        yield return Select;
    }

    #endregion
}