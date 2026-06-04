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
    [ToolComment("Find a scene GameObject and optionally include serialized data, components, or hierarchy.", "UnityReplGs.Tools.GameObjectFindTool.Find(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, true, false, true, false, 0)")]
    public static class GameObjectFindTool
    {
        public static string Find(ObjectRef gameObjectRef, bool includeData = false, bool includeHidden = false, bool includeComponents = false, bool includeHierarchy = false, int hierarchyDepth = 0)
        {
            return JsonOut.Try(() =>
            {
                var go = ToolUtil.ResolveGameObject(gameObjectRef);
                if (go == null)
                    return null;
                return ToolUtil.ToGameObjectSummary(go, includeData, includeHidden, includeComponents, includeHierarchy, hierarchyDepth);
            });
        }
    }
}
