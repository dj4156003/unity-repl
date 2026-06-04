
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
    [ToolComment("Create a Material asset using a named shader.", "UnityReplGs.Tools.AssetsMaterialCreateTool.Create(\"Assets/Generated/Test.mat\", \"Standard\")")]
    public static class AssetsMaterialCreateTool
    {
        public static string Create(string assetPath, string shaderName = "Standard")
        {
            return JsonOut.Try(() =>
            {
                assetPath = ToolUtil.RequireAssetPath(assetPath);
                if (!assetPath.EndsWith(".mat", System.StringComparison.OrdinalIgnoreCase))
                    throw new System.ArgumentException("Material path must end with .mat.");
                ToolUtil.EnsureParentFolder(assetPath);
                var shader = Shader.Find(string.IsNullOrEmpty(shaderName) ? "Standard" : shaderName);
                if (shader == null)
                    throw new System.ArgumentException("Shader not found: " + shaderName);
                var material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
                AssetDatabase.SaveAssets();
                return AssetRef.FromObject(material);
            });
        }
    }
}
