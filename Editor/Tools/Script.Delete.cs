
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
    [ToolComment("Delete project C# script files and optionally refresh the AssetDatabase.", "UnityReplGs.Tools.ScriptDeleteTool.Delete(new[] { \"Assets/Generated/Test.cs\" }, true)")]
    public static class ScriptDeleteTool
    {
        public static string Delete(string[] paths, bool refresh = true)
        {
            return JsonOut.Try(() =>
            {
                if (paths == null || paths.Length == 0)
                    throw new System.ArgumentException("paths is empty.");
                var result = paths.Select(path =>
                {
                    path = ToolUtil.ProjectRelativePath(path);
                    if (!path.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase))
                        throw new System.ArgumentException("Script path must end with .cs: " + path);
                    var existed = File.Exists(path);
                    if (existed)
                        File.Delete(path);
                    return new { path = path, existed = existed, deleted = existed && !File.Exists(path) };
                }).ToArray();
                if (refresh)
                    AssetDatabase.Refresh();
                return result;
            });
        }
    }
}
