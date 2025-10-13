using UnityEngine;

namespace PJH.Utility.Singleton
{
    /// <summary>
    /// Unity 씬이 바뀌어도 파괴되지 않고 유지되는 싱글톤-	먼저 생긴 인스턴스만 유지합니다.
    /// </summary>
    /// <typeparam name="T">MonoBehaviour 타입</typeparam>
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Awake가 필요하다면, 오버라이드할 때 base.Awake()를 꼭 호출해야 됨.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            transform.SetParent(null);

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}