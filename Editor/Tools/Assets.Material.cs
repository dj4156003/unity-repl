
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
    public static class AssetsMaterialTools
    {
        public const string AssetsMaterialCreateToolId = "assets-material-create";

        public static Shader ResolveShader(string shaderName)
        {
            var shader = Shader.Find(string.IsNullOrEmpty(shaderName) ? "Standard" : shaderName);
            if (shader == null)
                throw new System.ArgumentException("Shader not found: " + shaderName);
            return shader;
        }
    }
}
