using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PJHDebugRedirectInstaller
{
    private const string PackageName = "com.pjh.toolkit";
    private const string FileName = "PJHDebugRedirect.cs";
    private const string TargetFolder = "Assets/Editor";
    private static readonly string TargetPath = Path.Combine(TargetFolder, FileName);

    static PJHDebugRedirectInstaller()
    {
        EditorApplication.delayCall += EnsureFileInstalled;
        UnityEditor.PackageManager.Events.registeredPackages += OnPackagesChanged;
    }

    private static void OnPackagesChanged(UnityEditor.PackageManager.PackageRegistrationEventArgs args)
    {
        if (args.added != null)
        {
            foreach (var package in args.added)
            {
                if (package.name == PackageName)
                {
                    EnsureFileInstalled();
                    return;
                }
            }
        }

        if (args.removed != null)
        {
            foreach (var package in args.removed)
            {
                if (package.name == PackageName)
                {
                    RemoveFile();
                    return;
                }
            }
        }
    }

    private static void EnsureFileInstalled()
    {
        string packagePath = FindPackagePath();
        if (string.IsNullOrEmpty(packagePath)) return;

        string sourcePath = Path.Combine(packagePath, "Editor", FileName);
        string destPath = Path.Combine(Application.dataPath, "Editor", FileName);

        if (!File.Exists(sourcePath))
        {
            Debug.LogWarning($"[PJH Toolkit] 원본 파일을 찾을 수 없습니다: {sourcePath}");
            return;
        }

        string editorDir = Path.Combine(Application.dataPath, "Editor");
        if (!Directory.Exists(editorDir))
            Directory.CreateDirectory(editorDir);

        if (!File.Exists(destPath))
        {
            File.Copy(sourcePath, destPath, true);
            Debug.Log($"[PJH Toolkit] {FileName} 자동 생성됨");
            AssetDatabase.Refresh();
        }
    }

    private static void RemoveFile()
    {
        string destPath = Path.Combine(Application.dataPath, "Editor", FileName);
        if (File.Exists(destPath))
        {
            File.Delete(destPath);
            Debug.Log($"[PJH Toolkit] {FileName} 자동 삭제됨");
            AssetDatabase.Refresh();
        }
    }

    private static string FindPackagePath()
    {
        string cacheRoot = Path.Combine(Application.dataPath, "../Library/PackageCache");
        if (Directory.Exists(cacheRoot))
        {
            foreach (var dir in Directory.GetDirectories(cacheRoot))
            {
                if (Path.GetFileName(dir).StartsWith(PackageName))
                    return dir;
            }
        }

        string localPath = Path.Combine(Application.dataPath, $"../Packages/{PackageName}");
        if (Directory.Exists(localPath))
            return localPath;

        return null;
    }
}