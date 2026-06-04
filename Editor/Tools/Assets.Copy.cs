
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
    [ToolComment("Copy one or more assets to new project asset paths.", "UnityReplGs.Tools.AssetsCopyTool.Copy(new[] { \"Assets/Source.asset\" }, new[] { \"Assets/Copy.asset\" })")]
    public static class AssetsCopyTool
    {
        public static string Copy(string[] sourcePaths, string[] destinationPaths)
        {
            return JsonOut.Try(() =>
            {
                if (sourcePaths == null || sourcePaths.Length == 0)
                    throw new ArgumentException("sourcePaths is empty.");
                if (destinationPaths == null || sourcePaths.Length != destinationPaths.Length)
                    throw new ArgumentException("sourcePaths and destinationPaths must have the same length.");

                return sourcePaths.Select((source, i) =>
                {
                    source = source.Replace("\\", "/");
                    var destination = ToolUtil.RequireAssetPath(destinationPaths[i]);
                    ToolUtil.EnsureParentFolder(destination);
                    var success = AssetDatabase.CopyAsset(source, destination);
                    return new { sourcePath = source, destinationPath = destination, success = success, error = success ? null : AssetDatabase.GetAssetPath(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(destination)) };
                }).ToArray();
            });
        }
    }
}
