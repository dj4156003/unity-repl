using System.Reflection;

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
    [ToolComment("Clear all entries from the Unity Console.", "UnityReplGs.Tools.ConsoleClearLogsTool.ClearLogs()")]
    public static class ConsoleClearLogsTool
    {
        public static string ClearLogs()
        {
            return JsonOut.Try(() =>
            {
                var type = System.Type.GetType("UnityEditor.LogEntries,UnityEditor");
                var method = type == null ? null : type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method == null)
                    throw new System.MissingMethodException("UnityEditor.LogEntries.Clear");
                method.Invoke(null, null);
                return new { cleared = true };
            });
        }
    }
}
