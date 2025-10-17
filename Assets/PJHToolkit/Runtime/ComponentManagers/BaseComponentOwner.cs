using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    public abstract class BaseComponentOwner<T> : MonoBehaviour where T : BaseComponentOwner<T>
    {
        private readonly ComponentManager<T> _componentManager = new();

        private void Awake()
        {
            _componentManager.AddComponentToDictionary(this as T);
            BeforeComponentsInitialize();
            _componentManager.ComponentInitialize(this as T);
            _componentManager.AfterInitialize();
            AfterComponentsInitialize();
        }
        protected virtual void BeforeComponentsInitialize()
        {
            
        }
        protected virtual void AfterComponentsInitialize()
        {
            
        }

        public T1 GetCompo<T1>(bool isDerived = false) where T1 : IObjectComponent<T>
        {
            return _componentManager.GetCompo<T1>(isDerived);
        }

        public bool TryGetCompo<T1>(out T1 compo, bool isDerived = false)
            where T1 : IObjectComponent<T>
        {
            return _componentManager.TryGetCompo<T1>(out compo, isDerived);
        }

        public void EnableComponents(bool isEnabled)
        {
            _componentManager.EnableComponents(enabled);
        }
    }
}