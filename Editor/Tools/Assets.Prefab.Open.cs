
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
    [ToolComment("Open a prefab asset or prefab instance in Prefab Mode.", "UnityReplGs.Tools.AssetsPrefabOpenTool.Open(\"Assets/Prefabs/Player.prefab\", null)")]
    public static class AssetsPrefabOpenTool
    {
        public static string Open(string prefabPath = null, ObjectRef gameObjectRef = null)
        {
            return JsonOut.Try(() =>
            {
                GameObject go = null;
                if (gameObjectRef != null)
                    go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go != null && string.IsNullOrEmpty(prefabPath))
                    prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
                if (string.IsNullOrEmpty(prefabPath))
                    throw new System.ArgumentException("Provide prefabPath or gameObjectRef.");
                var stage = go == null ? PrefabStageUtility.OpenPrefab(prefabPath) : PrefabStageUtility.OpenPrefab(prefabPath, go);
                if (stage == null)
                    throw new System.InvalidOperationException("Failed to open prefab stage: " + prefabPath);
                return new { assetPath = stage.assetPath, root = ToolUtil.ToGameObjectData(stage.prefabContentsRoot) };
            });
        }
    }
}
