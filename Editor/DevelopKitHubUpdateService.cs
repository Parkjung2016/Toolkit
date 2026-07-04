using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PJDev.DevelopKit.Editors
{
    internal static class DevelopKitHubPackages
    {
        public readonly struct Entry
        {
            public Entry(string name, string url, string displayName)
            {
                Name = name;
                Url = url;
                DisplayName = displayName;
            }

            public string Name { get; }
            public string Url { get; }
            public string DisplayName { get; }
        }

        public const string DevelopKitName = "com.pjdev.developkit";
        public const string DevelopKitUrl = "https://github.com/Parkjung2016/DevelopKit.git";

        public const string BasicTemplateName = "com.pjdev.developkit.basictemplate";
        public const string BasicTemplateUrl = "https://github.com/Parkjung2016/DevelopKit_BasicTemplate.git";

        public const string FrameworkName = "com.pjdev.developkit.framework";
        public const string FrameworkUrl = "https://github.com/Parkjung2016/DevelopKit_Framework.git";

        public static readonly Entry[] Tracked =
        {
            new(DevelopKitName, DevelopKitUrl, "DevelopKit"),
            new(BasicTemplateName, BasicTemplateUrl, "Basic Template"),
            new(FrameworkName, FrameworkUrl, "Framework"),
        };
    }

    internal static class DevelopKitHubUpdateService
    {
        public static async Task<bool> HasAnyUpdateAvailableAsync()
        {
            PackageCollection installed = await GetInstalledPackagesAsync();

            foreach (DevelopKitHubPackages.Entry entry in DevelopKitHubPackages.Tracked)
            {
                PackageInfo packageInfo = installed.FirstOrDefault(p => p.name == entry.Name);
                if (packageInfo == null)
                    continue;

                if (await IsUpdateAvailableAsync(packageInfo, entry.Url))
                    return true;
            }

            return false;
        }

        public static async Task<bool> IsUpdateAvailableAsync(PackageInfo packageInfo, string packageUrl)
        {
            if (string.IsNullOrEmpty(packageInfo?.version))
                return false;

            string latestVersion = await FetchRemotePackageVersionAsync(packageUrl);
            if (string.IsNullOrEmpty(latestVersion))
                return false;

            return CompareVersions(latestVersion, packageInfo.version) > 0;
        }

        public static async Task<PackageCollection> GetInstalledPackagesAsync()
        {
            var completion = new TaskCompletionSource<PackageCollection>();
            ListRequest request = Client.List(true);

            void Progress()
            {
                if (!request.IsCompleted)
                    return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    completion.TrySetResult(request.Result);
                else
                    completion.TrySetException(new Exception(request.Error.message));
            }

            EditorApplication.update += Progress;
            return await completion.Task;
        }

        public static int CompareVersions(string left, string right)
        {
            int[] leftParts = ParseVersion(left);
            int[] rightParts = ParseVersion(right);

            for (int i = 0; i < 3; i++)
            {
                int cmp = leftParts[i].CompareTo(rightParts[i]);
                if (cmp != 0)
                    return cmp;
            }

            return 0;
        }

        public static async Task<string> FetchRemotePackageVersionAsync(string packageUrl)
        {
            if (!TryParseGitHubUrl(packageUrl, out string owner, out string repo, out string gitRef, out string packagePath))
                return null;

            string commitHash = await FetchCommitHashAsync(owner, repo, gitRef);
            if (string.IsNullOrEmpty(commitHash))
                return null;

            string packageJsonPath = string.IsNullOrEmpty(packagePath)
                ? "package.json"
                : $"{packagePath.Trim('/')}/package.json";
            string rawUrl = $"https://raw.githubusercontent.com/{owner}/{repo}/{commitHash}/{packageJsonPath}";

            using UnityWebRequest request = UnityWebRequest.Get(rawUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to fetch package version from '{rawUrl}': {request.error}");
                return null;
            }

            Match match = Regex.Match(request.downloadHandler.text, "\"version\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static int[] ParseVersion(string version)
        {
            string core = version.Split('-')[0];
            string[] parts = core.Split('.');

            return new[]
            {
                parts.Length > 0 && int.TryParse(parts[0], out int major) ? major : 0,
                parts.Length > 1 && int.TryParse(parts[1], out int minor) ? minor : 0,
                parts.Length > 2 && int.TryParse(parts[2], out int patch) ? patch : 0,
            };
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

            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/{gitRef}";
            using UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to resolve commit hash for '{owner}/{repo}@{gitRef}': {request.error}");
                return null;
            }

            Match match = Regex.Match(request.downloadHandler.text, "\"sha\"\\s*:\\s*\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static bool IsCommitHash(string value)
        {
            if (value.Length < 7 || value.Length > 40)
                return false;

            foreach (char c in value)
            {
                bool isHex = (c >= '0' && c <= '9') ||
                             (c >= 'a' && c <= 'f') ||
                             (c >= 'A' && c <= 'F');
                if (!isHex)
                    return false;
            }

            return true;
        }

        private static async Task<string> FetchDefaultBranchAsync(string owner, string repo)
        {
            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}";
            using UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("User-Agent", "DevelopKit-Hub");

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                return null;

            Match match = Regex.Match(request.downloadHandler.text, "\"default_branch\"\\s*:\\s*\"([^\"]+)\"");
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

            if (!Uri.TryCreate(packageUrl, UriKind.Absolute, out Uri uri))
                return false;

            if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                return false;

            string[] parts = uri.AbsolutePath.Trim('/').Split('/');
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
                Match pathMatch = Regex.Match(uri.Query, "[?&]path=([^&]+)");
                if (pathMatch.Success)
                    packagePath = Uri.UnescapeDataString(pathMatch.Groups[1].Value);
            }

            return true;
        }
    }
}
