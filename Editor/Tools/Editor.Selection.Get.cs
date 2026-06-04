
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
        "Read the current Unity Editor selection.",
        "UnityReplGs.Tools.EditorSelectionGetTool.Get()",
        "UnityReplGs.Tools.EditorSelectionGetTool.GetObjects(false)"
    )]
    public static class EditorSelectionGetTool
    {
        public static string Get()
        {
            return JsonOut.Try(() => new SelectionData
            {
                instanceIds = Selection.instanceIDs ?? new int[0],
                assetGuids = Selection.assetGUIDs ?? new string[0],
                activeObjectName = Selection.activeObject == null ? null : Selection.activeObject.name,
                activeObjectInstanceId = Selection.activeObject == null ? 0 : Selection.activeObject.GetInstanceID(),
                activeGameObjectPath = Selection.activeGameObject == null ? null : ToolUtil.GetGameObjectPath(Selection.activeGameObject)
            });
        }

        public static string GetObjects(bool includeSerialized = false)
        {
            return JsonOut.Try(() => Selection.objects.Select(o => ToolUtil.ToObjectSummary(o, includeSerialized, false)).ToArray());
        }
    }
}
