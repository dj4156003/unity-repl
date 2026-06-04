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
    [ToolComment("Disable the Unity profiler.", "UnityReplGs.Tools.ProfilerStopTool.Stop()")]
    public static class ProfilerStopTool
    {
        public static string Stop()
        {
            return JsonOut.Try(() =>
            {
                Profiler.enabled = false;
                return new { enabled = Profiler.enabled };
            });
        }
    }
}
