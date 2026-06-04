
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
    [ToolComment("Render a camera to a PNG file.", "UnityReplGs.Tools.ScreenshotCameraTool.Capture(null, \"Temp/camera.png\", 1024, 768)")]
    public static class ScreenshotCameraTool
    {
        public static string Capture(ObjectRef cameraRef = null, string outputPath = "Temp/unity-repl-gs-camera.png", int width = 1024, int height = 768)
        {
            return JsonOut.Try(() =>
            {
                var camera = ResolveCamera(cameraRef);
                if (camera == null)
                    throw new System.ArgumentException("Camera not found.");
                Capture(camera, outputPath, width, height);
                return new { path = outputPath, width = width, height = height, exists = File.Exists(outputPath) };
            });
        }

        static Camera ResolveCamera(ObjectRef cameraRef)
        {
            if (cameraRef == null)
                return Camera.main ?? UnityEngine.Object.FindObjectOfType<Camera>();
            var obj = ToolUtil.ResolveObject(cameraRef);
            var camera = obj as Camera;
            if (camera != null)
                return camera;
            var go = obj as GameObject;
            return go == null ? null : go.GetComponent<Camera>();
        }

        static void Capture(Camera camera, string outputPath, int width, int height)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var rt = new RenderTexture(width, height, 24);
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = rt;
                camera.Render();
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();
                File.WriteAllBytes(outputPath, tex.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.DestroyImmediate(tex);
                UnityEngine.Object.DestroyImmediate(rt);
            }
        }
    }
}
