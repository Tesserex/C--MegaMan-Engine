using MegaMan.Editor.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MegaMan.Editor.Mediator
{
    internal class ViewModelMediator
    {
        private static ViewModelMediator _instance;
        private static object _lock = new object();

        public static ViewModelMediator Current
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ViewModelMediator();
                    }
                }

                return _instance;
            }
        }

        private Dictionary<Type, object> _mediatedEvents = new Dictionary<Type, object>();

        private ViewModelMediator()
        {

        }

        public MediatedEvent<T> GetEvent<T>()
        {
            if (!_mediatedEvents.ContainsKey(typeof(T)))
            {
                _mediatedEvents.Add(typeof(T), new MediatedEvent<T>());
            }

            return (MediatedEvent<T>)_mediatedEvents[typeof(T)];
        }
    }
}
