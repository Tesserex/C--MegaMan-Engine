using Ninject;
using Ninject.Extensions.Conventions;

namespace MegaMan.Engine
{
    internal static class Injector
    {
        public static IKernel Container { get; private set; }

        static Injector()
        {
            Container = new StandardKernel();
            Container.Load(System.Reflection.Assembly.GetCallingAssembly());
            Container.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());
            Container.Bind(x => x.FromAssemblyContaining<IO.GameLoader>().SelectAllClasses().BindAllInterfaces());
        }
    }
}
