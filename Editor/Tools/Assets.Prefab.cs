
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
    public static class AssetsPrefabTools
    {
        public const string AssetsPrefabCloseToolId = "assets-prefab-close";
        public const string AssetsPrefabCreateToolId = "assets-prefab-create";
        public const string AssetsPrefabInstantiateToolId = "assets-prefab-instantiate";
        public const string AssetsPrefabOpenToolId = "assets-prefab-open";
        public const string AssetsPrefabSaveToolId = "assets-prefab-save";

        public static UnityEditor.SceneManagement.PrefabStage CurrentStage()
        {
            return PrefabStageUtility.GetCurrentPrefabStage();
        }

        public static GameObject LoadPrefab(string prefabPath)
        {
            prefabPath = ToolUtil.RequireAssetPath(prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
                throw new ArgumentException("Prefab not found: " + prefabPath);
            return prefab;
        }
    }
}
