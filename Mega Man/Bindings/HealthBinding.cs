using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MegaMan.Engine
{
    public class HealthBinding : Binding
    {
        private string _entityName;
        private HealthComponent _health;

        public HealthBinding(object target, PropertyInfo targetProperty, string entity)
            : base(target, targetProperty)
        {
            this._entityName = entity;
        }

        public override void Start(IEntityContainer container)
        {
            var entity = container.GetEntities(_entityName).FirstOrDefault();
            if (entity == null) return;

            _health = entity.GetComponent<HealthComponent>();
            if (_health != null)
            {
                Set(_health.Health / _health.MaxHealth);
                _health.HealthChanged += health_HealthChanged;
            }
        }

        public override void Stop()
        {
            if (_health != null)
            {
                _health.HealthChanged -= health_HealthChanged;
            }
        }

        private void Set(float value)
        {
            targetProperty.SetValue(target, value, null);
        }

        private void health_HealthChanged(float value, float max)
        {
            Set(value / max);
        }
    }
}
