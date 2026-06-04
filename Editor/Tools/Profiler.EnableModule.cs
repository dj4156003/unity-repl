using UnityEngine.Profiling;

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
    [ToolComment("Enable or disable a Unity ProfilerArea by name.", "UnityReplGs.Tools.ProfilerEnableModuleTool.EnableModule(\"CPU\", true)")]
    public static class ProfilerEnableModuleTool
    {
        public static string EnableModule(string moduleName, bool enabled)
        {
            return JsonOut.Try(() =>
            {
                var area = (ProfilerArea)System.Enum.Parse(typeof(ProfilerArea), moduleName, true);
                Profiler.SetAreaEnabled(area, enabled);
                return new { name = area.ToString(), enabled = Profiler.GetAreaEnabled(area) };
            });
        }
    }
}
