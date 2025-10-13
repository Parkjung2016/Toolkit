using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class RendererExtensions {
        /// <summary>
        /// 이 Renderer에 포함된 머티리얼 중 '_Color' 속성이 있는 머티리얼에 대해 ZWrite를 활성화합니다.
        /// 이는 해당 머티리얼이 Z 버퍼에 기록하도록 하며, 이후 렌더링 처리 방식에 영향을 줄 수 있습니다.
        /// 예를 들어, 투명한 오브젝트의 올바른 레이어링을 보장하는 데 사용될 수 있습니다.
        /// </summary>
        public static void EnableZWrite(this Renderer renderer) {
            foreach (Material material in renderer.materials) {
                if (material.HasProperty("_Color")) {
                    material.SetInt("_ZWrite", 1);
                    material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
                }
            }
        }

        /// <summary>
        /// 이 Renderer에 포함된 머티리얼 중 '_Color' 속성이 있는 머티리얼에 대해 ZWrite를 비활성화합니다.
        /// 이는 해당 머티리얼이 Z 버퍼에 기록하지 않도록 하며, 반투명하거나 여러 레이어로 구성된 오브젝트를 
        /// 렌더링할 때 이후 렌더링이 가려지지 않도록 하기 위해 필요한 경우가 있습니다.
        /// </summary>
        public static void DisableZWrite(this Renderer renderer) {
            foreach (Material material in renderer.materials) {
                if (material.HasProperty("_Color")) {
                    material.SetInt("_ZWrite", 0);
                    material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent + 100;
                }
            }
        }
    }
}