
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
    public static class AssetsTools
    {
        public const string AssetsCopyToolId = "assets-copy";
        public const string AssetsCreateFolderToolId = "assets-create-folder";
        public const string AssetsDeleteToolId = "assets-delete";
        public const string AssetsFindToolId = "assets-find";
        public const string AssetsFindBuiltInToolId = "assets-find-built-in";
        public const string AssetsGetDataToolId = "assets-get-data";
        public const string AssetsModifyToolId = "assets-modify";
        public const string AssetsMoveToolId = "assets-move";
        public const string AssetsRefreshToolId = "assets-refresh";

        public static void RequireWritableAssetPath(string assetPath)
        {
            ToolUtil.RequireAssetPath(assetPath);
            if (assetPath.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Package assets are read-only through this tool: " + assetPath);
        }

        public static UnityEngine.Object Load(string assetPath, string guid)
        {
            var path = ToolUtil.ResolveAssetPath(assetPath, guid);
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (type == null)
                throw new ArgumentException("Asset not found: " + path);
            var obj = AssetDatabase.LoadAssetAtPath(path, type);
            if (obj == null)
                throw new ArgumentException("Asset could not be loaded: " + path);
            return obj;
        }
    }
}
