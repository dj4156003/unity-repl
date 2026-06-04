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
    [ToolComment("Clear currently loaded profiler data.", "UnityReplGs.Tools.ProfilerClearDataTool.ClearData()")]
    public static class ProfilerClearDataTool
    {
        public static string ClearData()
        {
            return JsonOut.Try(() =>
            {
                var driver = System.Type.GetType("UnityEditorInternal.ProfilerDriver,UnityEditor");
                var method = driver == null ? null : driver.GetMethod("ClearAllFrames", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, null);
                return new { cleared = method != null };
            });
        }
    }
}
