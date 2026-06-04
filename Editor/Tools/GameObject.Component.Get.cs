
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
    [ToolComment(
        "Read component data from a scene GameObject.",
        "UnityReplGs.Tools.GameObjectComponentGetTool.Get(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"UnityEngine.Transform\", null, true, false)",
        "UnityReplGs.Tools.GameObjectComponentGetTool.ListOnGameObject(new UnityReplGs.Tools.ObjectRef { name = \"Player\" })"
    )]
    public static class GameObjectComponentGetTool
    {
        public static string Get(ObjectRef gameObjectRef, string componentType = null, ObjectRef componentRef = null, bool includeData = true, bool includeHidden = false)
        {
            return JsonOut.Try(() =>
            {
                var component = ToolUtil.ResolveComponent(gameObjectRef, componentRef, componentType);
                if (component == null)
                    return null;
                return new
                {
                    component = ToolUtil.ToComponentData(component),
                    data = includeData ? ToolUtil.ReadSerializedObject(component, includeHidden) : null
                };
            });
        }

        public static string ListOnGameObject(ObjectRef gameObjectRef)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    throw new System.ArgumentException("GameObject not found.");
                return go.GetComponents<UnityEngine.Component>().Where(c => c != null).Select(ToolUtil.ToComponentData).ToArray();
            });
        }
    }
}
