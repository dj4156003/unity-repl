
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
    [ToolComment("Create a new scene, optionally saving it to an asset path.", "UnityReplGs.Tools.SceneCreateTool.Create(\"Assets/Scenes/Generated.unity\", false)")]
    public static class SceneCreateTool
    {
        public static string Create(string scenePath = null, bool additive = false)
        {
            return JsonOut.Try(() =>
            {
                var mode = additive ? NewSceneMode.Additive : NewSceneMode.Single;
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, mode);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    scenePath = ToolUtil.RequireAssetPath(scenePath);
                    ToolUtil.EnsureParentFolder(scenePath);
                    EditorSceneManager.SaveScene(scene, scenePath);
                    AssetDatabase.Refresh();
                }
                return ToolUtil.ToSceneData(scene);
            });
        }
    }
}
