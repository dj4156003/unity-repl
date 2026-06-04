
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
    [ToolComment("Instantiate a prefab into the open scene, optionally under a parent and transform.", "UnityReplGs.Tools.AssetsPrefabInstantiateTool.Instantiate(\"Assets/Prefabs/Player.prefab\")")]
    public static class AssetsPrefabInstantiateTool
    {
        public static string Instantiate(string prefabPath, ObjectRef parentGameObjectRef = null, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool isLocalSpace = false)
        {
            return JsonOut.Try(() =>
            {
                var prefab = AssetsPrefabTools.LoadPrefab(prefabPath);
                var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (go == null)
                    throw new System.InvalidOperationException("Prefab instantiate returned null.");
                var parent = ToolUtil.ResolveGameObject(parentGameObjectRef);
                if (parent != null)
                    go.transform.SetParent(parent.transform, false);
                ToolUtil.SetTransform(go, position ?? Vector3.zero, rotation ?? Vector3.zero, scale ?? Vector3.one, !isLocalSpace);
                EditorUtility.SetDirty(go);
                return ToolUtil.ToGameObjectData(go);
            });
        }
    }
}
