using System.Linq;
using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// GameObject를 Hierarchy 창에서 숨깁니다.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void HideInHierarchy(this GameObject gameObject)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// GameObject에 지정된 타입의 컴포넌트를 가져옵니다. 해당 타입의 컴포넌트가 없으면 새로 추가합니다.
        /// </summary>
        /// <remarks>
        /// GameObject에 특정 타입의 컴포넌트가 있는지 확실하지 않을 때 유용합니다.
        /// 컴포넌트를 수동으로 확인하고 추가하는 대신, 이 메서드를 사용하면 한 줄로 둘 다 처리할 수 있습니다.
        /// </remarks>
        /// <typeparam name="T">가져오거나 추가할 컴포넌트의 타입입니다.</typeparam>
        /// <param name="gameObject">컴포넌트를 가져오거나 추가할 대상 GameObject입니다.</param>
        /// <returns>이미 존재하는 컴포넌트가 있다면 그 컴포넌트를, 없다면 새로 추가한 컴포넌트를 반환합니다.</returns>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 객체가 존재하면 자기 자신을 반환하고, 그렇지 않으면 null을 반환합니다.
        /// </summary>
        /// <remarks>
        /// 이 메서드는 null 참조와 Unity에서 파괴된 객체를 구분하는 데 도움이 됩니다.
        /// Unity의 "== null" 비교는 파괴된 객체에 대해서도 true를 반환할 수 있어, 오동작을 일으킬 수 있습니다.
        /// OrNull 메서드는 Unity의 null 체크 방식을 사용하며, 객체가 파괴된 상태라면 실제 null을 반환하여
        /// 연산을 안전하게 이어가고 NullReferenceException을 방지할 수 있도록 합니다.
        /// </remarks>
        /// <typeparam name="T">체크할 객체의 타입입니다.</typeparam>
        /// <param name="obj">확인할 객체입니다.</param>
        /// <returns>객체가 존재하고 파괴되지 않았다면 자기 자신을, 그렇지 않으면 null을 반환합니다.</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// GameObject의 모든 자식 오브젝트를 삭제합니다.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildren();
        }

        /// <summary>
        /// GameObject의 모든 자식 오브젝트를 즉시 삭제합니다.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildrenImmediate();
        }

        /// <summary>
        /// GameObject의 모든 자식 오브젝트를 활성화합니다.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
        public static void EnableChildren(this GameObject gameObject)
        {
            gameObject.transform.EnableChildren();
        }

        /// <summary>
        /// GameObject의 모든 자식 오브젝트를 비활성화합니다.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
        public static void DisableChildren(this GameObject gameObject)
        {
            gameObject.transform.DisableChildren();
        }

        /// <summary>
        /// GameObject의 Transform의 위치, 회전, 스케일을 로컬 좌표계를 기준으로 초기화합니다.
        /// </summary>
        /// <param name="gameObject">Transform을 초기화할 GameObject입니다.</param>
        public static void ResetTransformation(this GameObject gameObject)
        {
            gameObject.transform.Reset();
        }

        /// <summary>
        /// 이 GameObject의 Unity 씬 계층 내 경로를 반환합니다.
        /// </summary>
        /// <param name="gameObject">경로를 구할 대상 GameObject입니다.</param>
        /// <returns>
        /// 이 GameObject의 전체 계층 경로를 나타내는 문자열을 반환합니다.
        /// 각 계층 이름은 '/'로 구분되며, 최상위 부모부터 시작하여 지정된 GameObject의 **부모까지**의 이름으로 구성됩니다.
        /// </returns>
        public static string Path(this GameObject gameObject)
        {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }

        /// <summary>
        /// 이 GameObject의 Unity 씬 계층 구조에서 전체 경로를 반환합니다.
        /// </summary>
        /// <param name="gameObject">경로를 얻을 대상 GameObject입니다.</param>
        /// <returns>
        /// 이 GameObject의 전체 계층 경로를 나타내는 문자열을 반환합니다.
        /// '/'로 구분된 문자열이며, 각 부분은 루트 부모부터 시작해서 지정된 GameObject 자체 이름으로 끝납니다.
        /// </returns>
        public static string PathFull(this GameObject gameObject)
        {
            return gameObject.Path() + "/" + gameObject.name;
        }

        /// <summary>
        /// 이 GameObject와 그 자식들 모두에 대해 지정한 레이어를 재귀적으로 설정합니다.
        /// </summary>
        /// <param name="gameObject">레이어를 설정할 GameObject입니다.</param>
        /// <param name="layer">GameObject와 그 자식들에 설정할 레이어 번호입니다.</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }

        /// <summary>
        /// MonoBehaviour에 연결된 GameObject를 활성화하고, 해당 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">MonoBehaviour 타입입니다.</typeparam>
        /// <param name="obj">활성화할 MonoBehaviour 인스턴스입니다.</param>
        /// <returns>활성화된 MonoBehaviour 인스턴스를 반환합니다.</returns>
        public static T SetActive<T>(this T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// MonoBehaviour에 연결된 GameObject를 비활성화하고, 해당 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">MonoBehaviour 타입입니다.</typeparam>
        /// <param name="obj">비활성화할 MonoBehaviour 인스턴스입니다.</param>
        /// <returns>비활성화된 MonoBehaviour 인스턴스를 반환합니다.</returns>
        public static T SetInactive<T>(this T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}