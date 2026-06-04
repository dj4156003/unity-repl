
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
    [ToolComment("Read metadata, serialized data, and selected property paths for an asset.", "UnityReplGs.Tools.AssetsGetDataTool.GetData(\"Assets/Scenes/Main.unity\", null, true, false, null)")]
    public static class AssetsGetDataTool
    {
        public static string GetData(string assetPath = null, string guid = null, bool includeSerialized = true, bool includeHidden = false, string[] paths = null)
        {
            return JsonOut.Try(() =>
            {
                var obj = AssetsTools.Load(assetPath, guid);
                if (paths != null && paths.Length > 0)
                {
                    return new
                    {
                        asset = AssetRef.FromObject(obj),
                        paths = paths.ToDictionary(path => path, path => ToolUtil.ReadPath(obj, path))
                    };
                }
                return ToolUtil.ToObjectSummary(obj, includeSerialized, includeHidden);
            });
        }
    }
}
