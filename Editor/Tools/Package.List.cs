using System.Collections;
using UnityEditor.PackageManager;

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
    [ToolComment("List installed Unity packages through Unity Package Manager.", "UnityReplGs.Tools.PackageListTool.List(true, false)")]
    public static class PackageListTool
    {
        public static IEnumerator List(bool offlineMode = false, bool includeIndirectDependencies = true)
        {
            var request = Client.List(offlineMode, includeIndirectDependencies);
            while (!request.IsCompleted)
                yield return null;
            if (request.Status == StatusCode.Failure)
            {
                yield return JsonOut.Error(new System.Exception(request.Error == null ? "Package list failed." : request.Error.message));
                yield break;
            }
            yield return JsonOut.Ok(request.Result.Select(PackageInfoData).ToArray());
        }

        internal static object PackageInfoData(UnityEditor.PackageManager.PackageInfo info)
        {
            return new
            {
                name = info.name,
                displayName = info.displayName,
                version = info.version,
                source = info.source.ToString(),
                resolvedPath = info.resolvedPath,
                assetPath = info.assetPath,
                packageId = info.packageId,
                isDirectDependency = info.isDirectDependency
            };
        }
    }
}
