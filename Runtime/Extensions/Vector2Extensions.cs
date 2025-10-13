using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Vector2에 x 또는 y 값을 더합니다.
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0)
        {
            return new Vector2(vector2.x + x, vector2.y + y);
        }


        /// <summary>
        /// Vector2의 x 또는 y 값을 설정합니다.
        /// 지정하지 않은 값은 기존 값을 유지합니다.
        /// </summary>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector2.x, y ?? vector2.y);
        }

        /// <summary>
        /// 현재 Vector2가 주어진 Vector2로부터 특정 거리 이내에 있는지를 확인합니다.
        /// </summary>
        /// <param name="current">현재 Vector2 위치</param>
        /// <param name="target">비교 대상 Vector2 위치</param>
        /// <param name="range">비교할 거리 값</param>
        /// <returns>현재 Vector2가 대상 위치로부터 범위 이내에 있으면 true, 아니면 false</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }
        
        /// <summary>
        /// 중심점(origin)을 기준으로 최소 반지름과 최대 반지름 사이의 고리 모양 영역(annulus)에서
        /// 임의의 점을 계산하여 반환합니다.
        /// </summary>
        /// <param name="origin">고리 중심이 되는 Vector2</param>
        /// <param name="minRadius">최소 반지름</param>
        /// <param name="maxRadius">최대 반지름</param>
        /// <returns>고리 내부에서 임의로 선택된 Vector2 위치</returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius)
        {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // Calculate the position vector
            Vector2 position = direction * distance;
            return origin + position;
        }
    }
}