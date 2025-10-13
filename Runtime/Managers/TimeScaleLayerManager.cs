using System.Collections.Generic;
using UnityEngine;

namespace PJH.Utility.Managers
{
    public static class TimeScaleLayerManager
    {
        private class LayerData
        {
            public float scale;
            public int priority;
        }

        private static Dictionary<string, LayerData> timeScaleLayers = new Dictionary<string, LayerData>();

        /// <param name="key">Layer 이름</param>
        /// <param name="scale">적용할 TimeScale 값</param>
        /// <param name="priority">우선순위 (높을수록 우선)</param>
        public static void SetLayer(string key, float scale, int priority = 0)
        {
            timeScaleLayers[key] = new LayerData { scale = scale, priority = priority };
            Apply();
        }

        public static void RemoveLayer(string key)
        {
            timeScaleLayers.Remove(key);

            Apply();
        }

        private static void Apply()
        {
            if (timeScaleLayers.Count == 0)
            {
                Time.timeScale = 1f;
                return;
            }

            int maxPriority = int.MinValue;
            foreach (var kv in timeScaleLayers)
                maxPriority = Mathf.Max(maxPriority, kv.Value.priority);

            float finalScale = 1f;
            foreach (var kv in timeScaleLayers)
            {
                if (kv.Value.priority == maxPriority)
                    finalScale = Mathf.Min(finalScale, kv.Value.scale);
            }

            Time.timeScale = finalScale;
        }
    }
}