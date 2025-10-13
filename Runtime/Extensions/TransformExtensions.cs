using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PJH.Utility.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 지정한 Transform이 목표 Transform으로부터 특정 거리 이내에 있고, (선택적으로) 특정 각도(FOV) 이내에 있는지 확인합니다.
        /// </summary>
        /// <param name="source">거리와 각도를 확인할 Transform입니다.</param>
        /// <param name="target">비교할 목표 Transform입니다.</param>
        /// <param name="maxDistance">두 Transform 간의 최대 허용 거리입니다.</param>
        /// <param name="maxAngle">Transform의 forward 방향과 목표까지의 방향 간의 최대 허용 각도입니다. 기본값은 360도입니다.</param>
        /// <returns>거리와 각도(제공된 경우) 기준을 만족하면 true를 반환합니다. 그렇지 않으면 false입니다.</returns>
        public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle = 360f)
        {
            Vector3 directionToTarget = (target.position - source.position).With(y: 0);
            return directionToTarget.magnitude <= maxDistance &&
                   Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
        }

        /// <summary>
        /// 특정 Transform의 모든 자식들을 가져옵니다.
        /// </summary>
        /// <remarks>
        /// 이 메서드는 LINQ와 함께 사용할 수 있으며, 특정 태그를 가진 자식을 찾거나,
        /// 모든 자식을 비활성화하는 등 다양한 작업에 사용할 수 있습니다.
        /// Transform은 IEnumerable을 구현하며, GetEnumerator를 통해 자식들을 반환합니다.
        /// </remarks>
        /// <param name="parent">자식들을 가져올 부모 Transform입니다.</param>
        /// <returns>부모의 자식 Transform들을 포함하는 IEnumerable을 반환합니다.</returns>
        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                yield return child;
            }
        }

        /// <summary>
        /// Transform의 local 좌표계를 기준으로 위치, 회전, 스케일을 초기화(리셋)합니다.
        /// position : (0, 0, 0),
        /// localRotation : Quaternion.identity,
        /// localScale = (1, 1, 1)
        /// </summary>
        /// <param name="transform">초기화할 Transform입니다.</param>
        public static void Reset(this Transform transform)
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 지정한 Transform의 모든 자식 게임 오브젝트를 파괴합니다.
        /// </summary>
        /// <param name="parent">자식 게임 오브젝트들을 파괴할 Transform입니다.</param>
        public static void DestroyChildren(this Transform parent)
        {
            parent.ForEveryChild(child => Object.Destroy(child.gameObject));
        }

        /// <summary>
        /// 지정한 Transform의 모든 자식 게임 오브젝트를 즉시 파괴합니다.
        /// </summary>
        /// <param name="parent">자식 게임 오브젝트들을 즉시 파괴할 Transform입니다.</param>
        public static void DestroyChildrenImmediate(this Transform parent)
        {
            parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
        }

        /// <summary>
        /// 지정한 Transform의 모든 자식 게임 오브젝트를 활성화합니다.
        /// </summary>
        /// <param name="parent">자식 게임 오브젝트들을 활성화할 Transform입니다.</param>
        public static void EnableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(true));
        }

        /// <summary>
        /// 지정한 Transform의 모든 자식 게임 오브젝트를 비활성화합니다.
        /// </summary>
        /// <param name="parent">자식 게임 오브젝트들을 비활성화할 Transform입니다.</param>
        public static void DisableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(false));
        }

        /// <summary>
        /// 지정된 Transform의 모든 자식들을 역순으로 순회하며 액션을 수행합니다.
        /// </summary>
        /// <param name="parent">자식들을 순회할 부모 Transform입니다.</param>
        /// <param name="action">각 자식 Transform에 대해 실행할 액션입니다.</param>
        /// <remarks>
        /// 이 메서드는 자식 Transform들을 역순(가장 마지막 자식부터)으로 순회하며,
        /// 전달된 델리게이트 액션을 각 자식에게 실행합니다.
        /// </remarks>
        public static void ForEveryChild(this Transform parent, System.Action<Transform> action)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                action(parent.GetChild(i));
            }
        }
    }
}