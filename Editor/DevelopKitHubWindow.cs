using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace PJDev.DevelopKit.Editors
{
    public class DevelopKitHubWindow : EditorWindow
    {
        private static readonly Vector2 WindowSize = new Vector2(500, 500);

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement dimmed;
        private Label dimmedText;

        [MenuItem("PJDev/DevelopKit Hub", priority = -10000)]
        public static void Open()
        {
            var window = GetWindow<DevelopKitHubWindow>();
            window.titleContent = new GUIContent("DevelopKit Hub");
            window.minSize = WindowSize;
            window.maxSize = WindowSize + Vector2.one * 0.1f;
        }

        public async void CreateGUI()
        {
            rootVisualElement.Clear();
            await BuildUIAsync();
        }

        private async Task BuildUIAsync()
        {
            VisualElement root = rootVisualElement;
            root.Clear();

            VisualElement uxml = m_VisualTreeAsset.Instantiate();
            dimmed = uxml.Q("Dimmed");
            dimmedText = uxml.Q<Label>("DimmedText");
            root.Add(uxml);

            SetDimmed(true, "로딩 중...");

            try
            {
                PackageCollection installedPackages = await DevelopKitHubUpdateService.GetInstalledPackagesAsync();

                SetDimmed(true, "버전 확인 중...");

                await SetDevelopKitUpdateSectionAsync(installedPackages, uxml.Q("DevelopKit"));
                await SetPackageSectionAsync(
                    installedPackages,
                    uxml.Q("BasicTemplate"),
                    DevelopKitHubPackages.BasicTemplateName,
                    DevelopKitHubPackages.BasicTemplateUrl,
                    "Basic Template");
                await SetPackageSectionAsync(
                    installedPackages,
                    uxml.Q("Framework"),
                    DevelopKitHubPackages.FrameworkName,
                    DevelopKitHubPackages.FrameworkUrl,
                    "Framework");
            }
            finally
            {
                SetDimmed(false);
            }
        }

        private void SetDimmed(bool isDimmed, string message = null)
        {
            dimmed.visible = isDimmed;

            if (!string.IsNullOrEmpty(message) && dimmedText != null)
                dimmedText.text = message;
        }

        private async Task SetDevelopKitUpdateSectionAsync(PackageCollection installed, VisualElement section)
        {
            section.style.display = DisplayStyle.None;

            PackageInfo packageInfo = installed.FirstOrDefault(p => p.name == DevelopKitHubPackages.DevelopKitName);
            if (packageInfo == null)
                return;

            if (!await DevelopKitHubUpdateService.IsUpdateAvailableAsync(packageInfo, DevelopKitHubPackages.DevelopKitUrl))
                return;

            Button updateBtn = section.Q<Button>("Btn_Update");
            updateBtn.clicked += async () =>
            {
                SetDimmed(true, "업데이트 중...");
                try
                {
                    await UpdatePackageAsync(DevelopKitHubPackages.DevelopKitUrl);
                    await BuildUIAsync();
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.Message);
                    SetDimmed(false);
                }
            };

            section.style.display = DisplayStyle.Flex;
        }

        private async Task SetPackageSectionAsync(
            PackageCollection installed,
            VisualElement section,
            string packageName,
            string packageUrl,
            string displayName)
        {
            VisualElement background = section.Q("Background");
            Button btn = background.Q<Button>("Btn_Install");
            Button updateBtn = background.Q<Button>("Btn_Update");
            updateBtn.style.display = DisplayStyle.None;

            PackageInfo packageInfo = installed.FirstOrDefault(p => p.name == packageName);

            if (packageInfo != null)
            {
                btn.text = $"Remove {displayName}";
                SetSectionRemoveable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true, "제거 중...");
                    try
                    {
                        await RemovePackageAsync(packageName);
                        await BuildUIAsync();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        SetDimmed(false);
                    }
                };

                if (await DevelopKitHubUpdateService.IsUpdateAvailableAsync(packageInfo, packageUrl))
                {
                    updateBtn.text = $"Update {displayName}";
                    updateBtn.style.display = DisplayStyle.Flex;
                    SetSectionUpdatable(section);
                    updateBtn.clicked += async () =>
                    {
                        SetDimmed(true, "업데이트 중...");
                        try
                        {
                            await UpdatePackageAsync(packageUrl);
                            await BuildUIAsync();
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError(exception.Message);
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
                    SetDimmed(true, "설치 중...");
                    try
                    {
                        await InstallPackageAsync(packageUrl);
                        await BuildUIAsync();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        SetDimmed(false);
                    }
                };
            }
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

        private static Task InstallPackageAsync(string url) => AddPackageAsync(url);

        private static Task UpdatePackageAsync(string url) => AddPackageAsync(url);

        private static Task AddPackageAsync(string url)
        {
            var completion = new TaskCompletionSource<bool>();
            AddRequest request = Client.Add(url);

            void Progress()
            {
                if (!request.IsCompleted)
                    return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    completion.TrySetResult(true);
                else
                    completion.TrySetException(new Exception(request.Error.message));
            }

            EditorApplication.update += Progress;
            return completion.Task;
        }

        private static Task RemovePackageAsync(string name)
        {
            var completion = new TaskCompletionSource<bool>();
            RemoveRequest request = Client.Remove(name);

            void Progress()
            {
                if (!request.IsCompleted)
                    return;

                EditorApplication.update -= Progress;

                if (request.Status == StatusCode.Success)
                    completion.TrySetResult(true);
                else
                    completion.TrySetException(new Exception(request.Error.message));
            }

            EditorApplication.update += Progress;
            return completion.Task;
        }
    }
}
