
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
    [ToolComment("Modify scene GameObject name, active state, tag, layer, transform, or serialized data.", "UnityReplGs.Tools.GameObjectModifyTool.Modify(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"Hero\", true, null, null, null, null, null, false, null)")]
    public static class GameObjectModifyTool
    {
        public static string Modify(ObjectRef gameObjectRef, string name = null, bool? activeSelf = null, string tag = null, int? layer = null, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool isLocalSpace = false, string serializedPatchJson = null)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                Undo.RecordObject(go, "Modify GameObject");
                if (name != null) go.name = name;
                if (activeSelf.HasValue) go.SetActive(activeSelf.Value);
                if (tag != null) go.tag = tag;
                if (layer.HasValue) go.layer = layer.Value;
                ToolUtil.SetTransform(go, position, rotation, scale, !isLocalSpace);
                ToolUtil.ApplySerializedPatch(go, serializedPatchJson);
                EditorUtility.SetDirty(go);
                return ToolUtil.ToGameObjectData(go);
            });
        }
    }
}
