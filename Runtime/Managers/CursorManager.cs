using UnityEngine;

namespace PJH.Utility.Managers
{
    public static class CursorManager
    {
        /// <summary>
        /// 마우스 커서의 잠금 모드를 설정합니다.
        /// </summary>
        /// <param name="lockMode">설정할 CursorLockMode 값입니다. (None, Locked, Confined)</param>
        /// <remarks>
        /// CursorLockMode가 Locked일 경우 커서를 화면 중앙에 고정하고 보이지 않게 설정합니다.
        /// Locked가 아니면 커서를 보이도록 설정합니다.
        /// </remarks>
        public static void SetCursorLockMode(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;

            Cursor.visible = (lockMode != CursorLockMode.Locked);
        }
    }
}