using System.IO;
using UnityEditor;
using UnityEngine;

namespace PJH.Toolkit.Editor
{
    [InitializeOnLoad]
    public class PackageExtender : UnityEditor.Editor
    {
        private const string UnitaskName = "com.cysharp.unitask";
        private const string ImprovedTimerName = "com.gitamend.improvedtimers";

        private const string UnitaskUrl =
            "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";

        private const string ImprovedTimerUrl =
            "https://github.com/adammyhre/Unity-Improved-Timers.git";

        static PackageExtender()
        {
            bool checkUnitaskInstalled = CheckPackageInstalled(UnitaskName);
            if (!checkUnitaskInstalled)
            {
                AddPackage(UnitaskName, UnitaskUrl);
            }

            bool checkImprovedTimerInstalled = CheckPackageInstalled(ImprovedTimerName);
            if (!checkImprovedTimerInstalled)
            {
                AddPackage(ImprovedTimerName, ImprovedTimerUrl);
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
            if (!manifestText.Contains(name))
            {
                Debug.Log($"{name} not found in manifest.json");
                var modifiedText = manifestText.Insert(manifestText.IndexOf("dependencies") + 17,
                    $"\t\"{name}\": \"{url}\",\n");
                File.WriteAllText(manifestPath, modifiedText);
                Debug.Log($"Added {name} to manifest.json");
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