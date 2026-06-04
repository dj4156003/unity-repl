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
    [ToolComment("Read scripting-related profiler statistics.", "UnityReplGs.Tools.ProfilerGetScriptStatsTool.GetScriptStats()")]
    public static class ProfilerGetScriptStatsTool
    {
        public static string GetScriptStats()
        {
            return JsonOut.Try(() => new
            {
                monoHeapSize = Profiler.GetMonoHeapSizeLong(),
                monoUsedSize = Profiler.GetMonoUsedSizeLong(),
                allocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver()
            });
        }
    }
}
