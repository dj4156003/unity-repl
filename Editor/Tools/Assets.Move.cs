
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
    [ToolComment("Move or rename one or more project assets.", "UnityReplGs.Tools.AssetsMoveTool.Move(new[] { \"Assets/Old.asset\" }, new[] { \"Assets/New.asset\" })")]
    public static class AssetsMoveTool
    {
        public static string Move(string[] sourcePaths, string[] destinationPaths)
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
                    var error = AssetDatabase.MoveAsset(source, destination);
                    return new { sourcePath = source, destinationPath = destination, success = string.IsNullOrEmpty(error), error = error };
                }).ToArray();
            });
        }
    }
}
