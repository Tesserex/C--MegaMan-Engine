using System;
using System.Collections;
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
        private Guid? id;
        private List<SpriteFrame> frames;

        private int width;
        private int height;

        private IResourceImage texture;

        public Guid Id
        {
            get
            {
                if (id == null)
                    id = Guid.NewGuid();

                return id.Value;
            }
        }

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

        
        public string Name { get; set; }
        public string Part { get; set; }
        public string PaletteName { get; set; }

        public bool HorizontalFlip { get; set; }

        public bool VerticalFlip { get; set; }

        public bool Visible { get; set; }

        public int Layer { get; set; }

        public virtual FilePath SheetPath { get; set; }
        public virtual string SheetPathRelative { get { return SheetPath != null ? SheetPath.Relative : null; } }

        public virtual byte[] SheetData { get; set; }

        /// <summary>
        /// If this is true, it means the sprite sheet is backwards - it's facing left instead of right,
        /// so we have to flip all drawing of this sprite to match proper orientation rules.
        /// </summary>
        public bool Reversed { get; set; }

        /// <summary>
        /// Creates a new Sprite object with the given width and height, and no frames.
        /// </summary>
        public Sprite(int width, int height)
        {
            Height = height;
            Width = width;
            frames = new List<SpriteFrame>();

            HotSpot = new Point(0, 0);
            BoundBox = new RectangleF(0, 0, width, height);
            Visible = true;
            AnimDirection = AnimationDirection.Forward;
            AnimStyle = AnimationStyle.Repeat;
        }

        public Sprite(Sprite copy)
        {
            Name = copy.Name;
            Part = copy.Part;
            PaletteName = copy.PaletteName;

            Height = copy.Height;
            Width = copy.Width;
            Tickable = copy.Tickable;
            frames = copy.frames;
            HotSpot = new Point(copy.HotSpot.X, copy.HotSpot.Y);
            BoundBox = new RectangleF(0, 0, copy.Width, copy.Height);
            Visible = true;
            AnimDirection = copy.AnimDirection;
            AnimStyle = copy.AnimStyle;
            Layer = copy.Layer;

            Reversed = copy.Reversed;
            if (copy.SheetPath != null)
            {
                SheetPath = FilePath.FromRelative(copy.SheetPath.Relative, copy.SheetPath.BasePath);
                SheetData = copy.SheetData;
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
            frames.Add(new SpriteFrame(this, duration, new Rectangle(x, y, Width, Height)));
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
            frames.Insert(index, new SpriteFrame(this, duration, new Rectangle(x, y, Width, Height)));
            CheckTickable();
        }

        public bool Tickable { get; private set; }

        internal void CheckTickable()
        {
            Tickable = false;
            if (frames.Count <= 1)
                return;
            if (frames.Any(frame => frame.Duration > 0))
            {
                Tickable = true;
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

        public void Draw(IRenderingContext context, int layer, float positionX, float positionY, int frameIndex)
        {
            if (!Visible || Count == 0 || context == null) return;

            if (texture == null)
                texture = context.LoadResource(SheetPath, SheetData, PaletteName);

            var flipHorizontal = HorizontalFlip ^ Reversed;
            var flipVertical = VerticalFlip;

            var hx = (HorizontalFlip ^ Reversed) ? Width - HotSpot.X : HotSpot.X;
            var hy = VerticalFlip ? Height - HotSpot.Y : HotSpot.Y;

            var drawTexture = texture;
            var frame = this[frameIndex];

            context.Draw(drawTexture, layer,
                new Point((int)(positionX - hx), (int)(positionY - hy)),
                new Rectangle(frame.SheetLocation.X, frame.SheetLocation.Y, frame.SheetLocation.Width, frame.SheetLocation.Height),
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
            Tickable = false;
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

        IEnumerator IEnumerable.GetEnumerator()
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
            sprite = spr;
            Duration = duration;
            SheetLocation = sheetRect;
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
