using System;
using UnityEditor;
using UnityEngine;

namespace PJDev.DevelopKit.Editors
{
    [InitializeOnLoad]
    internal static class DevelopKitHubBootstrap
    {
        private const string AutoOpenedSessionKey = "PJDev.DevelopKit.Hub.AutoOpened";

        private static bool isChecking;

        static DevelopKitHubBootstrap()
        {
            EditorApplication.delayCall += TryOpenHubIfUpdateAvailable;
        }

        private static void TryOpenHubIfUpdateAvailable()
        {
            if (isChecking || SessionState.GetBool(AutoOpenedSessionKey, false))
                return;

            isChecking = true;
            CheckAndOpenAsync();
        }

        private static async void CheckAndOpenAsync()
        {
            try
            {
                if (await DevelopKitHubUpdateService.HasAnyUpdateAvailableAsync())
                {
                    SessionState.SetBool(AutoOpenedSessionKey, true);
                    DevelopKitHubWindow.Open();
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"DevelopKit Hub update check failed: {exception.Message}");
            }
            finally
            {
                isChecking = false;
            }
        }
    }
}
