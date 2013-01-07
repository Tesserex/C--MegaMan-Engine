using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public class Animator
    {
        private Tileset _tileset;

        public void ChangeTileset(Tileset tileset)
        {
            _tileset = tileset;

            if (_tileset != null)
            {
                foreach (var tile in _tileset)
                {
                    tile.Sprite.Play();
                }
            }
        }

        public Animator()
        {
            ((App)App.Current).Tick += Animator_Tick;
        }

        void Animator_Tick()
        {
            if (_tileset != null)
            {
                foreach (var tile in _tileset)
                {
                    tile.Sprite.Update();
                }
            }
        }
    }
}
