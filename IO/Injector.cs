using System.Reflection;
using Ninject;
using Ninject.Extensions.Conventions;

namespace MegaMan.IO
{
    internal static class Injector
    {
        public static IKernel Container { get; private set; }

        static Injector()
        {
            Container = new StandardKernel();
            Container.Load(Assembly.GetCallingAssembly());
            Container.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());
            Container.Bind(x => x.FromThisAssembly().SelectAllClasses().BindAllInterfaces());
        }
    }
}
