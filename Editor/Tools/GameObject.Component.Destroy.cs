
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
    [ToolComment("Remove a component from a scene GameObject.", "UnityReplGs.Tools.GameObjectComponentDestroyTool.Destroy(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"UnityEngine.BoxCollider\", null)")]
    public static class GameObjectComponentDestroyTool
    {
        public static string Destroy(ObjectRef gameObjectRef, string componentType = null, ObjectRef componentRef = null)
        {
            return JsonOut.Try(() =>
            {
                var component = ToolUtil.ResolveComponent(gameObjectRef, componentRef, componentType);
                if (component == null)
                    throw new System.ArgumentException("Component not found.");
                if (component is UnityEngine.Transform)
                    throw new System.InvalidOperationException("Transform cannot be destroyed.");
                var data = ToolUtil.ToComponentData(component);
                Undo.DestroyObjectImmediate(component);
                return new { destroyed = true, component = data };
            });
        }
    }
}
