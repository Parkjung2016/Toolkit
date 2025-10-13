using UnityEngine;

namespace PJH.Utility.Utils
{
    public static class RayCastUtil
    {
        /// <summary>
        /// 메인 카메라에서 화면 위치를 기준으로 Ray를 생성합니다.
        /// </summary>
        /// <param name="pos">화면상의 위치(픽셀 단위)입니다.</param>
        /// <returns>해당 화면 위치를 통과하는 Ray를 반환합니다.</returns>
        public static Ray GetScreenPointToRay(Vector3 pos)
        {
            return Camera.main.ScreenPointToRay(pos);
        }

        /// <summary>
        /// 월드 좌표를 화면 좌표로 변환합니다.
        /// </summary>
        /// <param name="pos">월드 위치입니다.</param>
        /// <returns>월드 위치에 대응하는 화면 위치를 반환합니다.</returns>
        public static Vector3 GetWorldToScreenPoint(Vector3 pos)
        {
            return Camera.main.WorldToScreenPoint(pos);
        }

        /// <summary>
        /// 화면 위치에서 레이캐스트를 실행하여 특정 레이어에 포함된 오브젝트를 감지합니다.
        /// </summary>
        /// <param name="aimPos">레이캐스트를 시작할 화면 위치입니다.</param>
        /// <param name="whatIsMask">레이캐스트가 감지할 레이어 마스크입니다.</param>
        /// <param name="hit">히트된 오브젝트 정보가 저장될 출력 매개변수입니다.</param>
        /// <returns>지정된 레이어 내 오브젝트를 히트하면 true, 아니면 false를 반환합니다.</returns>
        public static bool GetMousePositionRaycast(Vector3 aimPos, LayerMask whatIsMask, out RaycastHit hit)
        {
            Vector3 mousePos = aimPos;
            mousePos.z = Camera.main.nearClipPlane;

            Ray ray = GetScreenPointToRay(mousePos);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsMask);
        }

        /// <summary>
        /// 화면 위치에서 레이캐스트를 쏴서 히트한 지점의 월드 좌표를 반환합니다.
        /// </summary>
        /// <param name="aimPos">레이캐스트를 시작할 화면 위치입니다.</param>
        /// <param name="layerMask">레이캐스트가 감지할 레이어 마스크입니다.</param>
        /// <returns>히트한 위치의 월드 좌표를 반환하며, 히트하지 못하면 기본값(Vector3.zero)을 반환합니다.</returns>
        public static Vector3 GetMouseRayPoint(Vector3 aimPos, LayerMask layerMask)
        {
            if (GetMousePositionRaycast(aimPos, layerMask, out RaycastHit hit))
            {
                return hit.point;
            }

            return default;
        }

        /// <summary>
        /// 주어진 스크린 좌표를 기준으로, Vector3.up 평면과의 교차점을 월드 좌표로 반환합니다.
        /// </summary>
        /// <param name="position">스크린 좌표 (예: Input.mousePosition)</param>
        /// <returns>평면과의 교차 지점의 월드 좌표. 교차하지 않으면 Vector3.zero 반환.</returns>
        public static Vector3 GetWorldPointOnPlane(Vector3 position)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(position);

            Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);

            if (GroupPlane.Raycast(cameraRay, out float rayLength))

            {
                Vector3 pointTolook = cameraRay.GetPoint(rayLength);
                return pointTolook;
            }

            return default;
        }
    }
}