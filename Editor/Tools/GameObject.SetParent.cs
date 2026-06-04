
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
    [ToolComment("Change a scene GameObject parent while controlling transform preservation.", "UnityReplGs.Tools.GameObjectSetParentTool.SetParent(new UnityReplGs.Tools.ObjectRef { name = \"Child\" }, new UnityReplGs.Tools.ObjectRef { name = \"Parent\" }, true)")]
    public static class GameObjectSetParentTool
    {
        public static string SetParent(ObjectRef gameObjectRef, ObjectRef parentGameObjectRef = null, bool worldPositionStays = true)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                var parent = ToolUtil.ResolveGameObject(parentGameObjectRef);
                Undo.SetTransformParent(go.transform, parent == null ? null : parent.transform, "Set Parent");
                go.transform.SetParent(parent == null ? null : parent.transform, worldPositionStays);
                EditorUtility.SetDirty(go);
                return ToolUtil.ToGameObjectData(go);
            });
        }
    }
}
