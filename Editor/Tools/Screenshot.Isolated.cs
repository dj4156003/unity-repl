
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
    [ToolComment("Render a target object with a temporary hidden camera and light setup to a PNG file.", "UnityReplGs.Tools.ScreenshotIsolatedTool.Capture(new UnityReplGs.Tools.ObjectRef { name = \"Player\" }, \"Temp/isolated.png\", 512, \"#404040\", 1.2f)")]
    public static class ScreenshotIsolatedTool
    {
        public static string Capture(ObjectRef targetRef, string outputPath = "Temp/unity-repl-gs-isolated.png", int size = 512, string backgroundColor = "#404040", float padding = 1.2f)
        {
            return JsonOut.Try(() =>
            {
                var target = ResolveTarget(targetRef);
                if (target == null)
                    throw new System.ArgumentException("Target GameObject not found.");
                var clone = UnityEngine.Object.Instantiate(target);
                clone.name = target.name + " (Screenshot)";
                clone.hideFlags = HideFlags.HideAndDontSave;
                clone.transform.position = Vector3.zero;
                clone.transform.rotation = Quaternion.identity;

                var cameraGo = new GameObject("unity-repl-gs-screenshot-camera");
                var lightGo = new GameObject("unity-repl-gs-screenshot-light");
                cameraGo.hideFlags = HideFlags.HideAndDontSave;
                lightGo.hideFlags = HideFlags.HideAndDontSave;
                try
                {
                    var light = lightGo.AddComponent<Light>();
                    light.type = LightType.Directional;
                    light.intensity = 1.2f;
                    lightGo.transform.rotation = Quaternion.Euler(45, -35, 0);

                    var camera = cameraGo.AddComponent<Camera>();
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    Color color;
                    camera.backgroundColor = ColorUtility.TryParseHtmlString(backgroundColor, out color) ? color : new Color(0.25f, 0.25f, 0.25f);
                    camera.fieldOfView = 60f;
                    camera.nearClipPlane = 0.01f;
                    camera.farClipPlane = 1000f;

                    var bounds = CalculateBounds(clone);
                    var radius = Mathf.Max(bounds.extents.magnitude, 0.5f) * Mathf.Max(1f, padding);
                    camera.transform.position = bounds.center + new Vector3(0, radius * 0.35f, -radius * 2.2f);
                    camera.transform.LookAt(bounds.center);
                    CaptureCamera(camera, outputPath, size, size);
                    return new { path = outputPath, size = size, exists = File.Exists(outputPath), target = ToolUtil.ToGameObjectData(target) };
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(clone);
                    UnityEngine.Object.DestroyImmediate(cameraGo);
                    UnityEngine.Object.DestroyImmediate(lightGo);
                }
            });
        }

        static GameObject ResolveTarget(ObjectRef targetRef)
        {
            var go = ToolUtil.ResolveGameObject(targetRef);
            if (go != null)
                return go;
            if (targetRef != null && !string.IsNullOrEmpty(targetRef.assetPath))
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(targetRef.assetPath);
            return null;
        }

        static Bounds CalculateBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return new Bounds(go.transform.position, Vector3.one);
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return bounds;
        }

        static void CaptureCamera(Camera camera, string outputPath, int width, int height)
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
