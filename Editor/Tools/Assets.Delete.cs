
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
    [ToolComment("Delete project assets, optionally moving them to the operating system trash.", "UnityReplGs.Tools.AssetsDeleteTool.Delete(new[] { \"Assets/Temp.asset\" }, true)")]
    public static class AssetsDeleteTool
    {
        public static string Delete(string[] assetPaths, bool moveToTrash = false)
        {
            return JsonOut.Try(() =>
            {
                if (assetPaths == null || assetPaths.Length == 0)
                    throw new ArgumentException("assetPaths is empty.");
                return assetPaths.Select(path =>
                {
                    path = ToolUtil.RequireAssetPath(path);
                    var success = moveToTrash ? AssetDatabase.MoveAssetToTrash(path) : AssetDatabase.DeleteAsset(path);
                    return new { path = path, success = success };
                }).ToArray();
            });
        }
    }
}
