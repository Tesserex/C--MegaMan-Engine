using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public class SpriteAnimator
    {
        private readonly Sprite sprite;

        public SpriteAnimator(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public int CurrentIndex { get; set; }
        public int FrameTime { get; set; }

        /// <summary>
        /// Gets whether or not the sprite animation is currently playing.
        /// </summary>
        public bool Playing { get; private set; }

        public SpriteFrame CurrentFrame
        {
            get
            {
                return this.sprite[CurrentIndex];
            }
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
            if (!this.Playing || !this.sprite.Tickable) return;

            this.FrameTime += ticks;
            int neededTime = this.sprite[CurrentIndex].Duration;

            while (this.FrameTime >= neededTime)
            {
                this.FrameTime -= neededTime;
                this.TickFrame();

                neededTime = this.sprite[CurrentIndex].Duration;
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
        private void Reset()
        {
            if (this.sprite.AnimDirection == AnimationDirection.Forward) CurrentIndex = 0;
            else CurrentIndex = this.sprite.Count - 1;

            this.FrameTime = 0;
        }

        private void TickFrame()
        {
            switch (this.sprite.AnimDirection)
            {
                case AnimationDirection.Forward:
                    this.CurrentIndex++;
                    break;
                case AnimationDirection.Backward:
                    this.CurrentIndex--;
                    break;
            }

            if (this.CurrentIndex >= this.sprite.Count)
            {
                switch (this.sprite.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        this.CurrentIndex--;
                        this.Pause();
                        break;
                    case AnimationStyle.Repeat:
                        this.CurrentIndex = 0;
                        break;
                    case AnimationStyle.Bounce:
                        this.CurrentIndex -= 2;
                        this.sprite.AnimDirection = AnimationDirection.Backward;
                        break;
                }
            }
            else if (this.CurrentIndex < 0)
            {
                switch (this.sprite.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        this.CurrentIndex++;
                        this.Pause();
                        break;
                    case AnimationStyle.Repeat:
                        this.CurrentIndex = this.sprite.Count - 1;
                        break;
                    case AnimationStyle.Bounce:
                        this.CurrentIndex = 1;
                        this.sprite.AnimDirection = AnimationDirection.Forward;
                        break;
                }
            }
        }
    }
}
