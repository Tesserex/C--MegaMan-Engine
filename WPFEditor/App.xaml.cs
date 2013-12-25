using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public event Action Tick;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            timer.Tick += (s, ev) => { if (Tick != null) Tick(); };
            timer.Start();
        }

        public void AnimateTileset(Tileset tileset)
        {
            foreach (var tile in tileset)
            {
                AnimateSprite(tile.Sprite);
            }
        }

        public void AnimateSprite(Sprite sprite)
        {
            Tick -= sprite.Update;
            Tick += sprite.Update;
        }
    }
}
