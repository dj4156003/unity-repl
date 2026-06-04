
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
    [ToolComment("List shader assets in the project, optionally filtered by name.", "UnityReplGs.Tools.AssetsShaderListAllTool.ListAll(\"Lit\", 20)")]
    public static class AssetsShaderListAllTool
    {
        public static string ListAll(string nameFilter = null, int maxResults = 200)
        {
            return JsonOut.Try(() =>
            {
                var projectShaders = AssetDatabase.FindAssets("t:Shader")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(path => AssetDatabase.LoadAssetAtPath<Shader>(path))
                    .Where(s => s != null);
                var loadedShaders = Resources.FindObjectsOfTypeAll<Shader>().Where(s => s != null);
                return projectShaders.Concat(loadedShaders)
                    .Where(s => string.IsNullOrEmpty(nameFilter) || s.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .GroupBy(s => s.name)
                    .Select(g => g.First())
                    .OrderBy(s => s.name)
                    .Take(maxResults)
                    .Select(s => new { name = s.name, path = AssetDatabase.GetAssetPath(s), instanceId = s.GetInstanceID() })
                    .ToArray();
            });
        }
    }
}
