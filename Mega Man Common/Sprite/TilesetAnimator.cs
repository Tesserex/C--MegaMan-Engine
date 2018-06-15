using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Common
{
    public class TilesetAnimator
    {
        private Dictionary<int, SpriteAnimator> animators;

        public TilesetAnimator(Tileset tileset)
        {
            animators = tileset.ToDictionary(t => t.Id, t => new SpriteAnimator(t.Sprite));
        }

        public void Add(Tile tile)
        {
            if (!animators.ContainsKey(tile.Id))
            {
                animators[tile.Id] = new SpriteAnimator(tile.Sprite);
                if (animators.Values.Any(a => a.Playing))
                {
                    animators[tile.Id].Play();
                }
            }
        }

        public void Remove(Tile tile)
        {
            if (animators.ContainsKey(tile.Id))
            {
                animators[tile.Id].Stop();
                animators.Remove(tile.Id);
            }
        }

        public void Play()
        {
            foreach (var anim in animators.Values)
                anim.Play();
        }

        public void Pause()
        {
            foreach (var anim in animators.Values)
                anim.Pause();
        }

        public void Resume()
        {
            foreach (var anim in animators.Values)
                anim.Resume();
        }

        public void Stop()
        {
            foreach (var anim in animators.Values)
                anim.Stop();
        }

        public void Update()
        {
            foreach (var anim in animators.Values)
                anim.Update();
        }

        public int GetFrameIndex(int tileId)
        {
            return animators[tileId].CurrentIndex;
        }

        public SpriteFrame GetFrame(int tileId)
        {
            return animators[tileId].CurrentFrame;
        }
    }
}
