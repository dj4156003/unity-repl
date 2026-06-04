
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
    [ToolComment("Refresh the AssetDatabase with the requested import options.", "UnityReplGs.Tools.AssetsRefreshTool.Refresh()")]
    public static class AssetsRefreshTool
    {
        public static string Refresh(ImportAssetOptions options = ImportAssetOptions.Default)
        {
            return JsonOut.Try(() =>
            {
                AssetDatabase.Refresh(options);
                return new { refreshed = true, isCompiling = EditorApplication.isCompiling };
            });
        }
    }
}
