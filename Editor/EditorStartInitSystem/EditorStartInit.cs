using PJH.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PJH.Editor
{
    [InitializeOnLoad]
    public class EditorStartInit
    {
        private const string editorStartInitSettingSOFilePath =
            "Assets/EditorStartInitSystem/Editor/EditorStartInitSettingSO.asset";

        private static EditorStartInitSettingSO editorStartInitSetting;

        static EditorStartInit()
        {
            Init();
        }

        private static void Init()
        {
            GetEditorStartInitSetting();

            ApplyPlayModeStartScene();
        }

        private static void GetEditorStartInitSetting()
        {
            if (AssetDatabase.AssetPathExists(editorStartInitSettingSOFilePath))
                editorStartInitSetting =
                    AssetDatabase.LoadAssetAtPath<EditorStartInitSettingSO>(editorStartInitSettingSOFilePath);
            else
            {
                EditorStartInitSettingSO editorStartInitSettingInstance =
                    ScriptableObject.CreateInstance<EditorStartInitSettingSO>();
                AssetDatabase.CreateAsset(editorStartInitSettingInstance, editorStartInitSettingSOFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                editorStartInitSetting = editorStartInitSettingInstance;
            }
        }

        [MenuItem("SetupScene/Use Setup Scene")]
        private static void ToggleUseSetupScene()
        {
            editorStartInitSetting.useSetupScene = !editorStartInitSetting.useSetupScene;
            ApplyPlayModeStartScene();
        }

        [MenuItem("SetupScene/Use Setup Scene", true)]
        private static bool ToggleUseSetupSceneValidate()
        {
            if (editorStartInitSetting == null)
            {
                Init();
            }

            Menu.SetChecked("SetupScene/Use Setup Scene", editorStartInitSetting.useSetupScene);
            return true;
        }

        private static void ApplyPlayModeStartScene()
        {
            if (editorStartInitSetting == null) return;
            if (editorStartInitSetting.useSetupScene)
            {
                var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
                EditorSceneManager.playModeStartScene = sceneAsset;
                PJHDebug.Log($"▶ Setup 씬에서 시작: {pathOfFirstScene}");
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
                PJHDebug.Log("▶ 현재 씬에서 시작");
            }

            EditorUtility.SetDirty(editorStartInitSetting);
            AssetDatabase.SaveAssets();
        }
    }
}