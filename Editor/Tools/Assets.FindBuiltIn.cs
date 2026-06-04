
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
    [ToolComment("Search Unity built-in editor resources and built-in assets by name or type.", "UnityReplGs.Tools.AssetsFindBuiltInTool.FindBuiltIn(\"Default\", \"UnityEngine.Material\", 10)")]
    public static class AssetsFindBuiltInTool
    {
        public static string FindBuiltIn(string nameFilter = null, string typeName = null, int maxResults = 10)
        {
            return JsonOut.Try(() =>
            {
                if (maxResults <= 0)
                    throw new ArgumentException("maxResults must be greater than zero.");
                var type = string.IsNullOrEmpty(typeName) ? typeof(UnityEngine.Object) : ToolUtil.FindType(typeName);
                if (type == null || !typeof(UnityEngine.Object).IsAssignableFrom(type))
                    throw new ArgumentException("Built-in asset type is not a UnityEngine.Object type: " + typeName);

                var builtInPaths = new[] { "Resources/unity_builtin_extra", "Library/unity editor resources" };
                var assets = builtInPaths
                    .SelectMany(path =>
                    {
                        try { return AssetDatabase.LoadAllAssetsAtPath(path); }
                        catch { return new UnityEngine.Object[0]; }
                    })
                    .Where(o => o != null && type.IsAssignableFrom(o.GetType()))
                    .Where(o => string.IsNullOrEmpty(nameFilter) || o.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .GroupBy(o => o.GetInstanceID())
                    .Select(g => g.First())
                    .Take(maxResults)
                    .Select(o => new
                    {
                        name = o.name,
                        type = o.GetType().FullName,
                        instanceId = o.GetInstanceID()
                    })
                    .ToArray();
                return assets;
            });
        }
    }
}
