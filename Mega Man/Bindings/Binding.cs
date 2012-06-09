using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MegaMan.Common;

namespace MegaMan.Engine
{
    public abstract class Binding
    {
        protected object target;
        protected PropertyInfo targetProperty;

        public static Binding Create(SceneBindingInfo info, object target)
        {
            var sourceParts = info.Source.Split('.');
            if (sourceParts.Length == 0)
            {
                throw new GameRunException(String.Format("Binding source '{0}' is invalid.", info.Source));
            }

            var targetProperty = target.GetType().GetProperty(info.Target);

            if (targetProperty == null)
            {
                throw new GameRunException(String.Format("Binding target '{0}' is invalid.", info.Target));
            }

            switch (sourceParts[0].ToUpper())
            {
                case "INVENTORY":
                    return new InventoryBinding(target, targetProperty, sourceParts);
                
                case "WEAPON":
                    if (sourceParts.Length != 2)
                    {
                        throw new GameRunException("Weapon bindings must be given of the format 'Weapon.{NAME}', for example, 'Weapon.MBuster'.");
                    }

                    return new WeaponBinding(target, targetProperty, sourceParts[1]);

                case "HEALTH":
                    if (sourceParts.Length != 2)
                    {
                        throw new GameRunException("Health bindings must be given of the format 'Health.{ENTITYNAME}', for example, 'Health.Player'.");
                    }

                    return new HealthBinding(target, targetProperty, sourceParts[1]);
                
                default:
                    throw new GameRunException(String.Format("Binding type '{0}' is invalid.", info.Source));
            }
        }

        protected Binding(object target, PropertyInfo targetProperty)
        {
            this.target = target;
            this.targetProperty = targetProperty;
        }

        public abstract void Start(IEntityContainer container);
        public abstract void Stop();
    }
}
