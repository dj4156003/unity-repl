
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
        "Set the Unity Editor selection by object reference or instance id.",
        "UnityReplGs.Tools.EditorSelectionSetTool.Set(new[] { new UnityReplGs.Tools.ObjectRef { name = \"Player\" } }, true)",
        "UnityReplGs.Tools.EditorSelectionSetTool.SetByInstanceIds(new[] { 12345 }, false)"
    )]
    public static class EditorSelectionSetTool
    {
        public static string Set(ObjectRef[] objectRefs = null, bool ping = false)
        {
            return JsonOut.Try(() =>
            {
                var objects = objectRefs == null
                    ? new UnityEngine.Object[0]
                    : objectRefs.Select(ToolUtil.ResolveObject).Where(o => o != null).ToArray();
                Selection.objects = objects;
                if (ping && Selection.activeObject != null)
                    EditorGUIUtility.PingObject(Selection.activeObject);
                return new { selected = objects.Length, active = Selection.activeObject == null ? null : Selection.activeObject.name };
            });
        }

        public static string SetByInstanceIds(int[] instanceIds, bool ping = false)
        {
            return JsonOut.Try(() =>
            {
                if (instanceIds == null)
                    instanceIds = new int[0];
                Selection.instanceIDs = instanceIds;
                if (ping && Selection.activeObject != null)
                    EditorGUIUtility.PingObject(Selection.activeObject);
                return new { selected = Selection.objects.Length, instanceIds = Selection.instanceIDs };
            });
        }
    }
}
