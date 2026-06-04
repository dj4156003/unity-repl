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
    [ToolComment("Remove an installed Unity package by package name.", "UnityReplGs.Tools.PackageRemoveTool.Remove(\"com.unity.cinemachine\")")]
    public static class PackageRemoveTool
    {
        public static IEnumerator Remove(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                yield return JsonOut.Error(new System.ArgumentException("packageName is empty."));
                yield break;
            }
            var request = Client.Remove(packageName);
            while (!request.IsCompleted)
                yield return null;
            if (request.Status == StatusCode.Failure)
            {
                yield return JsonOut.Error(new System.Exception(request.Error == null ? "Package remove failed." : request.Error.message));
                yield break;
            }
            yield return JsonOut.Ok(new { removed = packageName, packageIdOrName = request.PackageIdOrName });
        }
    }
}
