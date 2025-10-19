using System;
using System.Collections.Generic;
using System.Linq;
using PJH.Toolkit.Extensions;
using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    public class ComponentManager
    {
        private readonly Dictionary<Type, IObjectComponentBase> _components = new();

        public void AddComponentToDictionary(MonoBehaviour owner)
        {
            var components = owner.GetComponentsInChildren<IObjectComponentBase>(true);
            var orderedComponentEnumerable = components
                .OrderBy(c =>
                {
                    var attr = c.GetType().GetCustomAttributes(typeof(ComponentOrderAttribute), false)
                        .FirstOrDefault() as ComponentOrderAttribute;
                    return attr?.Order ?? 0;
                });

            foreach (var component in orderedComponentEnumerable)
            {
                var compType = component.GetType();
                _components[compType] = component;
            }
        }

        public void ComponentInitialize<T>(T owner) where T : MonoBehaviour
        {
            _components.Values.ForEach(component =>
            {
                IObjectComponent<T> objectComponent = component as IObjectComponent<T>;
                objectComponent.Initialize(owner);
            });
        }

        public void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitable>().ForEach(afterInitable => afterInitable.AfterInitialize());
        }

        public T GetCompo<T>(bool isDerived = false) where T : IObjectComponentBase
        {
            if (_components.TryGetValue(typeof(T), out var baseComponent))
            {
                if (baseComponent is T exactMatch)
                    return exactMatch;
            }

            if (isDerived)
            {
                foreach (var kvp in _components)
                {
                    if (typeof(T).IsAssignableFrom(kvp.Key))
                    {
                        if (kvp.Value is T derivedMatch)
                            return derivedMatch;
                    }
                }
            }

            return default;
        }

        public bool TryGetCompo<T1>(out T1 compo, bool isDerived = false) where T1 : IObjectComponentBase
        {
            compo = GetCompo<T1>(isDerived);
            return compo != null;
        }

        public void EnableComponents(bool isEnabled)
        {
            _components.Values.OfType<MonoBehaviour>().ForEach(component => { component.enabled = isEnabled; });
        }
    }
}