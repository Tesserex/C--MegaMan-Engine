﻿using System.Reflection;
using MegaMan.Engine.Entities;

namespace MegaMan.Engine
{
    public class HealthBinding : Binding
    {
        private string _entityId;
        private HealthComponent _health;

        public HealthBinding(object target, PropertyInfo targetProperty, string entity)
            : base(target, targetProperty)
        {
            _entityId = entity;
        }

        public override void Start(IEntityPool entityPool)
        {
            var entity = entityPool.GetEntityById(_entityId);
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
