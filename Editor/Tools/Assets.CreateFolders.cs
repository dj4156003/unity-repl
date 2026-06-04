
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
    [ToolComment(
        "Create project folders under Assets and return their asset references.",
        "UnityReplGs.Tools.AssetsCreateFoldersTool.CreateFolder(\"Assets\", \"Generated\")",
        "UnityReplGs.Tools.AssetsCreateFoldersTool.CreateFolders(new[] { \"Assets/Generated\", \"Assets/Generated/Sub\" })"
    )]
    public static class AssetsCreateFoldersTool
    {
        public static string CreateFolders(string[] folderPaths)
        {
            return JsonOut.Try(() =>
            {
                if (folderPaths == null || folderPaths.Length == 0)
                    throw new ArgumentException("folderPaths is empty.");
                return folderPaths.Select(CreateOne).ToArray();
            });
        }

        public static string CreateFolder(string parentFolder, string folderName)
        {
            return JsonOut.Try(() =>
            {
                if (string.IsNullOrEmpty(parentFolder))
                    parentFolder = "Assets";
                parentFolder = parentFolder.Replace("\\", "/").TrimEnd('/');
                if (!AssetDatabase.IsValidFolder(parentFolder))
                    ToolUtil.EnsureParentFolder(parentFolder + "/placeholder");
                var guid = AssetDatabase.CreateFolder(parentFolder, folderName);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return new { path = path, guid = guid, existed = string.IsNullOrEmpty(guid) };
            });
        }

        static object CreateOne(string folderPath)
        {
            folderPath = ToolUtil.RequireAssetPath(folderPath).TrimEnd('/');
            if (AssetDatabase.IsValidFolder(folderPath))
                return new { path = folderPath, guid = AssetDatabase.AssetPathToGUID(folderPath), existed = true };
            ToolUtil.EnsureParentFolder(folderPath + "/placeholder");
            return new { path = folderPath, guid = AssetDatabase.AssetPathToGUID(folderPath), existed = false };
        }
    }
}
