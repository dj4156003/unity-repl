
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
    [ToolComment("Add a component type to a scene GameObject.", "UnityReplGs.Tools.GameObjectComponentAddTool.Add(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"UnityEngine.BoxCollider\")")]
    public static class GameObjectComponentAddTool
    {
        public static string Add(ObjectRef gameObjectRef, string componentType)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                var type = ToolUtil.FindType(componentType);
                if (type == null || !typeof(Component).IsAssignableFrom(type))
                    throw new System.ArgumentException("Component type not found: " + componentType);
                var component = Undo.AddComponent(go, type);
                EditorUtility.SetDirty(go);
                return ToolUtil.ToComponentData(component);
            });
        }
    }
}
