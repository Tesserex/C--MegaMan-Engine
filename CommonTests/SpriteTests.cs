using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaMan.Common.Tests
{
    [TestClass]
    public class SpriteTests
    {
        [TestMethod, TestCategory("Sprite")]
        public void Update_Playing_Runs()
        {
            var sprite = GetEmptySprite(5);
            var animator = new SpriteAnimator(sprite);

            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Play();
            animator.Update();

            Assert.AreEqual(1, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_GreaterThanOne_SkipsFrames()
        {
            var sprite = GetEmptySprite(4);
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            animator.Update(3);

            Assert.AreEqual(3, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Stopped_DoesntRun()
        {
            var sprite = GetEmptySprite(5);
            var animator = new SpriteAnimator(sprite);
            animator.Play();
            animator.Update();

            // should be at 1...

            animator.Pause();
            animator.Update();

            Assert.AreEqual(1, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Stop_Resets()
        {
            var sprite = GetEmptySprite(5);
            var animator = new SpriteAnimator(sprite);
            animator.Play();
            animator.Update();

            animator.Stop();

            Assert.AreEqual(0, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_LongFrame_Stays()
        {
            var sprite = GetEmptySprite(5);
            sprite[0].Duration = 2;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_ForwardOnce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.PlayOnce;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(2, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_BackwardOnce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Backward;
            sprite.AnimStyle = AnimationStyle.PlayOnce;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_ForwardRepeat()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.Repeat;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_BackwardRepeat()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Backward;
            sprite.AnimStyle = AnimationStyle.Repeat;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);
        }

        [TestMethod, TestCategory("Sprite")]
        public void Update_Bounce()
        {
            var sprite = GetEmptySprite(3);
            sprite.AnimDirection = AnimationDirection.Forward;
            sprite.AnimStyle = AnimationStyle.Bounce;
            var animator = new SpriteAnimator(sprite);
            animator.Play();

            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(2, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(0, animator.CurrentIndex);

            animator.Update();
            Assert.AreEqual(1, animator.CurrentIndex);
        }

        private static Sprite GetEmptySprite(int frames)
        {
            var sprite = new Sprite(1, 1);

            for (int i = 0; i < frames; i++)
            {
                sprite.AddFrame(0, 0, 1);
            }

            return sprite;
        }
    }
}
