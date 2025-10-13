using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PJH.Utility.CustomDebug
{
    public struct ReplaceMode
    {
        public ReplaceModeType mode;
        public int nthIndex;

        public ReplaceMode(ReplaceModeType mode, int nth = 1)
        {
            this.mode = mode;
            nthIndex = Math.Max(1, nth);
        }

        public static ReplaceMode All => new ReplaceMode(ReplaceModeType.ReplaceAll);
        public static ReplaceMode First => new ReplaceMode(ReplaceModeType.ReplaceFirst);
        public static ReplaceMode Nth(int n) => new ReplaceMode(ReplaceModeType.ReplaceNth, n);
    }

    public enum ReplaceModeType
    {
        ReplaceAll,
        ReplaceFirst,
        ReplaceNth
    }

    public static class PJHDebug
    {
        #region 화면 로그용 내부 MonoBehaviour

        private class FloatingDebug : MonoBehaviour
        {
            public class LogMessage
            {
                public string message;
                public float expireTime;
                public Color color;
            }

            private List<LogMessage> messages = new List<LogMessage>();
            private static FloatingDebug _instance;

            public static FloatingDebug Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        var go = new GameObject("PJHFloatingDebug");
                        _instance = go.AddComponent<FloatingDebug>();
                        UnityEngine.Object.DontDestroyOnLoad(go);
                    }

                    return _instance;
                }
            }

            public void AddMessage(string message, Color? color = null, float duration = 2f)
            {
                messages.Add(new LogMessage
                {
                    message = message,
                    expireTime = Time.time + duration,
                    color = color ?? Color.white
                });
            }
    
            private void OnGUI()
            {
                GUIStyle style = new GUIStyle();

                style.fontSize = Mathf.RoundToInt(Screen.height * 0.02f);
                style.normal.textColor = Color.white;

                float y = 10f;
                for (int i = messages.Count - 1; i >= 0; i--)
                {
                    var log = messages[i];
                    if (Time.time > log.expireTime)
                    {
                        messages.RemoveAt(i);
                        continue;
                    }

                    style.normal.textColor = log.color;

                    float labelHeight = style.fontSize + 5;
                    GUI.Label(new Rect(10, y, 1000, labelHeight), log.message, style);
                    y += labelHeight + 5;
                }

                style.normal.textColor = Color.white;
            }
        }

        #endregion

        #region 기본 로그

        [Conditional("ENABLE_LOG")]
        public static void Log(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            Color? color = null)
        {
            string msg = BuildMessage(message, prefixNumber, tag, showTimestamp);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(msg, color, duration);
            else
                UnityEngine.Debug.Log(msg, context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarning(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            Color? color = null)
        {
            string msg = BuildMessage(message, prefixNumber, tag, showTimestamp);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(msg, color ?? Color.yellow, duration);
            else
                UnityEngine.Debug.LogWarning(msg, context);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogError(object message, UnityEngine.Object context = null, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            Color? color = null)
        {
            string msg = BuildMessage(message, prefixNumber, tag, showTimestamp);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(msg, color ?? Color.red, duration);
            else
                UnityEngine.Debug.LogError(msg, context);
        }

        #endregion

        #region 색상 로그 (부분/전체 메시지 지원)

        [Conditional("ENABLE_LOG")]
        public static void LogColorPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(output, Color.white, duration);
            else
                UnityEngine.Debug.Log(output);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(output, Color.yellow, duration);
            else
                UnityEngine.Debug.LogWarning(output);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorPart(string message, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            string output = ApplyColors(BuildMessage(message, prefixNumber, tag, showTimestamp), colorInfos);
            if (showOnScreen)
                FloatingDebug.Instance.AddMessage(output, Color.red, duration);
            else
                UnityEngine.Debug.LogError(output);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f)
        {
            LogColorPart(message, prefixNumber, tag, showTimestamp, showOnScreen, duration,
                (message, color, ReplaceMode.All));
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f)
        {
            LogColorWarningPart(message, prefixNumber, tag, showTimestamp, showOnScreen, duration,
                (message, color, ReplaceMode.All));
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorPart(string message, Color color, int? prefixNumber = null, string tag = null,
            bool showTimestamp = false, bool showOnScreen = false, float duration = 2f)
        {
            LogColorErrorPart(message, prefixNumber, tag, showTimestamp, showOnScreen, duration,
                (message, color, ReplaceMode.All));
        }

        #endregion

        #region Format 로그

        [Conditional("ENABLE_LOG")]
        public static void LogFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params object[] args)
        {
            string message = string.Format(format, args);
            Log(message, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarningFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params object[] args)
        {
            string message = string.Format(format, args);
            LogWarning(message, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogErrorFormat(UnityEngine.Object context, string format, int? prefixNumber = null,
            string tag = null, bool showTimestamp = false, bool showOnScreen = false, float duration = 2f,
            params object[] args)
        {
            string message = string.Format(format, args);
            LogError(message, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        #endregion

        #region ColorFormat 로그

        [Conditional("ENABLE_LOG")]
        public static void LogColorFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, bool showOnScreen = false,
            float duration = 2f, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            Log(coloredMessage, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorWarningFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, bool showOnScreen = false,
            float duration = 2f, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            LogWarning(coloredMessage, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogColorErrorFormat(UnityEngine.Object context, Color color, string format,
            int? prefixNumber = null, string tag = null, bool showTimestamp = false, bool showOnScreen = false,
            float duration = 2f, params object[] args)
        {
            string message = string.Format(format, args);
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
            LogError(coloredMessage, context, prefixNumber, tag, showTimestamp, showOnScreen, duration);
        }

        #endregion

        #region Assert

        [Conditional("ENABLE_LOG")]
        public static void Assert(bool condition, string message = "", UnityEngine.Object context = null,
            int? prefixNumber = null, bool showTimestamp = false, bool showOnScreen = false,
            float duration = 2f)
        {
            if (!condition)
            {
                string msg = BuildMessage(message, prefixNumber, "Assert", showTimestamp);
                if (showOnScreen)
                    FloatingDebug.Instance.AddMessage(msg, Color.red, duration);
                else
                    UnityEngine.Debug.Assert(false, msg, context);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void AssertFormat(bool condition, UnityEngine.Object context, string format,
            int? prefixNumber = null, bool showTimestamp = false, bool showOnScreen = false,
            float duration = 2f, params object[] args)
        {
            if (!condition)
            {
                string message = string.Format(format, args);
                string msg = BuildMessage(message, prefixNumber, "Assert", showTimestamp);
                if (showOnScreen)
                    FloatingDebug.Instance.AddMessage(msg, Color.red, duration);
                else
                    UnityEngine.Debug.Assert(false, msg, context);
            }
        }

        #endregion

        #region 내부 유틸

        private static string BuildMessage(object message, int? prefixNumber, string tag, bool showTimestamp)
        {
            string numberPrefix = prefixNumber.HasValue ? $"#{prefixNumber.Value:000} " : "";
            string tagPrefix = !string.IsNullOrEmpty(tag) ? $"[{tag}] " : "";
            string timestamp = showTimestamp ? $"[{DateTime.Now:HH:mm:ss.fff}] " : "";
            return $"{numberPrefix}{tagPrefix}{timestamp}{message}";
        }

        private static string ApplyColors(string text, (string part, Color color, ReplaceMode mode)[] colorInfos)
        {
            if (string.IsNullOrEmpty(text) || colorInfos == null) return text;

            foreach (var (part, color, mode) in colorInfos)
            {
                if (string.IsNullOrEmpty(part)) continue;
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string coloredPart = $"<color=#{colorHex}>{part}</color>";

                switch (mode.mode)
                {
                    case ReplaceModeType.ReplaceAll:
                        text = text.Replace(part, coloredPart);
                        break;
                    case ReplaceModeType.ReplaceFirst:
                        int index = text.IndexOf(part, StringComparison.Ordinal);
                        if (index >= 0)
                            text = text.Substring(0, index) + coloredPart + text.Substring(index + part.Length);
                        break;
                    case ReplaceModeType.ReplaceNth:
                        int occurrence = 0;
                        int start = 0;
                        while ((index = text.IndexOf(part, start, StringComparison.Ordinal)) >= 0)
                        {
                            occurrence++;
                            if (occurrence == mode.nthIndex)
                            {
                                text = text.Substring(0, index) + coloredPart + text.Substring(index + part.Length);
                                break;
                            }

                            start = index + part.Length;
                        }

                        break;
                }
            }

            return text;
        }

        #endregion
    }
}