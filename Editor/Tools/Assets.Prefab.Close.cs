
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
    [ToolComment("Close the current Prefab Stage, optionally saving changes first.", "UnityReplGs.Tools.AssetsPrefabCloseTool.Close(true)")]
    public static class AssetsPrefabCloseTool
    {
        public static string Close(bool save = true)
        {
            return JsonOut.Try(() =>
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage == null || stage.prefabContentsRoot == null)
                    throw new System.InvalidOperationException("Prefab stage is not opened.");
                var assetPath = stage.assetPath;
                if (save)
                    PrefabUtility.SaveAsPrefabAsset(stage.prefabContentsRoot, assetPath);
                stage.ClearDirtiness();
                StageUtility.GoBackToPreviousStage();
                return AssetRef.FromPath(assetPath);
            });
        }
    }
}
