using System.IO;
using UnityEditor;
using UnityEngine;

namespace PJH.Utility.Editor
{
    [InitializeOnLoad]
    public class BasicTemplateToolbarExtender : UnityEditor.Editor
    {
        private const string UnitaskName = "com.cysharp.unitask";

        private const string UnitaskUrl =
            "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";

        static BasicTemplateToolbarExtender()
        {
            bool checkUnitaskInstalled = CheckPackageInstalled(UnitaskName);
            if (!checkUnitaskInstalled)
            {
                AddPackage(UnitaskName, UnitaskUrl);
            }
        }

        private static void AddPackage(string name, string url)
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty),
                "Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"manifest.json not found at '{manifestPath}'");
                return;
            }

            string manifestText = File.ReadAllText(manifestPath);
            if (!manifestText.Contains(UnitaskName))
            {
                Debug.Log($"{UnitaskName} not found in manifest.json");
                var modifiedText = manifestText.Insert(manifestText.IndexOf("dependencies") + 17,
                    $"\t\"{UnitaskName}\": \"{UnitaskUrl}\",\n");
                File.WriteAllText(manifestPath, modifiedText);
                Debug.Log($"Added {UnitaskName} to manifest.json");
            }

            UnityEditor.PackageManager.Client.Resolve();
        }

        private static bool CheckPackageInstalled(string packageName)
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty),
                "Packages/manifest.json");
            string manifestText = File.ReadAllText(manifestPath);
            return manifestText.Contains(packageName);
        }
    }
}