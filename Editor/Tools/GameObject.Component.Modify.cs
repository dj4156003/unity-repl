
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
    [ToolComment("Modify a component through serialized-property patches or enabled state.", "UnityReplGs.Tools.GameObjectComponentModifyTool.Modify(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"UnityEngine.BoxCollider\", null, UnityReplGs.Tools.JsonOut.ToJson(new { m_IsTrigger = true }), null)")]
    public static class GameObjectComponentModifyTool
    {
        public static string Modify(ObjectRef gameObjectRef, string componentType = null, ObjectRef componentRef = null, string serializedPatchJson = null, bool? enabled = null)
        {
            return JsonOut.Try(() =>
            {
                var component = ToolUtil.ResolveComponent(gameObjectRef, componentRef, componentType);
                if (component == null)
                    throw new System.ArgumentException("Component not found.");
                Undo.RecordObject(component, "Modify Component");
                if (enabled.HasValue)
                {
                    var prop = component.GetType().GetProperty("enabled", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
                        prop.SetValue(component, enabled.Value, null);
                }
                ToolUtil.ApplySerializedPatch(component, serializedPatchJson);
                EditorUtility.SetDirty(component);
                return new { component = ToolUtil.ToComponentData(component), data = ToolUtil.ReadSerializedObject(component, false) };
            });
        }
    }
}
