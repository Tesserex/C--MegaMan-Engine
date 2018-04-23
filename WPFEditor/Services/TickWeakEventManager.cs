using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MegaMan.Editor.Services
{
    public class TickWeakEventManager : WeakEventManager
    {
        private TickWeakEventManager()
        {

        }

        /// <summary>
        /// Add a handler for the given source's event.
        /// </summary>
        public static void AddHandler(EventHandler<EventArgs> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedAddHandler((App)Application.Current, handler);
        }

        /// <summary>
        /// Remove a handler for the given source's event.
        /// </summary>
        public static void RemoveHandler(EventHandler<EventArgs> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedRemoveHandler((App)Application.Current, handler);
        }

        /// <summary>
        /// Get the event manager for the current thread.
        /// </summary>
        private static TickWeakEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(TickWeakEventManager);
                TickWeakEventManager manager =
                    (TickWeakEventManager)GetCurrentManager(managerType);

                // at first use, create and register a new manager
                if (manager == null)
                {
                    manager = new TickWeakEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        /// <summary>
        /// Return a new list to hold listeners to the event.
        /// </summary>
        protected override ListenerList NewListenerList()
        {
            return new ListenerList<EventArgs>();
        }


        /// <summary>
        /// Listen to the given source for the event.
        /// </summary>
        protected override void StartListening(object source)
        {
            App typedSource = (App)source;
            typedSource.Tick += OnSomeEvent;
        }

        /// <summary>
        /// Stop listening to the given source for the event.
        /// </summary>
        protected override void StopListening(object source)
        {
            App typedSource = (App)source;
            typedSource.Tick -= OnSomeEvent;
        }

        /// <summary>
        /// Event handler for the SomeEvent event.
        /// </summary>
        void OnSomeEvent(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }
    }
}
