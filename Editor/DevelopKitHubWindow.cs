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

        [MenuItem("PJDev/DevelopKit Hub", priority = -1000)]
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
            if (string.IsNullOrEmpty(packageInfo?.version))
                return false;

            var latestVersion = await FetchRemotePackageVersionAsync(packageUrl);
            if (string.IsNullOrEmpty(latestVersion))
                return false;

            return CompareVersions(latestVersion, packageInfo.version) > 0;
        }

        private static int CompareVersions(string left, string right)
        {
            var leftParts = ParseVersion(left);
            var rightParts = ParseVersion(right);

            for (var i = 0; i < 3; i++)
            {
                var cmp = leftParts[i].CompareTo(rightParts[i]);
                if (cmp != 0)
                    return cmp;
            }

            return 0;
        }

        private static int[] ParseVersion(string version)
        {
            var core = version.Split('-')[0];
            var parts = core.Split('.');

            return new[]
            {
                parts.Length > 0 && int.TryParse(parts[0], out var major) ? major : 0,
                parts.Length > 1 && int.TryParse(parts[1], out var minor) ? minor : 0,
                parts.Length > 2 && int.TryParse(parts[2], out var patch) ? patch : 0,
            };
        }

        private static async Task<string> FetchRemotePackageVersionAsync(string packageUrl)
        {
            if (!TryParseGitHubUrl(packageUrl, out var owner, out var repo, out var gitRef, out var packagePath))
                return null;

            var commitHash = await FetchCommitHashAsync(owner, repo, gitRef);
            if (string.IsNullOrEmpty(commitHash))
                return null;

            var packageJsonPath = string.IsNullOrEmpty(packagePath)
                ? "package.json"
                : $"{packagePath.Trim('/')}/package.json";
            var rawUrl =
                $"https://raw.githubusercontent.com/{owner}/{repo}/{commitHash}/{packageJsonPath}";

            using var request = UnityWebRequest.Get(rawUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    $"Failed to fetch package version from '{rawUrl}': {request.error}");
                return null;
            }

            var match = Regex.Match(request.downloadHandler.text, "\"version\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static async Task<string> FetchCommitHashAsync(string owner, string repo, string gitRef)
        {
            if (string.IsNullOrEmpty(gitRef) || gitRef.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            {
                gitRef = await FetchDefaultBranchAsync(owner, repo);
                if (string.IsNullOrEmpty(gitRef))
                    return null;
            }

            if (IsCommitHash(gitRef))
                return gitRef;

            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/{gitRef}";
            using var request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    $"Failed to resolve commit hash for '{owner}/{repo}@{gitRef}': {request.error}");
                return null;
            }

            var match = Regex.Match(request.downloadHandler.text, "\"sha\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static bool IsCommitHash(string value)
        {
            if (value.Length < 7 || value.Length > 40)
                return false;

            foreach (var c in value)
            {
                var isHex = (c >= '0' && c <= '9') ||
                            (c >= 'a' && c <= 'f') ||
                            (c >= 'A' && c <= 'F');
                if (!isHex)
                    return false;
            }

            return true;
        }

        private static async Task<string> FetchDefaultBranchAsync(string owner, string repo)
        {
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}";
            using var request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                return null;

            var match = Regex.Match(
                request.downloadHandler.text,
                "\"default_branch\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static bool TryParseGitHubUrl(
            string packageUrl,
            out string owner,
            out string repo,
            out string gitRef,
            out string packagePath)
        {
            owner = null;
            repo = null;
            gitRef = "HEAD";
            packagePath = null;

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
                gitRef = uri.Fragment.TrimStart('#');

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var pathMatch = Regex.Match(uri.Query, "[?&]path=([^&]+)");
                if (pathMatch.Success)
                    packagePath = Uri.UnescapeDataString(pathMatch.Groups[1].Value);
            }

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
