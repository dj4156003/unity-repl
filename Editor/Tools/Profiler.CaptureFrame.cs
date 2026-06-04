using System.Collections;
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
    [ToolComment("Enable profiling and capture one or more editor frames.", "UnityReplGs.Tools.ProfilerCaptureFrameTool.CaptureFrame(3)")]
    public static class ProfilerCaptureFrameTool
    {
        public static IEnumerator CaptureFrame(int frames = 1)
        {
            if (frames <= 0)
                frames = 1;
            var wasEnabled = Profiler.enabled;
            Profiler.enabled = true;
            for (var i = 0; i < frames; i++)
                yield return null;
            yield return JsonOut.Ok(new { capturedFrames = frames, enabledBefore = wasEnabled, enabledAfter = Profiler.enabled });
        }
    }
}
