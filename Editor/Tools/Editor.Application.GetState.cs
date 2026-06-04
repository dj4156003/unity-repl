
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
    [ToolComment("Inspect Unity Editor state such as play mode, compile state, current scene, and selection.", "UnityReplGs.Tools.EditorApplicationGetStateTool.GetState()")]
    public static class EditorApplicationGetStateTool
    {
        public static string GetState()
        {
            return JsonOut.Try(() => new
            {
                unityVersion = Application.unityVersion,
                isPlaying = EditorApplication.isPlaying,
                isPaused = EditorApplication.isPaused,
                isCompiling = EditorApplication.isCompiling,
                isUpdating = EditorApplication.isUpdating,
                timeSinceStartup = EditorApplication.timeSinceStartup,
                currentScene = ToolUtil.ToSceneData(UnityEngine.SceneManagement.SceneManager.GetActiveScene()),
                selected = Selection.objects == null ? 0 : Selection.objects.Length
            });
        }
    }
}
