using System;
using System.Collections.Generic;

namespace MegaMan.Editor.Mediator
{
    class MediatedEvent<T>
    {
        private List<EventHandler<T>> _handlers = new List<EventHandler<T>>();

        private T _lastValue;

        public void Subscribe(EventHandler<T> handler, bool getLastValue = false)
        {
            _handlers.Add(handler);

            if (getLastValue && _lastValue != null)
                handler(null, _lastValue);
        }

        public void Unsubscribe(EventHandler<T> handler)
        {
            _handlers.Remove(handler);
        }

        public void Raise(object sender, T args)
        {
            _lastValue = args;

            foreach (var h in _handlers)
            {
                h(sender, args);
            }
        }
    }
}
