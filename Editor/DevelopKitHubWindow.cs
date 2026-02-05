using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace Skddkkkk.DevelopKit.Editors
{
    public class DevelopKitHubWindow : EditorWindow
    {
        private const string basicTemplatePackageUrl =
            "https://github.com/Parkjung2016/DevelopKit_BasicTemplate.git";

        private const string basicTemplatePackageName =
            "com.skddkkkk.developkit.basictemplate";

        private const string frameworkPackageUrl =
            "https://github.com/Parkjung2016/DevelopKit_Framework.git";

        private const string frameworkPackageName =
            "com.skddkkkk.developkit.framework";

        private static readonly Vector2 windowSize = new Vector2(500, 500);

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private VisualElement dimmed;

        [MenuItem("Skddkkkk/DevelopKit Hub")]
        public static void ShowExample()
        {
            var wnd = GetWindow<DevelopKitHubWindow>();
            wnd.titleContent = new GUIContent("DevelopKit Hub");
            wnd.minSize = windowSize;
            wnd.maxSize = windowSize + Vector2.one * 0.1f;
        }

        public async void CreateGUI()
        {
            var installedPackages = await GetInstalledPackages();

            var root = rootVisualElement;
            var uxml = m_VisualTreeAsset.Instantiate();
            dimmed = uxml.Q("Dimmed");
            SetBasicTemplateInstallButton(installedPackages, uxml.Q("BasicTemplate"));
            SetFrameworkInstallButton(installedPackages, uxml.Q("Framework"));
            SetDimmed(false);
            root.Add(uxml);
        }

        private void SetDimmed(bool isDimmed)
        {
            dimmed.visible = isDimmed;
        }
        
        private void SetBasicTemplateInstallButton(
            PackageCollection installed,
            VisualElement section)
        {
            var btn = section.Q("Background").Q<Button>("Btn_Install");

            if (installed.Any(p => p.name == basicTemplatePackageName))
            {
                btn.text = "Remove Basic Template";
                SetSectionRemoveable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true);
                    await RemovePackageAsync(basicTemplatePackageName);
                    SetDimmed(false);
                };
            }
            else
            {
                btn.text = "Install Basic Template";
                SetSectionInstallable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(false);
                    await InstallPackageAsync(basicTemplatePackageUrl);
                    SetDimmed(true);
                };
            }
        }

        private void SetFrameworkInstallButton(
            PackageCollection installed,
            VisualElement section)
        {
            var btn = section.Q("Background").Q<Button>("Btn_Install");

            if (installed.Any(p => p.name == frameworkPackageName))
            {
                btn.text = "Remove Framework";
                SetSectionRemoveable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true);
                    await RemovePackageAsync(frameworkPackageName);
                    SetDimmed(false);
                };
            }
            else
            {
                btn.text = "Install Framework";
                SetSectionInstallable(section);
                btn.clicked += async () =>
                {
                    SetDimmed(true);
                    await InstallPackageAsync(frameworkPackageUrl);
                    SetDimmed(false);
                };
            }
        }

        private void SetSectionInstallable(VisualElement section)
        {
            section.RemoveFromClassList("removeable");
            section.AddToClassList("installable");
        }

        private void SetSectionRemoveable(VisualElement section)
        {
            section.RemoveFromClassList("installable");
            section.AddToClassList("removeable");
        }

        private Task InstallPackageAsync(string url)
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
                    tcs.SetException(new System.Exception(request.Error.message));
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
                    tcs.SetException(new System.Exception(request.Error.message));
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
                    tcs.SetException(new System.Exception(request.Error.message));
            }

            return await tcs.Task;
        }
    }
}