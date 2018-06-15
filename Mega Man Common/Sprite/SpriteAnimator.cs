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
                return sprite[CurrentIndex];
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
            if (!Playing || !sprite.Tickable) return;

            FrameTime += ticks;
            int neededTime = sprite[CurrentIndex].Duration;

            while (FrameTime >= neededTime)
            {
                FrameTime -= neededTime;
                TickFrame();

                neededTime = sprite[CurrentIndex].Duration;
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
            Playing = false;
            Reset();
        }

        /// <summary>
        /// Resumes playing the animation from the current frame.
        /// </summary>
        public virtual void Resume() { Playing = true; }

        /// <summary>
        /// Pauses the animation at the current frame.
        /// </summary>
        public virtual void Pause() { Playing = false; }

        /// <summary>
        /// Restarts the animation to the first frame or the last, based on the value of AnimDirection.
        /// </summary>
        private void Reset()
        {
            if (sprite.AnimDirection == AnimationDirection.Forward) CurrentIndex = 0;
            else CurrentIndex = sprite.Count - 1;

            FrameTime = 0;
        }

        private void TickFrame()
        {
            switch (sprite.AnimDirection)
            {
                case AnimationDirection.Forward:
                    CurrentIndex++;
                    break;
                case AnimationDirection.Backward:
                    CurrentIndex--;
                    break;
            }

            if (CurrentIndex >= sprite.Count)
            {
                switch (sprite.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        CurrentIndex--;
                        Pause();
                        break;
                    case AnimationStyle.Repeat:
                        CurrentIndex = 0;
                        break;
                    case AnimationStyle.Bounce:
                        CurrentIndex -= 2;
                        sprite.AnimDirection = AnimationDirection.Backward;
                        break;
                }
            }
            else if (CurrentIndex < 0)
            {
                switch (sprite.AnimStyle)
                {
                    case AnimationStyle.PlayOnce:
                        CurrentIndex++;
                        Pause();
                        break;
                    case AnimationStyle.Repeat:
                        CurrentIndex = sprite.Count - 1;
                        break;
                    case AnimationStyle.Bounce:
                        CurrentIndex = 1;
                        sprite.AnimDirection = AnimationDirection.Forward;
                        break;
                }
            }
        }
    }
}
