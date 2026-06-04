using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools.Data
{
    public class ToolToggleResult
    {
        public string name;
        public bool enabled;
        public bool found;
        public string message;
    }
}
