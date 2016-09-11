using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ninject;
using Ninject.Extensions.Conventions;
using MegaMan.Editor.Bll.Factories;

namespace MegaMan.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IKernel Container { get; private set; }

        public event Action Tick;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            timer.Tick += (s, ev) =>
            {
                if (Tick != null)
                {
                    Tick();
                    Tick();
                }
            };
            timer.Start();

            Container = new StandardKernel();
            Container.Load(System.Reflection.Assembly.GetExecutingAssembly());
            Container.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());
            Container.Bind(x => x.FromAssemblyContaining(typeof(MegaMan.IO.IGameLoader)).SelectAllClasses().BindAllInterfaces());
            Container.Bind<FactoryCore>().ToSelf().InSingletonScope();
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
            if (sprite != null)
            {
                Tick -= sprite.Update;
                Tick += sprite.Update;
            }
        }
    }
}
