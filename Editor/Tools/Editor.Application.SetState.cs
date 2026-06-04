
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
    [ToolComment("Change Unity Editor play, pause, or step state.", "UnityReplGs.Tools.EditorApplicationSetStateTool.SetState(true, null, false)")]
    public static class EditorApplicationSetStateTool
    {
        public static string SetState(bool? isPlaying = null, bool? isPaused = null, bool step = false)
        {
            return JsonOut.Try(() =>
            {
                if (isPlaying.HasValue)
                    EditorApplication.isPlaying = isPlaying.Value;
                if (isPaused.HasValue)
                    EditorApplication.isPaused = isPaused.Value;
                if (step)
                    EditorApplication.Step();
                return new
                {
                    isPlaying = EditorApplication.isPlaying,
                    isPaused = EditorApplication.isPaused,
                    isCompiling = EditorApplication.isCompiling
                };
            });
        }
    }
}
