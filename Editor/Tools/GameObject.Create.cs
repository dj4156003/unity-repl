
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
    [ToolComment("Create an empty or primitive GameObject in the open scene.", "UnityReplGs.Tools.GameObjectCreateTool.Create(\"Probe Cube\", null, null, null, null, false, \"Cube\")")]
    public static class GameObjectCreateTool
    {
        public static string Create(string name, ObjectRef parentGameObjectRef = null, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool isLocalSpace = false, string primitiveType = null)
        {
            return JsonOut.Try(() =>
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("name is empty.");
                GameObject go;
                if (string.IsNullOrEmpty(primitiveType))
                {
                    go = new GameObject(name);
                }
                else
                {
                    var parsed = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), primitiveType, true);
                    go = GameObject.CreatePrimitive(parsed);
                    go.name = name;
                }
                var parent = ToolUtil.ResolveGameObject(parentGameObjectRef);
                if (parent != null)
                    go.transform.SetParent(parent.transform, false);
                ToolUtil.SetTransform(go, position ?? Vector3.zero, rotation ?? Vector3.zero, scale ?? Vector3.one, !isLocalSpace);
                Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
                EditorUtility.SetDirty(go);
                return ToolUtil.ToGameObjectData(go);
            });
        }
    }
}
