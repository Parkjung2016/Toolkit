using PJH.Utility.CustomDebug;

namespace PJH.Utility.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using System.Collections.Generic;

    public static class AddressableUtil
    {
        [MenuItem("Tools/Addressable/Rename Sprites")]
        public static void RenameSprites()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                PJHDebug.LogColorPart("Addressable Asset Settings not found.", Color.red);
                return;
            }

            List<AddressableAssetEntry> spriteEntries = new List<AddressableAssetEntry>();
            settings.GetAllAssets(spriteEntries, includeSubObjects: false, groupFilter: null,
                entryFilter: entry => entry.MainAssetType == typeof(Texture2D));

            foreach (var entry in spriteEntries)
            {
                string oldAddress = entry.address;
                if (oldAddress.Contains(".sprite")) continue;
                string newAddress = oldAddress + ".sprite";

                if (entry.address != newAddress)
                {
                    Debug.Log($"[{oldAddress}] -> [{newAddress}]");
                    entry.address = newAddress;
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
                }
            }

            AssetDatabase.SaveAssets();
            PJHDebug.LogColorPart("Addressable Sprites renamed successfully", Color.green);
        }
    }
}