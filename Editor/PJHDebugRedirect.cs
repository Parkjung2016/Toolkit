using System;
using System.Reflection;
using System.Text.RegularExpressions;
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
            string name = EditorUtility.InstanceIDToObject(instance).name;
            if (name != nameof(PJHDebug)) return false;

            string stack_trace = GetStackTrace();
            if (!string.IsNullOrEmpty(stack_trace))
            {
                MatchCollection matches = Regex.Matches(stack_trace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    string pathline = match.Groups[1].Value.Trim();
                    if (!pathline.Contains("PJHDebug.cs")) // ✅ PJHDebug.cs 제외
                    {
                        int split_index = pathline.LastIndexOf(":");
                        if (split_index > 0)
                        {
                            string path = pathline.Substring(0, split_index);
                            line = Convert.ToInt32(pathline.Substring(split_index + 1));

                            string fullpath =
                                Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullpath, line);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static string GetStackTrace()
        {
            var assembly_unity_editor = Assembly.GetAssembly(typeof(EditorWindow));
            if (assembly_unity_editor == null) return null;

            var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
            if (type_console_window == null) return null;
            var field_console_window =
                type_console_window.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (field_console_window == null) return null;
            var instance_console_window = field_console_window.GetValue(null);
            if (instance_console_window == null) return null;

            if ((object)EditorWindow.focusedWindow == instance_console_window)
            {
                var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
                if (type_list_view_state == null) return null;

                var field_list_view =
                    type_console_window.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field_list_view == null) return null;

                var value_list_view = field_list_view.GetValue(instance_console_window);
                if (value_list_view == null) return null;

                var field_active_text =
                    type_console_window.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field_active_text == null) return null;

                string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
                return value_active_text;
            }

            return null;
        }
    }
}