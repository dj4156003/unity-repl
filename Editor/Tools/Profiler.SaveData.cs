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
    [ToolComment("Save current profiler data to a file path.", "UnityReplGs.Tools.ProfilerSaveDataTool.SaveData(\"Temp/profile.data\")")]
    public static class ProfilerSaveDataTool
    {
        public static string SaveData(string path)
        {
            return JsonOut.Try(() =>
            {
                var driver = System.Type.GetType("UnityEditorInternal.ProfilerDriver,UnityEditor");
                var method = driver == null ? null : driver.GetMethod("SaveProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
                if (method == null)
                    throw new System.MissingMethodException("UnityEditorInternal.ProfilerDriver.SaveProfile");
                method.Invoke(null, new object[] { path });
                return new { path = path };
            });
        }
    }
}
