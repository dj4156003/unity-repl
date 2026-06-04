
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
    [ToolComment("Close an open scene from the editor.", "UnityReplGs.Tools.SceneUnloadTool.Unload(\"Main\", true)")]
    public static class SceneUnloadTool
    {
        public static string Unload(string scenePathOrName, bool removeScene = true)
        {
            return JsonOut.Try(() =>
            {
                var scene = ToolUtil.FindScene(scenePathOrName);
                if (!scene.IsValid())
                    throw new System.ArgumentException("Scene not found: " + scenePathOrName);
                var data = ToolUtil.ToSceneData(scene);
                var success = EditorSceneManager.CloseScene(scene, removeScene);
                return new { success = success, scene = data };
            });
        }
    }
}
