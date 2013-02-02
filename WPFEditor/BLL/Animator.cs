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
        private IStageSelector _stageSelector;

        public Animator()
        {
            ((App)App.Current).Tick += Animator_Tick;
        }

        public void SetStageSelector(IStageSelector selector)
        {
            if (_stageSelector != null)
            {
                _stageSelector.StageChanged -= StageChanged;
            }

            _stageSelector = selector;

            _stageSelector.StageChanged += StageChanged;

            if (selector.Stage != null)
            {
                ChangeTileset(selector.Stage.Tileset);
            }
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            ChangeTileset(e.Stage.Tileset);
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
