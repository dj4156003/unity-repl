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
    [ToolComment("Set an open scene as the active scene.", "UnityReplGs.Tools.SceneSetActiveTool.SetActive(\"Main\")")]
    public static class SceneSetActiveTool
    {
        public static string SetActive(string scenePathOrName)
        {
            return JsonOut.Try(() =>
            {
                var scene = ToolUtil.FindScene(scenePathOrName);
                if (!scene.IsValid())
                    throw new System.ArgumentException("Scene not found: " + scenePathOrName);
                var success = UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
                return new { success = success, scene = ToolUtil.ToSceneData(UnityEngine.SceneManagement.SceneManager.GetActiveScene()) };
            });
        }
    }
}
