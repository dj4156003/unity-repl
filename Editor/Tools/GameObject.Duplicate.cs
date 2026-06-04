
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
    [ToolComment("Duplicate a scene GameObject and optionally rename the copy.", "UnityReplGs.Tools.GameObjectDuplicateTool.Duplicate(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"Player Copy\")")]
    public static class GameObjectDuplicateTool
    {
        public static string Duplicate(ObjectRef gameObjectRef, string newName = null)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                var copy = UnityEngine.Object.Instantiate(go, go.transform.parent);
                copy.name = string.IsNullOrEmpty(newName) ? go.name + " Copy" : newName;
                Undo.RegisterCreatedObjectUndo(copy, "Duplicate GameObject");
                EditorUtility.SetDirty(copy);
                return ToolUtil.ToGameObjectData(copy);
            });
        }
    }
}
