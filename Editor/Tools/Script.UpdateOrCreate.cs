
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
    [ToolComment("Create or overwrite a project C# file and optionally refresh the AssetDatabase.", "UnityReplGs.Tools.ScriptUpdateOrCreateTool.UpdateOrCreate(\"Assets/Generated/Test.cs\", \"public class Test {}\", true)")]
    public static class ScriptUpdateOrCreateTool
    {
        public static string UpdateOrCreate(string path, string csharpCode, bool refresh = true)
        {
            return JsonOut.Try(() =>
            {
                path = ToolUtil.ProjectRelativePath(path);
                if (!path.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase))
                    throw new System.ArgumentException("Script path must end with .cs.");
                if (!path.StartsWith("Assets/"))
                    throw new System.ArgumentException("Script writes are restricted to Assets/. Path: " + path);
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);
                File.WriteAllText(path, csharpCode ?? string.Empty);
                if (refresh)
                    AssetDatabase.ImportAsset(path);
                return new { path = path, bytes = File.ReadAllBytes(path).Length, refreshed = refresh };
            });
        }
    }
}
