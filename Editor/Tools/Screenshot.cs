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
    public static class ScreenshotTools
    {
        public const string ScreenshotCameraToolId = "screenshot-camera";
        public const string ScreenshotGameViewToolId = "screenshot-game-view";
        public const string ScreenshotIsolatedToolId = "screenshot-isolated";
        public const string ScreenshotSceneViewToolId = "screenshot-scene-view";
    }
}
