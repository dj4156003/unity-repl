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
    public static class EditorTools
    {
        public const string EditorApplicationGetStateToolId = "editor-application-get-state";
        public const string EditorApplicationSetStateToolId = "editor-application-set-state";
        public const string EditorSelectionGetToolId = "editor-selection-get";
        public const string EditorSelectionSetToolId = "editor-selection-set";
    }
}
