using UnityEngine;

namespace PJH.Utility.Utils.CoroutineHelpers
{
    public class YieldCache
    {
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            return WaitFor.Seconds(seconds);
        }

        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float seconds)
        {
            return WaitFor.SecondsRealtime(seconds);
        }
    }
}