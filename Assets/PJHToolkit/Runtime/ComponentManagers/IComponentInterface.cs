using UnityEngine;

namespace PJH.Toolkit.PJHToolkit.Runtime.ComponentManagers
{
    /// <summary>
    /// 초기화 이후에 호출되는 메서드를 정의하는 인터페이스입니다.
    /// </summary>
    public interface IAfterInitable
    {
        public void AfterInitialize();
    }

    public interface IObjectComponentBase
    {
    }

    /// <summary>
    /// ComponentOrder 어트리뷰트로 초기화 순서를 지정할 수 있습니다.
    /// </summary>
    public interface IObjectComponent<in T> where T : MonoBehaviour
    {
        public void Initialize(T owner);
    }
}