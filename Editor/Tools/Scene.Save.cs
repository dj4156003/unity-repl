
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
    [ToolComment("Save an open scene, optionally saving as a new asset path.", "UnityReplGs.Tools.SceneSaveTool.Save(null, null)")]
    public static class SceneSaveTool
    {
        public static string Save(string scenePathOrName = null, string saveAsPath = null)
        {
            return JsonOut.Try(() =>
            {
                var scene = string.IsNullOrEmpty(scenePathOrName)
                    ? UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                    : ToolUtil.FindScene(scenePathOrName);
                if (!scene.IsValid())
                    throw new System.ArgumentException("Scene not found: " + scenePathOrName);
                var success = string.IsNullOrEmpty(saveAsPath)
                    ? EditorSceneManager.SaveScene(scene)
                    : EditorSceneManager.SaveScene(scene, ToolUtil.RequireAssetPath(saveAsPath));
                return new { success = success, scene = ToolUtil.ToSceneData(scene) };
            });
        }
    }
}
