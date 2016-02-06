using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject;

namespace MegaMan.IO
{
    internal static class Extensions
    {
        public static IEnumerable<T> GetImplementersOf<T>()
        {
            return Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(T)) && !t.IsAbstract)
                .Select(t => Injector.Container.Get(t))
                .Cast<T>();
        }
    }
}
