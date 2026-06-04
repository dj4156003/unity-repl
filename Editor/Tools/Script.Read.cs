
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
    [ToolComment("Read a text file from the project by line range.", "UnityReplGs.Tools.ScriptReadTool.Read(\"Assets/Scripts/Player.cs\", 1, 120)")]
    public static class ScriptReadTool
    {
        public static string Read(string path, int startLine = 1, int endLine = -1)
        {
            return JsonOut.Try(() =>
            {
                path = ToolUtil.ProjectRelativePath(path);
                if (!File.Exists(path))
                    throw new FileNotFoundException(path);
                var lines = File.ReadAllLines(path);
                var start = System.Math.Max(1, startLine);
                var end = endLine < 0 ? lines.Length : System.Math.Min(endLine, lines.Length);
                var content = string.Join("\n", lines.Skip(start - 1).Take(System.Math.Max(0, end - start + 1)).ToArray());
                return new { path = path, startLine = start, endLine = end, content = content };
            });
        }
    }
}
