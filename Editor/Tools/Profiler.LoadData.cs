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
    [ToolComment("Load profiler data from a file path.", "UnityReplGs.Tools.ProfilerLoadDataTool.LoadData(\"Temp/profile.data\")")]
    public static class ProfilerLoadDataTool
    {
        public static string LoadData(string path)
        {
            return JsonOut.Try(() =>
            {
                var driver = System.Type.GetType("UnityEditorInternal.ProfilerDriver,UnityEditor");
                var method = driver == null ? null : driver.GetMethod("LoadProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string), typeof(bool) }, null);
                if (method != null)
                    method.Invoke(null, new object[] { path, true });
                else
                {
                    method = driver == null ? null : driver.GetMethod("LoadProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
                    if (method == null)
                        throw new System.MissingMethodException("UnityEditorInternal.ProfilerDriver.LoadProfile");
                    method.Invoke(null, new object[] { path });
                }
                return new { path = path };
            });
        }
    }
}
