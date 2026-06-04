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
    public static class ProfilerTools
    {
        public const string ProfilerCaptureFrameToolId = "profiler-capture-frame";
        public const string ProfilerClearDataToolId = "profiler-clear-data";
        public const string ProfilerEnableModuleToolId = "profiler-enable-module";
        public const string ProfilerGetMemoryStatsToolId = "profiler-get-memory-stats";
        public const string ProfilerGetRenderingStatsToolId = "profiler-get-rendering-stats";
        public const string ProfilerGetScriptStatsToolId = "profiler-get-script-stats";
        public const string ProfilerGetStatusToolId = "profiler-get-status";
        public const string ProfilerListModulesToolId = "profiler-list-modules";
        public const string ProfilerLoadDataToolId = "profiler-load-data";
        public const string ProfilerSaveDataToolId = "profiler-save-data";
        public const string ProfilerStartToolId = "profiler-start";
        public const string ProfilerStopToolId = "profiler-stop";
    }
}
