
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
    [ToolComment("Read metadata, serialized data, and selected paths for a UnityEngine.Object.", "UnityReplGs.Tools.ObjectGetDataTool.GetData(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, true, false, null)")]
    public static class ObjectGetDataTool
    {
        public static string GetData(ObjectRef objectRef, bool includeSerialized = true, bool includeHidden = false, string[] paths = null)
        {
            return JsonOut.Try(() =>
            {
                var obj = ToolUtil.ResolveObject(objectRef);
                if (obj == null)
                    throw new System.ArgumentException("Object not found.");
                if (paths != null && paths.Length > 0)
                {
                    return new
                    {
                        objectRef = AssetRef.FromObject(obj),
                        paths = paths.ToDictionary(path => path, path => ToolUtil.ReadPath(obj, path))
                    };
                }
                return ToolUtil.ToObjectSummary(obj, includeSerialized, includeHidden);
            });
        }
    }
}
