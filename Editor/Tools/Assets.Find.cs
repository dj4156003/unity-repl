
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools
{
    [ToolComment("Find project assets with AssetDatabase filters and return asset references.", "UnityReplGs.Tools.AssetsFindTool.Find(\"t:Scene\", null, 5)")]
    public static class AssetsFindTool
    {
        public static string Find(string filter = "", string[] searchInFolders = null, int maxResults = 10)
        {
            return JsonOut.Try(() =>
            {
                if (maxResults <= 0)
                    throw new ArgumentException("maxResults must be greater than zero.");
                var guids = searchInFolders == null || searchInFolders.Length == 0
                    ? AssetDatabase.FindAssets(filter ?? string.Empty)
                    : AssetDatabase.FindAssets(filter ?? string.Empty, searchInFolders);
                return guids.Take(maxResults)
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(path => !string.IsNullOrEmpty(path))
                    .Select(AssetRef.FromPath)
                    .ToArray();
            });
        }
    }
}
