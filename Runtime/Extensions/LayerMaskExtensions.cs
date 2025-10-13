using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// 주어진 레이어 번호가 LayerMask에 포함되어 있는지 확인합니다.
        /// </summary>
        /// <param name="mask">확인할 LayerMask입니다.</param>
        /// <param name="layerNumber">LayerMask에 포함되어 있는지 확인할 레이어 번호입니다.</param>
        /// <returns>레이어 번호가 LayerMask에 포함되어 있으면 true, 그렇지 않으면 false를 반환합니다.</returns>
        public static bool Contains(this LayerMask mask, int layerNumber)
        {
            return mask == (mask | (1 << layerNumber));
        }
    }
}