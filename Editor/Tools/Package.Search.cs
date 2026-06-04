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
    [ToolComment("Search packages through Unity Package Manager.", "UnityReplGs.Tools.PackageSearchTool.Search(\"cinemachine\", true, 10)")]
    public static class PackageSearchTool
    {
        public static IEnumerator Search(string query = null, bool offlineMode = false, int maxResults = 50)
        {
            var request = Client.SearchAll(offlineMode);
            while (!request.IsCompleted)
                yield return null;
            if (request.Status == StatusCode.Failure)
            {
                yield return JsonOut.Error(new Exception(request.Error == null ? "Package search failed." : request.Error.message));
                yield break;
            }
            var result = request.Result
                .Where(p => string.IsNullOrEmpty(query)
                    || p.name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    || (!string.IsNullOrEmpty(p.displayName) && p.displayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (!string.IsNullOrEmpty(p.description) && p.description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0))
                .Take(maxResults)
                .Select(p => new
                {
                    name = p.name,
                    displayName = p.displayName,
                    version = p.version,
                    source = p.source.ToString(),
                    resolvedPath = p.resolvedPath,
                    assetPath = p.assetPath,
                    packageId = p.packageId,
                    isDirectDependency = p.isDirectDependency
                })
                .ToArray();
            yield return JsonOut.Ok(result);
        }
    }
}
