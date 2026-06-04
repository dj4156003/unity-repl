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
    [ToolComment("Read profiler enabled state and related status flags.", "UnityReplGs.Tools.ProfilerGetStatusTool.GetStatus()")]
    public static class ProfilerGetStatusTool
    {
        public static string GetStatus()
        {
            return JsonOut.Try(() => new
            {
                enabled = Profiler.enabled,
                supported = Profiler.supported,
                enableBinaryLog = Profiler.enableBinaryLog,
                logFile = Profiler.logFile,
                areas = System.Enum.GetValues(typeof(ProfilerArea)).Cast<ProfilerArea>()
                    .Select(a => new { name = a.ToString(), enabled = Profiler.GetAreaEnabled(a) })
                    .ToArray()
            });
        }
    }
}
