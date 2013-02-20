using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public class Animator : IRequireCurrentStage
    {
        private Tileset _tileset;
        private IStageSelector _stageSelector;

        public Animator()
        {
            ((App)App.Current).Tick += Animator_Tick;
        }

        public void SetStage(StageDocument stage)
        {
            ChangeTileset(stage.Tileset);
        }

        public void UnsetStage()
        {
            ChangeTileset(null);
        }

        private void ChangeTileset(Tileset tileset)
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

        private void Animator_Tick()
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
