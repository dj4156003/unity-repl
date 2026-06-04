
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
    [ToolComment("Read shader metadata, properties, and compile messages.", "UnityReplGs.Tools.AssetsShaderGetDataTool.GetData(\"Universal Render Pipeline/Lit\", null, true, true)")]
    public static class AssetsShaderGetDataTool
    {
        public static string GetData(string shaderName = null, string assetPath = null, bool includeProperties = true, bool includeMessages = true)
        {
            return JsonOut.Try(() =>
            {
                Shader shader = null;
                if (!string.IsNullOrEmpty(assetPath))
                    shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
                if (shader == null && !string.IsNullOrEmpty(shaderName))
                    shader = Shader.Find(shaderName);
                if (shader == null)
                    throw new System.ArgumentException("Shader not found.");

                var properties = includeProperties
                    ? Enumerable.Range(0, shader.GetPropertyCount())
                        .Select(i => new
                        {
                            name = shader.GetPropertyName(i),
                            description = shader.GetPropertyDescription(i),
                            type = shader.GetPropertyType(i).ToString(),
                            flags = shader.GetPropertyFlags(i).ToString()
                        })
                        .ToArray()
                    : null;

                object messages = null;
                if (includeMessages)
                {
                    var messageMethod = typeof(ShaderUtil).GetMethod("GetShaderMessages", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    messages = messageMethod == null ? null : messageMethod.Invoke(null, new object[] { shader });
                }

                return new
                {
                    name = shader.name,
                    path = AssetDatabase.GetAssetPath(shader),
                    isSupported = shader.isSupported,
                    maximumLOD = shader.maximumLOD,
                    renderQueue = shader.renderQueue,
                    propertyCount = shader.GetPropertyCount(),
                    properties = properties,
                    messages = messages
                };
            });
        }
    }
}
