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
    [ToolComment("Read Unity editor rendering statistics.", "UnityReplGs.Tools.ProfilerGetRenderingStatsTool.GetRenderingStats()")]
    public static class ProfilerGetRenderingStatsTool
    {
        public static string GetRenderingStats()
        {
            return JsonOut.Try(() =>
            {
                var type = System.Type.GetType("UnityEditor.UnityStats,UnityEditor");
                if (type == null)
                    throw new System.InvalidOperationException("UnityEditor.UnityStats was not found.");
                string[] names = { "batches", "drawCalls", "setPassCalls", "triangles", "vertices", "shadowCasters", "renderTextureChanges" };
                return names.ToDictionary(name => name, name =>
                {
                    var prop = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    return prop == null ? null : prop.GetValue(null, null);
                });
            });
        }
    }
}
