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
    [ToolComment("Add a Unity package through Unity Package Manager.", "UnityReplGs.Tools.PackageAddTool.Add(\"com.unity.cinemachine\")")]
    public static class PackageAddTool
    {
        public static IEnumerator Add(string packageIdOrName)
        {
            if (string.IsNullOrEmpty(packageIdOrName))
            {
                yield return JsonOut.Error(new System.ArgumentException("packageIdOrName is empty."));
                yield break;
            }
            var request = Client.Add(packageIdOrName);
            while (!request.IsCompleted)
                yield return null;
            if (request.Status == StatusCode.Failure)
            {
                yield return JsonOut.Error(new System.Exception(request.Error == null ? "Package add failed." : request.Error.message));
                yield break;
            }
            var info = request.Result;
            yield return JsonOut.Ok(new
            {
                name = info.name,
                displayName = info.displayName,
                version = info.version,
                source = info.source.ToString(),
                resolvedPath = info.resolvedPath,
                assetPath = info.assetPath,
                packageId = info.packageId,
                isDirectDependency = info.isDirectDependency
            });
        }
    }
}
