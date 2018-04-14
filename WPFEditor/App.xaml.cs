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

            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            timer.Tick += (s, ev) => {
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
        }

        public void AnimateTileset(TilesetAnimator animator)
        {
            Tick -= animator.Update;
            Tick += animator.Update;
        }

        public void AnimateSprite(SpriteAnimator animator)
        {
            Tick -= animator.Update;
            Tick += animator.Update;
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG   // In debug mode do not custom-handle the exception, let Visual Studio handle it
            e.Handled = false;
#else
            ShowUnhandledException(e);    
#endif
        }

        void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string errorMessage = string.Format("I'm sorry, an application error occurred.\nIf this error occurs again there may be a bug in the application.\n\nError: {0}\n\nDo you want to continue?\nIf you click No the application will close.",

            e.Exception.Message + (e.Exception.InnerException != null ? "\n" +
            e.Exception.InnerException.Message : null));

            if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                if (MessageBox.Show("Any changes will not be saved.\nAre you sure you want to close?", "Close Wily's Lab", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
