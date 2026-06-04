
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
    [ToolComment("Apply a serialized-property JSON patch to a UnityEngine.Object.", "UnityReplGs.Tools.ObjectModifyTool.Modify(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, UnityReplGs.Tools.JsonOut.ToJson(new { m_Name = \"Renamed\" }))")]
    public static class ObjectModifyTool
    {
        public static string Modify(ObjectRef objectRef, string serializedPatchJson)
        {
            return JsonOut.Try(() =>
            {
                var obj = ToolUtil.ResolveObject(objectRef);
                if (obj == null)
                    throw new System.ArgumentException("Object not found.");
                Undo.RecordObject(obj, "Modify Object");
                ToolUtil.ApplySerializedPatch(obj, serializedPatchJson);
                EditorUtility.SetDirty(obj);
                return ToolUtil.ToObjectSummary(obj, true, false);
            });
        }
    }
}
