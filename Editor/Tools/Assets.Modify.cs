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
    [ToolComment("Apply a serialized-property JSON patch to an asset and save it.", "UnityReplGs.Tools.AssetsModifyTool.Modify(\"Assets/Generated/Test.asset\", null, UnityReplGs.Tools.JsonOut.ToJson(new { m_Name = \"Renamed\" }))")]
    public static class AssetsModifyTool
    {
        public static string Modify(string assetPath = null, string guid = null, string serializedPatchJson = null)
        {
            return JsonOut.Try(() =>
            {
                var obj = AssetsTools.Load(assetPath, guid);
                ToolUtil.ApplySerializedPatch(obj, serializedPatchJson);
                UnityEditor.AssetDatabase.SaveAssets();
                return ToolUtil.ToObjectSummary(obj, true, false);
            });
        }
    }
}
