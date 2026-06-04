
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
    [ToolComment("Open a scene asset in the editor.", "UnityReplGs.Tools.SceneOpenTool.Open(\"Assets/Scenes/Main.unity\", UnityEditor.SceneManagement.OpenSceneMode.Single)")]
    public static class SceneOpenTool
    {
        public static string Open(string scenePath, OpenSceneMode mode = OpenSceneMode.Single)
        {
            return JsonOut.Try(() =>
            {
                scenePath = ToolUtil.RequireAssetPath(scenePath);
                var scene = EditorSceneManager.OpenScene(scenePath, mode);
                return ToolUtil.ToSceneData(scene);
            });
        }
    }
}
