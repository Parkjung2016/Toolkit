using PJH.Utility.Utils;
using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 자식 오브젝트들 중에 지정한 타입의 컴포넌트가 존재하는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">검색할 컴포넌트 타입입니다.</typeparam>
        /// <param name="component">기준이 되는 컴포넌트입니다.</param>
        /// <returns>컴포넌트가 존재하면 true, 없으면 false를 반환합니다.</returns>
        public static bool HasComponentInChildren<T>(this Component component) where T : Component
        {
            return component.GetComponentInChildren<T>();
        }

        /// <summary>
        /// 부모 오브젝트들 중에 지정한 타입의 컴포넌트가 존재하는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">검색할 컴포넌트 타입입니다.</typeparam>
        /// <param name="component">기준이 되는 컴포넌트입니다.</param>
        /// <returns>컴포넌트가 존재하면 true, 없으면 false를 반환합니다.</returns>
        public static bool HasComponentInParent<T>(this Component component) where T : Component
        {
            return component.GetComponentInChildren<T>();
        }

        /// <summary>
        /// 현재 오브젝트에 지정한 타입의 컴포넌트가 존재하는지 확인합니다.
        /// </summary>
        /// <typeparam name="T">검색할 컴포넌트 타입입니다.</typeparam>
        /// <param name="component">기준이 되는 컴포넌트입니다.</param>
        /// <returns>컴포넌트가 존재하면 true, 없으면 false를 반환합니다.</returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>();
        }

        /// <summary>
        /// 현재 오브젝트에 지정합 타입의 컴포넌트틀 찾아서 반환합니다. 없으면 새로 추가합니다.
        /// </summary>
        /// <typeparam name="T">검색할 컴포넌트 타입입니다.</typeparam>
        /// <param name="gameObject">기준이 되는 오브젝트입니다.</param>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            return ComponentUtil.GetOrAddComponent<T>(gameObject);
        }
    }
}