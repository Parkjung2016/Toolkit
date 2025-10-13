using UnityEngine;

namespace PJH.Utility.Utils
{
    public static class VectorMath
    {
        /// <summary>
        /// 평면 위에서 두 벡터 사이의 부호 있는 각도를 계산합니다.
        /// </summary>
        /// <param name="vector1">첫 번째 벡터입니다.</param>
        /// <param name="vector2">두 번째 벡터입니다.</param>
        /// <param name="planeNormal">각도를 계산할 평면의 법선 벡터입니다.</param>
        /// <returns>두 벡터 사이의 부호 있는 각도를 반환합니다.</returns>
        public static float GetAngle(Vector3 vector1, Vector3 vector2, Vector3 planeNormal) {
            var angle = Vector3.Angle(vector1, vector2);
            var sign = Mathf.Sign(Vector3.Dot(planeNormal, Vector3.Cross(vector1, vector2)));
            return angle * sign;
        }

        /// <summary>
        /// 벡터와 정규화된 방향 벡터의 내적을 계산합니다.
        /// </summary>
        /// <param name="vector">투영할 벡터입니다.</param>
        /// <param name="direction">투영할 기준 방향 벡터입니다.</param>
        /// <returns>벡터와 방향 벡터의 내적 값을 반환합니다.</returns>
        public static float GetDotProduct(Vector3 vector, Vector3 direction) => 
            Vector3.Dot(vector, direction.normalized);

        /// <summary>
        /// 주어진 방향 벡터를 따라가는 성분을 제거한 벡터를 반환합니다.
        /// </summary>
        /// <param name="vector">성분을 제거할 원래 벡터입니다.</param>
        /// <param name="direction">제거할 방향 벡터입니다.</param>
        /// <returns>지정한 방향 성분이 제거된 벡터를 반환합니다.</returns>
        public static Vector3 RemoveDotVector(Vector3 vector, Vector3 direction) {
            direction.Normalize();
            return vector - direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// 주어진 방향 벡터를 따라가는 성분만 추출하여 반환합니다.
        /// </summary>
        /// <param name="vector">성분을 추출할 원래 벡터입니다.</param>
        /// <param name="direction">성분을 추출할 기준 방향 벡터입니다.</param>
        /// <returns>지정한 방향에 해당하는 벡터 성분을 반환합니다.</returns>
        public static Vector3 ExtractDotVector(Vector3 vector, Vector3 direction) {
            direction.Normalize();
            return direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// 지정한 평면(법선 기준) 위로 벡터를 회전시킵니다.
        /// </summary>
        /// <param name="vector">회전시킬 원래 벡터입니다.</param>
        /// <param name="planeNormal">회전 대상 평면의 법선 벡터입니다.</param>
        /// <param name="upDirection">현재의 '위쪽' 방향으로 사용될 기준 방향입니다.</param>
        /// <returns>회전된 결과 벡터를 반환합니다.</returns>
        public static Vector3 RotateVectorOntoPlane(Vector3 vector, Vector3 planeNormal, Vector3 upDirection) {
            // Calculate rotation;
            var rotation = Quaternion.FromToRotation(upDirection, planeNormal);

            // Apply rotation to vector;
            vector = rotation * vector;

            return vector;
        }

        /// <summary>
        /// 시작 위치와 방향으로 정의된 선분에 주어진 점을 투영합니다.
        /// </summary>
        /// <param name="lineStartPosition">선의 시작 위치입니다.</param>
        /// <param name="lineDirection">선의 방향 벡터입니다. (정규화되어 있어야 함)</param>
        /// <param name="point">선 위로 투영할 대상 점입니다.</param>
        /// <returns>원래 점에서 가장 가까운 선 위의 투영된 위치를 반환합니다.</returns>
        public static Vector3 ProjectPointOntoLine(Vector3 lineStartPosition, Vector3 lineDirection, Vector3 point) {
            var projectLine = point - lineStartPosition;
            var dotProduct = Vector3.Dot(projectLine, lineDirection);

            return lineStartPosition + lineDirection * dotProduct;
        }

        /// <summary>
        /// 현재 벡터를 일정 속도로 목표 벡터 방향으로 이동시킵니다.
        /// </summary>
        /// <param name="currentVector">현재 위치한 벡터입니다.</param>
        /// <param name="speed">목표로 향할 속도입니다.</param>
        /// <param name="deltaTime">이동에 적용할 시간 간격입니다.</param>
        /// <param name="targetVector">접근할 목표 벡터입니다.</param>
        /// <returns>주어진 속도와 시간에 따라 목표에 가까워진 새로운 벡터를 반환합니다.</returns>
        public static Vector3 IncrementVectorTowardTargetVector(Vector3 currentVector, float speed, float deltaTime,
            Vector3 targetVector) {
            return Vector3.MoveTowards(currentVector, targetVector, speed * deltaTime);
        }
    }
}