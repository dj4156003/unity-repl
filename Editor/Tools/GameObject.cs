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
    public static class GameObjectTools
    {
        public const string GameObjectCreateToolId = "gameobject-create";
        public const string GameObjectDestroyToolId = "gameobject-destroy";
        public const string GameObjectDuplicateToolId = "gameobject-duplicate";
        public const string GameObjectFindToolId = "gameobject-find";
        public const string GameObjectModifyToolId = "gameobject-modify";
        public const string GameObjectSetParentToolId = "gameobject-set-parent";
    }
}
