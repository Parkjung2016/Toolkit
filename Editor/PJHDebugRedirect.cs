using System;
using System.Reflection;
using System.Text.RegularExpressions;
using PJH.Toolkit.CustomDebug;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PJH.Utility.Editor
{
    // --- 콘솔 클릭 시 커스텀 로그 위치로 이동 ---
    public class PJHDebugRedirect : AssetPostprocessor
    {
        [OnOpenAsset(0)]
        static bool OnOpenAsset(int instance, int line)
        {
            string stackTrace = GetStackTrace();
            if (string.IsNullOrEmpty(stackTrace))
                return false;

            MatchCollection matches = Regex.Matches(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                string pathLine = match.Groups[1].Value.Trim();
                if (!pathLine.Contains("PJHDebug.cs")) // ✅ PJHDebug.cs 제외
                {
                    int split_index = pathLine.LastIndexOf(":");
                    if (split_index > 0)
                    {
                        string path = pathLine.Substring(0, split_index);
                        line = Convert.ToInt32(pathLine.Substring(split_index + 1));

                        string fullPath =
                            Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, line);
                        return true;
                    }
                }

            }

            return false;
        }

        static string GetStackTrace()
        {
            Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            if (unityEditorAssembly == null) return null;

            Type consoleWindowType = unityEditorAssembly.GetType("UnityEditor.ConsoleWindow");
            FieldInfo fieldConsoleWindow = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            object consoleWindowInstance = fieldConsoleWindow?.GetValue(null);
            if (consoleWindowInstance == null) return null;

            // 현재 포커스된 창이 콘솔이 아닐 때는 무시
            if (EditorWindow.focusedWindow != consoleWindowInstance)
                return null;

            FieldInfo fieldActiveText = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldActiveText?.GetValue(consoleWindowInstance)?.ToString();
        }

    }
}