
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
    public static class SceneTools
    {
        public const string SceneCreateToolId = "scene-create";
        public const string SceneGetDataToolId = "scene-get-data";
        public const string SceneListOpenedToolId = "scene-list-opened";
        public const string SceneOpenToolId = "scene-open";
        public const string SceneSaveToolId = "scene-save";
        public const string SceneSetActiveToolId = "scene-set-active";
        public const string SceneUnloadToolId = "scene-unload";

        public static Scene ResolveScene(string scenePathOrName)
        {
            var scene = ToolUtil.FindScene(scenePathOrName);
            if (!scene.IsValid())
                throw new System.ArgumentException("Scene not found: " + scenePathOrName);
            return scene;
        }
    }
}
