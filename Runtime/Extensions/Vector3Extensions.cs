using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Vector3의 x, y, z 값을 원하는 값으로 설정합니다. (지정하지 않으면 기존 값 유지)
        /// </summary>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        /// <summary>
        /// Vector3의 x, y, z 값에 각각 값을 더합니다.
        /// </summary>
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }

        /// <summary>
        /// 현재 Vector3가 대상 Vector3로부터 일정 거리(range) 이내에 있는지를 확인합니다.
        /// </summary>
        /// <param name="current">현재 Vector3 위치</param>
        /// <param name="target">비교할 대상 Vector3 위치</param>
        /// <param name="range">비교할 거리 범위</param>
        /// <returns>range 거리 이내에 있다면 true, 아니면 false</returns>
        public static bool InRangeOf(this Vector3 current, Vector3 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }

        /// <summary>
        /// 두 Vector3를 컴포넌트 단위(x, y, z)로 나눕니다.
        /// </summary>
        /// <remarks>
        /// v1의 각 컴포넌트가 0이 아닐 경우 v0의 해당 컴포넌트를 나눕니다.
        /// 0이면 나누지 않고 기존 값을 유지합니다.
        /// </remarks>
        /// <example>
        /// 게임 오브젝트를 비율에 따라 스케일 조정할 때 사용할 수 있습니다.
        /// <code>
        /// myObject.transform.localScale = originalScale.ComponentDivide(targetDimensions);
        /// </code>
        /// </example>
        public static Vector3 ComponentDivide(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v1.x != 0 ? v0.x / v1.x : v0.x,
                v1.y != 0 ? v0.y / v1.y : v0.y,
                v1.z != 0 ? v0.z / v1.z : v0.z);
        }

        /// <summary>
        /// Vector2를 y 값이 0인 Vector3로 변환합니다.
        /// </summary>
        /// <param name="v2">변환할 Vector2</param>
        /// <returns>x는 v2.x, y는 0, z는 v2.y인 Vector3</returns>
        public static Vector3 ToVector3(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }

        /// <summary>
        /// Vector3의 각 컴포넌트에 [-range, range] 범위의 랜덤한 오프셋을 추가합니다.
        /// </summary>
        /// <param name="vector">원본 Vector3</param>
        /// <param name="range">각 축에 적용할 최대 랜덤 오프셋 값</param>
        /// <returns>랜덤 오프셋이 적용된 Vector3</returns>
        public static Vector3 RandomOffset(this Vector3 vector, float range)
        {
            return vector + new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                Random.Range(-range, range)
            );
        }

        /// <summary>
        /// 중심점(origin)을 기준으로 최소/최대 반지름 범위 안의 임의의 지점(Vector3)을 반환합니다. 
        /// 2D 원형 고리(annulus) 안에서 선택됩니다.
        /// </summary>
        /// <param name="origin">중심이 되는 지점</param>
        /// <param name="minRadius">최소 반지름</param>
        /// <param name="maxRadius">최대 반지름</param>
        /// <returns>고리 형태 범위 내 임의의 Vector3 위치</returns>
        public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)
        {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // Converting the 2D direction vector to a 3D position vector
            Vector3 position = new Vector3(direction.x, 0, direction.y) * distance;
            return origin + position;
        }
    }
}