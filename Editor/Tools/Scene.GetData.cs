
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
    [ToolComment("Read data for an open scene, including roots or hierarchy when requested.", "UnityReplGs.Tools.SceneGetDataTool.GetData(null, true, true, 1)")]
    public static class SceneGetDataTool
    {
        public static string GetData(string scenePathOrName = null, bool includeRoots = true, bool includeHierarchy = false, int hierarchyDepth = 0)
        {
            return JsonOut.Try(() =>
            {
                var scene = string.IsNullOrEmpty(scenePathOrName)
                    ? UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                    : ToolUtil.FindScene(scenePathOrName);
                if (!scene.IsValid())
                    throw new System.ArgumentException("Scene not found: " + scenePathOrName);
                return new
                {
                    scene = ToolUtil.ToSceneData(scene),
                    roots = includeRoots && scene.isLoaded
                        ? scene.GetRootGameObjects()
                            .Select(go => includeHierarchy ? ToolUtil.ToGameObjectSummary(go, false, false, false, true, hierarchyDepth) : ToolUtil.ToGameObjectData(go))
                            .ToArray()
                        : null
                };
            });
        }
    }
}
