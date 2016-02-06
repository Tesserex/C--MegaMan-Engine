using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Geometry;
using MegaMan.Common.Rendering;

namespace MegaMan.Common
{
    /// <summary>
    /// Represents a 2D rectangular image sprite, which can be animated.
    /// </summary>
    public class Sprite : ICollection<SpriteFrame>
    {
        private List<SpriteFrame> frames;
        private int currentFrame;
        private int lastFrameTime;

        private int width;
        private int height;

        private IResourceImage texture;

        /// <summary>
        /// Gets or sets the direction in which to play the sprite animation.
        /// </summary>
        public AnimationDirection AnimDirection { get; set; }

        /// <summary>
        /// Gets or sets the animation style.
        /// </summary>
        public AnimationStyle AnimStyle { get; set; }

        /// <summary>
        /// Gets or sets the point representing the drawing offset for the sprite.
        /// </summary>
        public Point HotSpot { get; set; }

        /// <summary>
        /// Gets a rectangle representing the box surrounding the sprite.
        /// </summary>
        public RectangleF BoundBox { get; protected set; }

        /// <summary>
        /// Gets or sets the height of the sprite.
        /// </summary>
        public virtual int Height
        {
            get { return height; }
            set
            {
                if (height == value)
                    return;

                height = value;
                ResizeFrames();
            }
        }

        /// <summary>
        /// Gets or sets the width of the sprite.
        /// </summary>
        public virtual int Width
        {
            get { return width; }
            set
            {
                if (width == value)
                    return;

                width = value;
                ResizeFrames();
            }
        }

        /// <summary>
        /// Gets the number of frames in the sprite animation.
        /// </summary>
        public int Count { get { return frames.Count; } }

        public int CurrentIndex { get { return this.currentFrame; } set { this.currentFrame = value; } }

        public SpriteFrame CurrentFrame
        {
            get
            {
                return frames[currentFrame];
            }
        }

        public int FrameTime { get { return this.lastFrameTime; } set { this.lastFrameTime = value; } }

        public string Name { get; set; }
        public string Part { get; set; }
        public string PaletteName { get; set; }

        /// <summary>
        /// Gets whether or not the sprite animation is currently playing.
        /// </summary>
        public bool Playing { get; protected set; }

        public bool HorizontalFlip { get; set; }

        public bool VerticalFlip { get; set; }

        public bool Visible { get; set; }

        public int Layer { get; set; }

        public virtual FilePath SheetPath { get; set; }
        public virtual string SheetPathRelative { get { return SheetPath != null ? SheetPath.Relative : null; } }

        /// <summary>
        /// If this is true, it means the sprite sheet is backwards - it's facing left instead of right,
        /// so we have to flip all drawing of this sprite to match proper orientation rules.
        /// </summary>
        public bool Reversed { get; set; }

        public event Action Stopped;

        /// <summary>
        /// Creates a new Sprite object with the given width and height, and no frames.
        /// </summary>
        public Sprite(int width, int height)
        {
            this.Height = height;
            this.Width = width;
            frames = new List<SpriteFrame>();

            this.currentFrame = 0;
            this.lastFrameTime = 0;
            this.HotSpot = new Point(0, 0);
            this.BoundBox = new RectangleF(0, 0, width, height);
            this.Playing = false;
            this.Visible = true;
            this.AnimDirection = AnimationDirection.Forward;
            this.AnimStyle = AnimationStyle.Repeat;
        }

        public Sprite(Sprite copy)
        {
            this.Name = copy.Name;
            this.Part = copy.Part;
            this.PaletteName = copy.PaletteName;

            this.Height = copy.Height;
            this.Width = copy.Width;
            this.tickable = copy.tickable;
            this.frames = copy.frames;
            this.currentFrame = 0;
            this.lastFrameTime = 0;
            this.HotSpot = new Point(copy.HotSpot.X, copy.HotSpot.Y);
            this.BoundBox = new RectangleF(0, 0, copy.Width, copy.Height);
            this.Playing = false;
            this.Visible = true;
            this.AnimDirection = copy.AnimDirection;
            this.AnimStyle = copy.AnimStyle;
            this.Layer = copy.Layer;

            this.Reversed = copy.Reversed;
            if (copy.SheetPath != null)
            {
                this.SheetPath = FilePath.FromRelative(copy.SheetPath.Relative, copy.SheetPath.BasePath);
            }
        }

        public SpriteFrame this[int index]
        {
            get { return frames[index]; }
        }

        /// <summary>
        /// Adds a frame with no image or duration
        /// </summary>
        public void AddFrame()
        {
            AddFrame(0, 0, 0);
        }

        /// <summary>
        /// Adds a frame to the sprite with the given coordinates and duration.
        /// </summary>
        /// <param name="tilesheet">The image from which to retreive the frame.</param>
        /// <param name="x">The x-coordinate, on the tilesheet, of the top-left corner of the frame image.</param>
        /// <param name="y">The y-coordinate, on the tilesheet, of the top-left corner of the frame image.</param>
        /// <param name="duration">The duration of the frame, in game ticks.</param>
        public void AddFrame(int x, int y, int duration)
        {
            this.frames.Add(new SpriteFrame(this, duration, new Rectangle(x, y, this.Width, this.Height)));
            CheckTickable();
        }

        /// <summary>
        /// Inserts a frame with no image or duration at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the frame.</param>
        public void InsertFrame(int index)
        {
            InsertFrame(index, 0, 0, 0);
        }

        /// <summary>
        /// Inserts a frame with the given coordinates and duration at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert the frame.</param>
        /// <param name="tilesheet">The image from which to retreive the frame.</param>
        /// <param name="x">The x-coordinate, on the tilesheet, of the top-left corner of the frame image.</param>
        /// <param name="y">The y-coordinate, on the tilesheet, of the top-left corner of the frame image.</param>
        /// <param name="duration">The duration of the frame, in game ticks.</param>
        public void InsertFrame(int index, int x, int y, int duration)
        {
            this.frames.Insert(index, new SpriteFrame(this, duration, new Rectangle(x, y, this.Width, this.Height)));
            CheckTickable();
        }

        /// <summary>
        /// Advances the sprite animation by one tick. This should be the default if Update is called once per game step.
        /// </summary>
        public void Update() { Update(1); }

        /// <summary>
        /// Advances the sprite animation, if it is currently playing.
        /// </summary>
        /// <param name="ticks">The number of steps, or ticks, to advance the animation.</param>
        public void Update(int ticks)
        {
            if (!Playing || !tickable) return;

            this.lastFrameTime += ticks;
            int neededTime = frames[currentFrame].Duration;

            while (this.lastFrameTime >= neededTime)
            {
                this.lastFrameTime -= neededTime;
                this.TickFrame();

                neededTime = frames[currentFrame].Duration;
            }
        }

        private bool tickable;

        internal void CheckTickable()
        {
            tickable = false;
            if (frames.Count <= 1)
                return;
            else if (frames.Any(frame => frame.Duration > 0))
            {
                tickable = true;
            }
        }

        /// <summary>
        /// Begins playing the animation from the beginning.
        /// </summary>
        public virtual void Play()
        {
            Playing = true;
            Reset();
        }

        /// <summary>
        /// Stops and resets the animation.
        /// </summary>
        public virtual void Stop()
        {
            this.Playing = false;
            this.Reset();
            if (Stopped != null) Stopped();
        }

        /// <summary>
        /// Resumes playing the animation from the current frame.
        /// </summary>
        public virtual void Resume() { this.Playing = true; }

        /// <summary>
        /// Pauses the animation at the current frame.
        /// </summary>
        public virtual void Pause() { this.Playing = false; }

        /// <summary>
        /// Restarts the animation to the first frame or the last, based on the value of AnimDirection.
        /// </summary>
        public void Reset()
        {
            if (AnimDirection == AnimationDirection.Forward) currentFrame = 0;
            else currentFrame = Count - 1;

            lastFrameTime = 0;
        }

        private void TickFrame()
        {
            switch (this.AnimDirection)
            {
                case AnimationDirection.Forward:
                    this.currentFrame++;
                    break;
                case AnimationDirection.Backward:
                    this.currentFrame--;
                    break;
            }

            if (this.currentFrame >= this.Count)
            {
                switch (this.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        this.currentFrame--;
                        this.Pause();
                        break;
                    case AnimationStyle.Repeat:
                        this.currentFrame = 0;
                        break;
                    case AnimationStyle.Bounce:
                        this.currentFrame -= 2;
                        this.AnimDirection = AnimationDirection.Backward;
                        break;
                }
            }
            else if (this.currentFrame < 0)
            {
                switch (this.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        this.currentFrame++;
                        this.Pause();
                        break;
                    case AnimationStyle.Repeat:
                        this.currentFrame = this.Count - 1;
                        break;
                    case AnimationStyle.Bounce:
                        this.currentFrame = 1;
                        this.AnimDirection = AnimationDirection.Forward;
                        break;
                }
            }
        }

        private void ResizeFrames()
        {
            if (frames == null)
                return;

            foreach (var frame in frames)
            {
                frame.Resize(Width, Height);
            }
        }

        public void Draw(IRenderingContext context, int layer, float positionX, float positionY)
        {
            if (!Visible || Count == 0 || context == null) return;

            if (texture == null)
                texture = context.LoadResource(SheetPath, PaletteName);

            bool flipHorizontal = HorizontalFlip ^ Reversed;
            bool flipVertical = VerticalFlip;

            int hx = (HorizontalFlip ^ Reversed) ? Width - HotSpot.X : HotSpot.X;
            int hy = VerticalFlip ? Height - HotSpot.Y : HotSpot.Y;

            var drawTexture = this.texture;

            context.Draw(drawTexture, layer,
                new Common.Geometry.Point((int)(positionX - hx), (int)(positionY - hy)),
                new Common.Geometry.Rectangle(CurrentFrame.SheetLocation.X, CurrentFrame.SheetLocation.Y, CurrentFrame.SheetLocation.Width, CurrentFrame.SheetLocation.Height),
                flipHorizontal, flipVertical);
        }

        public static Sprite Empty = new Sprite(0, 0);

        #region ICollection<SpriteFrame> Members

        public void Add(SpriteFrame item)
        {
            frames.Add(item);
            CheckTickable();
        }

        public void Clear()
        {
            frames.Clear();
            tickable = false;
        }

        public bool Contains(SpriteFrame item)
        {
            return frames.Contains(item);
        }

        public void CopyTo(SpriteFrame[] array, int arrayIndex)
        {
            frames.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(SpriteFrame item)
        {
            var result = frames.Remove(item);
            currentFrame = Math.Max(Math.Min(frames.Count - 1, currentFrame), 0);
            return result;
        }

        #endregion

        #region IEnumerable<SpriteFrame> Members

        public IEnumerator<SpriteFrame> GetEnumerator()
        {
            return frames.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return frames.GetEnumerator();
        }

        #endregion
    }

    public class SpriteFrame
    {
        private Sprite sprite;
        private int duration;

        public Rectangle SheetLocation { get; private set; }

        /// <summary>
        /// Gets or sets the number of ticks that this image should be displayed.
        /// </summary>
        public int Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
                sprite.CheckTickable();
            }
        }

        internal SpriteFrame(Sprite spr, int duration, Rectangle sheetRect)
        {
            this.sprite = spr;
            this.Duration = duration;
            this.SheetLocation = sheetRect;
        }

        public void SetSheetPosition(int x, int y)
        {
            SheetLocation = new Rectangle(x, y, sprite.Width, sprite.Height);
        }

        internal void Resize(int width, int height)
        {
            SheetLocation = new Rectangle(SheetLocation.X, SheetLocation.Y, width, height);
        }
    }

    /// <summary>
    /// Specifies in which direction an animation playback should sweep.
    /// </summary>
    public enum AnimationDirection
    {
        Forward = 1,
        Backward = 2
    }

    /// <summary>
    /// Describes how an animation is played.
    /// </summary>
    public enum AnimationStyle
    {
        /// <summary>
        /// The animation is played until the last frame, and then stops.
        /// </summary>
        PlayOnce = 1,
        /// <summary>
        /// The animation will repeat from the beginning indefinitely.
        /// </summary>
        Repeat = 2,
        /// <summary>
        /// The animation will play forward and backward indefinitely.
        /// </summary>
        Bounce = 3
    }
}
