using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CrashReporterDotNET;
using MegaMan.IO;
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

        public event EventHandler Tick;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            timer.Tick += (s, ev) => {
                Tick?.Invoke(this, ev);
            };
            timer.Start();

            Container = new StandardKernel();
            Container.Load(Assembly.GetExecutingAssembly());
            Container.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());
            Container.Bind(x => x.FromAssemblyContaining(typeof(IGameLoader)).SelectAllClasses().BindAllInterfaces());
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
#if !DEBUG   // In debug mode do not custom-handle the exception, let Visual Studio handle it
            ShowUnhandledException(e.Exception);    
#endif
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if !DEBUG   // In debug mode do not custom-handle the exception, let Visual Studio handle it
            ShowUnhandledException((Exception)e.ExceptionObject);    
#endif
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG   // In debug mode do not custom-handle the exception, let Visual Studio handle it
            e.Handled = false;
#else
            e.Handled = true;
            ShowUnhandledException(e.Exception);    
#endif
        }

        void ShowUnhandledException(Exception ex)
        {
            string errorMessage = string.Format("I'm sorry, an application error occurred.\nIf this error occurs again there may be a bug in the application.\n\nError: {0}\n\nDo you want to continue?\nIf you click No the application will close.",

            ex.Message + (ex.InnerException != null ? "\n" +
            ex.InnerException.Message : null));

            var reportCrash = new ReportCrash("tesserex@gmail.com") {
                Silent = true,
                DoctorDumpSettings = new DoctorDumpSettings() {
                    ApplicationID = new Guid("24be95fb-ed64-4805-b8a2-bcfa5985a5c2"),
                    SendAnonymousReportSilently = true
                }
            };
            reportCrash.Send(ex);

            if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                if (MessageBox.Show("Any changes will not be saved.\nAre you sure you want to close?", "Close Wily's Lab", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Current.Shutdown();
                }
            }
        }

        public static bool IsDiskFull(Exception ex)
        {
            const int HR_ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);
            const int HR_ERROR_DISK_FULL = unchecked((int)0x80070070);

            return ex.HResult == HR_ERROR_HANDLE_DISK_FULL
                || ex.HResult == HR_ERROR_DISK_FULL;
        }
    }
}
