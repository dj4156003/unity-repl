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
    [ToolComment("Enable the Unity profiler, optionally requesting deep profiling.", "UnityReplGs.Tools.ProfilerStartTool.Start(false)")]
    public static class ProfilerStartTool
    {
        public static string Start(bool deepProfiling = false)
        {
            return JsonOut.Try(() =>
            {
                Profiler.enableBinaryLog = false;
                Profiler.enabled = true;
                Profiler.SetAreaEnabled(ProfilerArea.CPU, true);
                return new { enabled = Profiler.enabled, deepProfilingRequested = deepProfiling };
            });
        }
    }
}
