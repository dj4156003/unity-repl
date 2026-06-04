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
    [ToolComment("Read Unity runtime memory counters from the Profiler API.", "UnityReplGs.Tools.ProfilerGetMemoryStatsTool.GetMemoryStats()")]
    public static class ProfilerGetMemoryStatsTool
    {
        public static string GetMemoryStats()
        {
            return JsonOut.Try(() => new
            {
                totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong(),
                totalReservedMemory = Profiler.GetTotalReservedMemoryLong(),
                totalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong(),
                monoHeapSize = Profiler.GetMonoHeapSizeLong(),
                monoUsedSize = Profiler.GetMonoUsedSizeLong(),
                tempAllocatorSize = Profiler.GetTempAllocatorSize()
            });
        }
    }
}
