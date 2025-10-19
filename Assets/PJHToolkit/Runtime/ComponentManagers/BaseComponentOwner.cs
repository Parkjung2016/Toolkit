using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    public abstract class BaseComponentOwner<T> : MonoBehaviour where T : BaseComponentOwner<T>
    {
        private readonly ComponentManager _componentManager = new();

        protected void InitComponent<T1>(T1 t) where T1 : T
        {
            _componentManager.AddComponentToDictionary(t);
            BeforeComponentsInitialize();
            _componentManager.ComponentInitialize(t);
            _componentManager.AfterInitialize();
            AfterComponentsInitialize();
        }

        protected virtual void BeforeComponentsInitialize()
        {
        }

        protected virtual void AfterComponentsInitialize()
        {
        }

        public T1 GetCompo<T1>(bool isDerived = false) where T1 : IObjectComponentBase
        {
            return _componentManager.GetCompo<T1>(isDerived);
        }

        public bool TryGetCompo<T1>(out T1 compo, bool isDerived = false)
            where T1 : IObjectComponentBase
        {
            return _componentManager.TryGetCompo(out compo, isDerived);
        }

        public void EnableComponents(bool isEnabled)
        {
            _componentManager.EnableComponents(enabled);
        }
    }
}