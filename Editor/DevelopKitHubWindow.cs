using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PJDev.DevelopKit.Editors
{
    public class DevelopKitHubWindow : EditorWindow
    {
        private const string basicTemplatePackageUrl =
            "https://github.com/Parkjung2016/DevelopKit_BasicTemplate.git";

        private const string basicTemplatePackageName =
            "com.pjdev.developkit.basictemplate";

        private const string frameworkPackageUrl =
            "https://github.com/Parkjung2016/DevelopKit_Framework.git";

        private const string frameworkPackageName =
            "com.pjdev.developkit.framework";

        private static readonly Vector2 windowSize = new Vector2(500, 500);

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement dimmed;

        [MenuItem("PJDev/DevelopKit Hub")]
        public static void ShowExample()
        {
            var wnd = GetWindow<DevelopKitHubWindow>();
            wnd.titleContent = new GUIContent("DevelopKit Hub");
            wnd.minSize = windowSize;
            wnd.maxSize = windowSize + Vector2.one * 0.1f;
        }

        public async void CreateGUI()
        {
            rootVisualElement.Clear();
            await BuildUIAsync();
        }

        private async Task BuildUIAsync()
        {
            var installedPackages = await GetInstalledPackages();

            var root = rootVisualElement;
            var uxml = m_VisualTreeAsset.Instantiate();
            dimmed = uxml.Q("Dimmed");
            await SetPackageSectionAsync(
                installedPackages,
                uxml.Q("BasicTemplate"),
                basicTemplatePackageName,
                basicTemplatePackageUrl,
                "Basic Template");
            await SetPackageSectionAsync(
                installedPackages,
                uxml.Q("Framework"),
                frameworkPackageName,
                frameworkPackageUrl,
                "Framework");
            SetDimmed(false);
            root.Add(uxml);
        }

        private void SetDimmed(bool isDimmed)
        {
            dimmed.visible = isDimmed;
        }

        private async Task SetPackageSectionAsync(
            PackageCollection installed,
            VisualElement section,
            string packageName,
            string packageUrl,
            string displayName)
        {
            var background = section.Q("Background");
            var btn = background.Q<Button>("Btn_Install");
            var updateBtn = background.Q<Button>("Btn_Update");
            updateBtn.style.display = DisplayStyle.None;

            var packageInfo = installed.FirstOrDefault(p => p.name == packageName);

            if (packageInfo != null)
            {
                btn.text = $"Remove {displayName}";
                SetSectionRemoveable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true);
                    try
                    {
                        await RemovePackageAsync(packageName);
                        await BuildUIAsync();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        SetDimmed(false);
                    }
                };

                if (await IsUpdateAvailableAsync(packageInfo, packageUrl))
                {
                    updateBtn.text = $"Update {displayName}";
                    updateBtn.style.display = DisplayStyle.Flex;
                    SetSectionUpdatable(section);
                    updateBtn.clicked += async () =>
                    {
                        SetDimmed(true);
                        try
                        {
                            await UpdatePackageAsync(packageUrl);
                            await BuildUIAsync();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                            SetDimmed(false);
                        }
                    };
                }
            }
            else
            {
                btn.text = $"Install {displayName}";
                SetSectionInstallable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true);
                    try
                    {
                        await InstallPackageAsync(packageUrl);
                        await BuildUIAsync();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        SetDimmed(false);
                    }
                };
            }
        }

        private static async Task<bool> IsUpdateAvailableAsync(PackageInfo packageInfo, string packageUrl)
        {
            if (packageInfo?.git == null || string.IsNullOrEmpty(packageInfo.git.hash))
                return false;

            var latestHash = await FetchLatestGitHubCommitHashAsync(packageUrl);
            if (string.IsNullOrEmpty(latestHash))
                return false;

            return !HashesMatch(packageInfo.git.hash, latestHash);
        }

        private static bool HashesMatch(string installedHash, string latestHash)
        {
            var installed = installedHash.Trim().ToLowerInvariant();
            var latest = latestHash.Trim().ToLowerInvariant();
            var length = Math.Min(installed.Length, latest.Length);
            return string.Compare(
                       installed,
                       0,
                       latest,
                       0,
                       length,
                       StringComparison.Ordinal) == 0;
        }

        private static async Task<string> FetchLatestGitHubCommitHashAsync(string packageUrl)
        {
            if (!TryParseGitHubUrl(packageUrl, out var owner, out var repo, out var revision))
                return null;

            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/{revision}";
            using var request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    $"Failed to check package update from '{apiUrl}': {request.error}");
                return null;
            }

            var match = Regex.Match(request.downloadHandler.text, "\"sha\"\\s*:\\s*\"([a-f0-9]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static bool TryParseGitHubUrl(
            string packageUrl,
            out string owner,
            out string repo,
            out string revision)
        {
            owner = null;
            repo = null;
            revision = "HEAD";

            if (string.IsNullOrEmpty(packageUrl))
                return false;

            if (!Uri.TryCreate(packageUrl, UriKind.Absolute, out var uri))
                return false;

            if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                return false;

            var parts = uri.AbsolutePath.Trim('/').Split('/');
            if (parts.Length < 2)
                return false;

            owner = parts[0];
            repo = parts[1].EndsWith(".git", StringComparison.OrdinalIgnoreCase)
                ? parts[1][..^4]
                : parts[1];

            if (!string.IsNullOrEmpty(uri.Fragment))
                revision = uri.Fragment.TrimStart('#');

            return true;
        }

        private void SetSectionInstallable(VisualElement section)
        {
            section.RemoveFromClassList("removeable");
            section.RemoveFromClassList("updatable");
            section.AddToClassList("installable");
        }

        private void SetSectionRemoveable(VisualElement section)
        {
            section.RemoveFromClassList("installable");
            section.RemoveFromClassList("updatable");
            section.AddToClassList("removeable");
        }

        private void SetSectionUpdatable(VisualElement section)
        {
            section.AddToClassList("updatable");
        }

        private Task InstallPackageAsync(string url) => AddPackageAsync(url);

        private Task UpdatePackageAsync(string url) => AddPackageAsync(url);

        private Task AddPackageAsync(string url)
        {
            var tcs = new TaskCompletionSource<bool>();
            AddRequest request = Client.Add(url);

            EditorApplication.update += Progress;

            void Progress()
            {
                if (!request.IsCompleted) return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    tcs.SetResult(true);
                else
                    tcs.SetException(new Exception(request.Error.message));
            }

            return tcs.Task;
        }

        private Task RemovePackageAsync(string name)
        {
            var tcs = new TaskCompletionSource<bool>();
            RemoveRequest request = Client.Remove(name);

            EditorApplication.update += Progress;

            void Progress()
            {
                if (!request.IsCompleted) return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    tcs.SetResult(true);
                else
                    tcs.SetException(new Exception(request.Error.message));
            }

            return tcs.Task;
        }

        private async Task<PackageCollection> GetInstalledPackages()
        {
            var tcs = new TaskCompletionSource<PackageCollection>();
            ListRequest request = Client.List(true);

            EditorApplication.update += Progress;

            void Progress()
            {
                if (!request.IsCompleted) return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    tcs.SetResult(request.Result);
                else
                    tcs.SetException(new Exception(request.Error.message));
            }

            return await tcs.Task;
        }
    }
}
