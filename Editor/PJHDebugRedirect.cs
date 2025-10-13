using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace PJH.Utility.Editor
{
    // --- 콘솔 클릭 시 커스텀 로그 위치로 이동 ---
    public class PJHDebugRedirect : AssetPostprocessor
    {
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.ToLower().Contains("[mark]"))
            {
                Match match = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string pathLine = match.Groups[1].Value;
                    int splitIndex = pathLine.LastIndexOf(":");
                    if (splitIndex > 0)
                    {
                        string path = pathLine.Substring(0, splitIndex);
                        int lineNumber = Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(
                            path.Replace('/', Path.DirectorySeparatorChar), lineNumber);
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetStackTrace()
        {
            var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var consoleInstance = fieldInfo.GetValue(null) as EditorWindow;
            if (consoleInstance != null && EditorWindow.focusedWindow == consoleInstance)
            {
                var listViewState = consoleWindowType.GetField("m_ListView",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.GetValue(consoleInstance);
                var rowField = listViewState.GetType().GetField("row",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                int row = (int)rowField.GetValue(listViewState);
                var activeTextField = consoleWindowType.GetField("m_ActiveText",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return activeTextField.GetValue(consoleInstance).ToString();
            }

            return null;
        }
    }
}
