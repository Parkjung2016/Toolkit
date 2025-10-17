using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    public interface IAfterInitable
    {
        public void AfterInitialize();
    }

    public interface IObjectComponent<in T> where T : MonoBehaviour
    {
        public void Initialize(T owner);
    }
}