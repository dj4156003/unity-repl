
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
    [ToolComment("Save the currently open Prefab Stage.", "UnityReplGs.Tools.AssetsPrefabSaveTool.Save()")]
    public static class AssetsPrefabSaveTool
    {
        public static string Save()
        {
            return JsonOut.Try(() =>
            {
                var stage = AssetsPrefabTools.CurrentStage();
                if (stage == null || stage.prefabContentsRoot == null)
                    throw new System.InvalidOperationException("Prefab stage is not opened.");
                var saved = PrefabUtility.SaveAsPrefabAsset(stage.prefabContentsRoot, stage.assetPath);
                stage.ClearDirtiness();
                return AssetRef.FromObject(saved);
            });
        }
    }
}
