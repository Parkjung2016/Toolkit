using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// 주어진 회전(Quaternion)에서 지정한 축의 회전만 유지한 새로운 Quaternion을 반환합니다.
        /// <paramref name="keepAxis"/>의 각 값이 0이 아닌 축만 유지되며, 나머지 축은 0으로 설정됩니다.
        /// 예를 들어, Vector3.up (0, 1, 0)을 전달하면 Y축 회전만 유지됩니다.
        /// </summary>
        /// <param name="q">원본 Quaternion입니다.</param>
        /// <param name="keepAxis">
        /// 각 축의 회전을 유지할지 여부를 나타내는 Vector3입니다.  
        /// 값이 0이 아니면 해당 축의 회전을 유지하고, 0이면 제거됩니다.
        /// </param>
        /// <returns>지정한 축 회전만 유지한 새로운 Quaternion을 반환합니다.</returns>
        public static Quaternion KeepOnlyAxisRotation(this Quaternion q, Vector3 keepAxis)
        {
            Vector3 euler = q.eulerAngles;
            return Quaternion.Euler(
                keepAxis.x != 0 ? euler.x : 0f,
                keepAxis.y != 0 ? euler.y : 0f,
                keepAxis.z != 0 ? euler.z : 0f
            );
        }
    }
}