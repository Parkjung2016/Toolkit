using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
 public class ComponentManager<T> where T : MonoBehaviour
    {
        private readonly Dictionary<Type, IObjectComponent<T>> _components = new();

        public void AddComponentToDictionary(T owner)
        {
            owner.GetComponentsInChildren<IObjectComponent<T>>(true).ToList().ForEach(component =>
            {
                var compType = component.GetType();
                _components[compType] = component;
            });
        }

        public void ComponentInitialize(T owner)
        {
            _components.Values.ToList().ForEach(component =>
            {
                IObjectComponent<T> objectComponent = component;
                objectComponent.Initialize(owner);
            });
        }

        public void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitable>()
                .ToList().ForEach(afterInitable => afterInitable.AfterInitialize());
        }

        public T1 GetCompo<T1>(bool isDerived = false) where T1 : IObjectComponent<T>
        {
            if (_components.TryGetValue(typeof(T1), out var baseComponent))
            {
                if (baseComponent is T1 exactMatch)
                    return exactMatch;
            }

            if (isDerived)
            {
                foreach (var kvp in _components)
                {
                    if (typeof(T1).IsAssignableFrom(kvp.Key))
                    {
                        if (kvp.Value is T1 derivedMatch)
                            return derivedMatch;
                    }
                }
            }

            return default;
        }

        public bool TryGetCompo<T1>(out T1 compo, bool isDerived = false) where T1 : IObjectComponent<T>
        {
            compo = GetCompo<T1>(isDerived);
            return compo != null;
        }

        public void EnableComponents(bool isEnabled)
        {
            _components.Values.OfType<MonoBehaviour>().ToList().ForEach(component =>
            {
                component.enabled = isEnabled;
            });
        }
    }
}