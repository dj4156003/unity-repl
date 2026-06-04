
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
    [ToolComment("Create a prefab asset from a scene GameObject or an existing prefab source.", "UnityReplGs.Tools.AssetsPrefabCreateTool.Create(\"Assets/Generated/Player.prefab\", new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, null, false)")]
    public static class AssetsPrefabCreateTool
    {
        public static string Create(string prefabPath, ObjectRef sceneGameObjectRef = null, string sourcePrefabPath = null, bool connectSceneObject = false)
        {
            return JsonOut.Try(() =>
            {
                prefabPath = ToolUtil.RequireAssetPath(prefabPath);
                if (!prefabPath.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase))
                    throw new System.ArgumentException("Prefab path must end with .prefab.");
                ToolUtil.EnsureParentFolder(prefabPath);

                GameObject saved;
                if (!string.IsNullOrEmpty(sourcePrefabPath))
                {
                    sourcePrefabPath = ToolUtil.RequireAssetPath(sourcePrefabPath);
                    var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
                    if (source == null)
                        throw new System.ArgumentException("Prefab not found: " + sourcePrefabPath);
                    var instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
                    saved = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                    UnityEngine.Object.DestroyImmediate(instance);
                }
                else
                {
                    var go = ToolUtil.ResolveGameObject(sceneGameObjectRef);
                    if (go == null)
                        throw new System.ArgumentException("sceneGameObjectRef is required when sourcePrefabPath is not provided.");
                    saved = connectSceneObject
                        ? PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction)
                        : PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                }
                AssetDatabase.SaveAssets();
                return AssetRef.FromObject(saved);
            });
        }
    }
}
