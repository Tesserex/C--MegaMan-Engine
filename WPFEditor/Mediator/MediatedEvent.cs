using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Mediator
{
    class MediatedEvent<T>
    {
        private List<EventHandler<T>> _handlers = new List<EventHandler<T>>();

        public void Subscribe(EventHandler<T> handler)
        {
            _handlers.Add(handler);
        }

        public void Unsubscribe(EventHandler<T> handler)
        {
            _handlers.Remove(handler);
        }

        public void Raise(object sender, T args)
        {
            foreach (var h in _handlers)
            {
                h(sender, args);
            }
        }
    }
}
