using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public class TilesetAnimator
    {
        private Dictionary<int, SpriteAnimator> animators;

        public TilesetAnimator(Tileset tileset)
        {
            this.animators = tileset.ToDictionary(t => t.Id, t => new SpriteAnimator(t.Sprite));
        }

        public void Add(Tile tile)
        {
            if (!this.animators.ContainsKey(tile.Id))
            {
                this.animators[tile.Id] = new SpriteAnimator(tile.Sprite);
                if (this.animators.Values.Any(a => a.Playing))
                {
                    this.animators[tile.Id].Play();
                }
            }
        }

        public void Remove(Tile tile)
        {
            if (this.animators.ContainsKey(tile.Id))
            {
                this.animators[tile.Id].Stop();
                this.animators.Remove(tile.Id);
            }
        }

        public void Play()
        {
            foreach (var anim in this.animators.Values)
                anim.Play();
        }

        public void Pause()
        {
            foreach (var anim in this.animators.Values)
                anim.Pause();
        }

        public void Resume()
        {
            foreach (var anim in this.animators.Values)
                anim.Resume();
        }

        public void Stop()
        {
            foreach (var anim in this.animators.Values)
                anim.Stop();
        }

        public void Update()
        {
            foreach (var anim in this.animators.Values)
                anim.Update();
        }

        public int GetFrameIndex(int tileId)
        {
            return this.animators[tileId].CurrentIndex;
        }

        public SpriteFrame GetFrame(int tileId)
        {
            return this.animators[tileId].CurrentFrame;
        }
    }
}
