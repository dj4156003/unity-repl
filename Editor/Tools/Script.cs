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
    public static class ScriptTools
    {
        public const string ScriptDeleteToolId = "script-delete";
        public const string ScriptExecuteToolId = "script-execute";
        public const string ScriptReadToolId = "script-read";
        public const string ScriptUpdateOrCreateToolId = "script-update-or-create";
    }
}
